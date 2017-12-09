using System;
using System.Linq;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.GameRule;
using Stateless;

// ReSharper disable once CheckNamespace

namespace Netsphere.Game.GameRules
{
    internal abstract class GameRuleBase
    {
        private static readonly TimeSpan s_preHalfTimeWaitTime = TimeSpan.FromSeconds(9);
        private static readonly TimeSpan s_preResultWaitTime = TimeSpan.FromSeconds(9);
        private static readonly TimeSpan s_halfTimeWaitTime = TimeSpan.FromSeconds(24);
        private static readonly TimeSpan s_resultWaitTime = TimeSpan.FromSeconds(14);
        public static readonly TimeSpan StartingWaitTime = TimeSpan.FromSeconds(6);
        private bool _sentStartingCountdown;

        public abstract GameRule GameRule { get; }
        public Room Room { get; }
        public abstract Briefing Briefing { get; }
        public StateMachine<GameRuleState, GameRuleStateTrigger> StateMachine { get; }

        public TimeSpan GameTime { get; private set; }
        public TimeSpan RoundTime { get; private set; }

        protected GameRuleBase(Room room)
        {
            Room = room;
            StateMachine = new StateMachine<GameRuleState, GameRuleStateTrigger>(GameRuleState.Waiting);
            StateMachine.OnTransitioned(StateMachine_OnTransition);
        }

        public virtual void Initialize()
        {
        }

        public virtual void Cleanup()
        {
        }

        public virtual void Reload()
        {
        }

        public virtual void Update(TimeSpan delta)
        {
            RoundTime += delta;
            if (StateMachine.IsInState(GameRuleState.Playing))
            {
                GameTime += delta;

                foreach (var plr in Room.TeamManager.PlayersPlaying)
                {
                    plr.RoomInfo.PlayTime += delta;
                    plr.RoomInfo.CharacterPlayTime[plr.CharacterManager.CurrentSlot] += delta;
                }
            }

            #region Starting
            if (StateMachine.IsInState(GameRuleState.Starting))
            {
                if (!_sentStartingCountdown)
                {
                    // Wait a second before starting the countdown to prevent client bugs
                    if (RoundTime >= TimeSpan.FromSeconds(1))
                    {
                        foreach (var plr in Room.TeamManager.PlayersInGame)
                        {
                            // Prevents the client from using weapons
                            plr.Session.SendAsync(new SEventMessageAckMessage(GameEventMessage.HalfTimeIn, 1, 0, 0, ""));

                            // Starts the next round countdown
                            plr.Session.SendAsync(new SEventMessageAckMessage(GameEventMessage.NextRoundIn,
                                (ulong)StartingWaitTime.TotalMilliseconds, 0, 0, ""));
                        }

                        _sentStartingCountdown = true;
                        RoundTime = TimeSpan.Zero;
                    }
                }
                else
                {
                    if (RoundTime >= StartingWaitTime)
                    {
                        RoundTime = TimeSpan.Zero;
                        StateMachine.Fire(GameRuleStateTrigger.StartGame);

                        // The client needs ResetRound after NextRoundIn
                        foreach (var plr in Room.TeamManager.PlayersInGame)
                        {
                            // HalfTimeIn gets the client stuck after ResetRound so we need to reset the sub state
                            //plr.Session.SendAsync(new SChangeSubStateAckMessage(GameTimeState.FirstHalf));
                            plr.Session.SendAsync(new SEventMessageAckMessage(GameEventMessage.ResetRound, 0, 0, 0, ""));
                        }
                    }
                }
            }
            #endregion

            #region HalfTime
            if (StateMachine.IsInState(GameRuleState.EnteringHalfTime))
            {
                if (RoundTime >= s_preHalfTimeWaitTime)
                {
                    RoundTime = TimeSpan.Zero;
                    StateMachine.Fire(GameRuleStateTrigger.StartHalfTime);
                }
                else
                {
                    Room.Broadcast(new SEventMessageAckMessage(GameEventMessage.HalfTimeIn, 2, 0, 0,
                        ((int)(s_preHalfTimeWaitTime - RoundTime).TotalSeconds + 1).ToString()));
                }
            }

            if (StateMachine.IsInState(GameRuleState.HalfTime))
            {
                if (RoundTime >= s_halfTimeWaitTime)
                    StateMachine.Fire(GameRuleStateTrigger.StartSecondHalf);
            }
            #endregion

            #region Result
            if (StateMachine.IsInState(GameRuleState.EnteringResult))
            {
                if (RoundTime >= s_preResultWaitTime)
                {
                    RoundTime = TimeSpan.Zero;
                    StateMachine.Fire(GameRuleStateTrigger.StartResult);
                }
                else
                {
                    Room.Broadcast(new SEventMessageAckMessage(GameEventMessage.ResultIn, 3, 0, 0,
                        (int)(s_preResultWaitTime - RoundTime).TotalSeconds + 1 + " second(s)"));
                }
            }

            if (StateMachine.IsInState(GameRuleState.Result))
            {
                if (RoundTime >= s_resultWaitTime)
                    StateMachine.Fire(GameRuleStateTrigger.EndGame);
            }
            #endregion
        }

        public abstract PlayerRecord GetPlayerRecord(Player plr);

        #region Scores
        public virtual void OnScoreKill(Player killer, Player assist, Player target, AttackAttribute attackAttribute)
        {
            killer.RoomInfo.Stats.Kills++;
            target.RoomInfo.Stats.Deaths++;

            if (assist != null)
            {
                assist.RoomInfo.Stats.KillAssists++;

                Room.Broadcast(
                    new SScoreKillAssistAckMessage(new ScoreAssistDto(killer.RoomInfo.PeerId, assist.RoomInfo.PeerId,
                        target.RoomInfo.PeerId, attackAttribute)));
            }
            else
            {
                Room.Broadcast(
                    new SScoreKillAckMessage(new ScoreDto(killer.RoomInfo.PeerId, target.RoomInfo.PeerId,
                        attackAttribute)));
            }
        }

        public virtual void OnScoreTeamKill(Player killer, Player target, AttackAttribute attackAttribute)
        {
            target.RoomInfo.Stats.Deaths++;

            Room.Broadcast(
                new SScoreTeamKillAckMessage(new Score2Dto(killer.RoomInfo.PeerId, target.RoomInfo.PeerId,
                    attackAttribute)));
        }

        public virtual void OnScoreHeal(Player plr)
        {
            Room.Broadcast(new SScoreHealAssistAckMessage(plr.RoomInfo.PeerId));
        }

        public virtual void OnScoreSuicide(Player plr)
        {
            plr.RoomInfo.Stats.Deaths++;
            Room.Broadcast(new SScoreSuicideAckMessage(plr.RoomInfo.PeerId, AttackAttribute.KillOneSelf));
        }
        #endregion

        private void StateMachine_OnTransition(StateMachine<GameRuleState, GameRuleStateTrigger>.Transition transition)
        {
            RoundTime = TimeSpan.Zero;
            switch (transition.Destination)
            {
                case GameRuleState.Starting:
                    _sentStartingCountdown = false;
                    foreach (var team in Room.TeamManager.Values)
                        team.Score = 0;

                    foreach (var plr in Room.TeamManager.Values.SelectMany(
                            team => team.Values.Where(
                                plr => plr.RoomInfo.IsReady || Room.Master == plr)))
                    {
                        plr.RoomInfo.Reset();
                        plr.RoomInfo.State = plr.RoomInfo.Mode == PlayerGameMode.Normal
                            ? PlayerState.Alive
                            : PlayerState.Spectating;
                        plr.Session.SendAsync(new SBeginRoundAckMessage());
                    }

                    Room.BroadcastBriefing();
                    Room.Broadcast(new SChangeStateAckMessage(GameState.Playing));
                    Room.Broadcast(new SChangeSubStateAckMessage(GameTimeState.FirstHalf));

                    // Friendly games have no starting countdown
                    if (Room.Options.IsFriendly)
                        StateMachine.Fire(GameRuleStateTrigger.StartGame);

                    break;

                case GameRuleState.FirstHalf:
                    GameTime = TimeSpan.Zero;

                    // Make sure the client resets the first half timer after the starting countdown
                    Room.Broadcast(new SChangeSubStateAckMessage(GameTimeState.FirstHalf));
                    break;

                case GameRuleState.HalfTime:
                    Room.Broadcast(new SChangeSubStateAckMessage(GameTimeState.HalfTime));
                    break;

                case GameRuleState.SecondHalf:
                    Room.Broadcast(new SChangeSubStateAckMessage(GameTimeState.SecondHalf));
                    break;

                case GameRuleState.Result:
                    foreach (var plr in Room.TeamManager.PlayersPlaying)
                    {
                        foreach (var @char in plr.CharacterManager)
                        {
                            var loss = (int)plr.RoomInfo.CharacterPlayTime[@char.Slot].TotalMinutes *
                                       Config.Instance.Game.DurabilityLossPerMinute;
                            loss += (int)plr.RoomInfo.Stats.Deaths * Config.Instance.Game.DurabilityLossPerDeath;

                            foreach (var item in @char.Weapons.GetItems().Where(item => item != null && item.Durability != -1))
                                item.LoseDurabilityAsync(loss).Wait();

                            foreach (var item in @char.Costumes.GetItems().Where(item => item != null && item.Durability != -1))
                                item.LoseDurabilityAsync(loss).Wait();

                            foreach (var item in @char.Skills.GetItems().Where(item => item != null && item.Durability != -1))
                                item.LoseDurabilityAsync(loss).Wait();
                        }
                    }

                    foreach (var plr in Room.TeamManager.Players.Where(plr => plr.RoomInfo.State != PlayerState.Lobby))
                        plr.RoomInfo.State = PlayerState.Waiting;

                    Room.Broadcast(new SChangeStateAckMessage(GameState.Result));
                    Room.BroadcastBriefing(true);
                    break;

                case GameRuleState.Waiting:
                    foreach (var plr in Room.TeamManager.Players.Where(plr => plr.RoomInfo.State != PlayerState.Lobby))
                    {
                        plr.RoomInfo.Reset();
                        plr.RoomInfo.State = PlayerState.Lobby;
                    }

                    Room.Broadcast(new SChangeStateAckMessage(GameState.Waiting));
                    Room.BroadcastBriefing();
                    break;
            }
        }
    }
}
