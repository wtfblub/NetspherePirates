using System;
using System.Linq;
using System.Threading.Tasks;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.GameRule;
using Stateless;

// ReSharper disable once CheckNamespace

namespace Netsphere.Game.GameRules
{
    internal abstract class GameRuleBase
    {
        private static readonly TimeSpan PreHalfTimeWaitTime = TimeSpan.FromSeconds(9);
        private static readonly TimeSpan PreResultWaitTime = TimeSpan.FromSeconds(9);
        private static readonly TimeSpan HalfTimeWaitTime = TimeSpan.FromSeconds(24);
        private static readonly TimeSpan ResultWaitTime = TimeSpan.FromSeconds(14);

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
        { }

        public virtual void Cleanup()
        { }

        public virtual void Reload()
        { }

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

            #region HalfTime

            if (StateMachine.IsInState(GameRuleState.EnteringHalfTime))
            {
                if (RoundTime >= PreHalfTimeWaitTime)
                {
                    RoundTime = TimeSpan.Zero;
                    StateMachine.Fire(GameRuleStateTrigger.StartHalfTime);
                }
                else
                {
                    Room.BroadcastAsync(new SEventMessageAckMessage(GameEventMessage.HalfTimeIn, 2, 0, 0,
                        ((int)(PreHalfTimeWaitTime - RoundTime).TotalSeconds + 1).ToString())).WaitEx();
                }
            }

            if (StateMachine.IsInState(GameRuleState.HalfTime))
            {
                if (RoundTime >= HalfTimeWaitTime)
                    StateMachine.Fire(GameRuleStateTrigger.StartSecondHalf);
            }

            #endregion

            #region Result

            if (StateMachine.IsInState(GameRuleState.EnteringResult))
            {
                if (RoundTime >= PreResultWaitTime)
                {
                    RoundTime = TimeSpan.Zero;
                    StateMachine.Fire(GameRuleStateTrigger.StartResult);
                }
                else
                {
                    Room.BroadcastAsync(new SEventMessageAckMessage(GameEventMessage.ResultIn, 3, 0, 0,
                        (int)(PreResultWaitTime - RoundTime).TotalSeconds + 1 + " second(s)")).WaitEx();
                }
            }

            if (StateMachine.IsInState(GameRuleState.Result))
            {
                if (RoundTime >= ResultWaitTime)
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

                Room.BroadcastAsync(
                    new SScoreKillAssistAckMessage(new ScoreAssistDto(killer.RoomInfo.PeerId, assist.RoomInfo.PeerId,
                        target.RoomInfo.PeerId, attackAttribute))).WaitEx();
            }
            else
            {
                Room.BroadcastAsync(
                    new SScoreKillAckMessage(new ScoreDto(killer.RoomInfo.PeerId, target.RoomInfo.PeerId,
                        attackAttribute))).WaitEx();
            }
        }

        public virtual void OnScoreTeamKill(Player killer, Player target, AttackAttribute attackAttribute)
        {
            target.RoomInfo.Stats.Deaths++;

            Room.BroadcastAsync(
                new SScoreTeamKillAckMessage(new Score2Dto(killer.RoomInfo.PeerId, target.RoomInfo.PeerId,
                    attackAttribute))).WaitEx();
        }

        public virtual void OnScoreHeal(Player plr)
        {
            Room.BroadcastAsync(new SScoreHealAssistAckMessage(plr.RoomInfo.PeerId)).WaitEx();
        }

        public virtual void OnScoreSuicide(Player plr)
        {
            plr.RoomInfo.Stats.Deaths++;
            Room.BroadcastAsync(new SScoreSuicideAckMessage(plr.RoomInfo.PeerId, AttackAttribute.KillOneSelf)).WaitEx();
        }

        #endregion

        private void StateMachine_OnTransition(StateMachine<GameRuleState, GameRuleStateTrigger>.Transition transition)
        {
            RoundTime = TimeSpan.Zero;
            switch (transition.Destination)
            {
                case GameRuleState.FirstHalf:
                    GameTime = TimeSpan.Zero;
                    foreach (var team in Room.TeamManager.Values)
                        team.Score = 0;
                    foreach ( // ToDo Use one of the Player properties
                        var plr in
                            Room.TeamManager.Values.SelectMany(
                                team =>
                                    team.Values.Where(
                                        plr =>
                                            plr.RoomInfo.IsReady || Room.Master == plr ||
                                            plr.RoomInfo.Mode == PlayerGameMode.Spectate)))
                    {
                        plr.RoomInfo.Reset();
                        plr.RoomInfo.State = plr.RoomInfo.Mode == PlayerGameMode.Normal
                            ? PlayerState.Alive
                            : PlayerState.Spectating;
                        plr.Session.SendAsync(new SBeginRoundAckMessage()).WaitEx();
                    }

                    Room.BroadcastBriefingAsync().WaitEx();
                    Room.BroadcastAsync(new SChangeStateAckMessage(GameState.Playing)).WaitEx();
                    Room.BroadcastAsync(new SChangeSubStateAckMessage(GameTimeState.FirstHalf)).WaitEx();
                    break;

                case GameRuleState.HalfTime:
                    Room.BroadcastAsync(new SChangeSubStateAckMessage(GameTimeState.HalfTime)).WaitEx();
                    break;

                case GameRuleState.SecondHalf:
                    Room.BroadcastAsync(new SChangeSubStateAckMessage(GameTimeState.SecondHalf)).WaitEx();
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

                    Room.BroadcastAsync(new SChangeStateAckMessage(GameState.Result)).WaitEx();
                    Room.BroadcastBriefingAsync(true).WaitEx();
                    break;

                case GameRuleState.Waiting:
                    foreach (var plr in Room.TeamManager.Players.Where(plr => plr.RoomInfo.State != PlayerState.Lobby))
                    {
                        plr.RoomInfo.Reset();
                        plr.RoomInfo.State = PlayerState.Lobby;
                    }

                    Room.BroadcastAsync(new SChangeStateAckMessage(GameState.Waiting)).WaitEx();
                    Room.BroadcastBriefingAsync().WaitEx();
                    break;
            }
        }
    }
}