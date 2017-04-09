using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Netsphere.Network.Message.GameRule;
using NLog;

// ReSharper disable once CheckNamespace
namespace Netsphere.Game.GameRules
{
    internal class ChaserGameRule : GameRuleBase
    {
        private const uint PlayersNeededToStart = 2; // ToDo change to 4

        private static readonly TimeSpan s_nextChaserWaitTime = TimeSpan.FromSeconds(8);
        private readonly Random _random = new Random();

        private TimeSpan _chaserRoundTime;
        private TimeSpan _chaserTimer;
        private TimeSpan _nextChaserTimer;

        private bool _waitingNextChaser;
        private Player _bonus;

        public override GameRule GameRule => GameRule.Chaser;
        public override Briefing Briefing { get; }

        public Player Chaser { get; private set; }

        public Player Bonus
        {
            get { return _bonus; }
            private set
            {
                if (_bonus == value)
                    return;
                _bonus = value;
                if (StateMachine.IsInState(GameRuleState.Playing))
                    Room.Broadcast(new SChangeBonusTargetAckMessage(_bonus?.Account.Id ?? 0));
            }
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ChaserGameRule(Room room)
            : base(room)
        {
            Briefing = new ChaserBriefing(this);

            StateMachine.Configure(GameRuleState.Waiting)
                .PermitIf(GameRuleStateTrigger.StartGame, GameRuleState.FullGame, CanStartGame);

            StateMachine.Configure(GameRuleState.FullGame)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.StartResult, GameRuleState.EnteringResult)
                .OnEntry(() =>
                {
                    _waitingNextChaser = true;
                    NextChaser();
                });

            StateMachine.Configure(GameRuleState.EnteringResult)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.StartResult, GameRuleState.Result);

            StateMachine.Configure(GameRuleState.Result)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.EndGame, GameRuleState.Waiting)
                .OnEntry(() =>
                {
                    Bonus = null;
                    Chaser = null;
                });
        }

        public override void Initialize()
        {
            Room.TeamManager.Add(Team.Alpha, (uint)Room.Options.MatchKey.PlayerLimit, (uint)Room.Options.MatchKey.SpectatorLimit);
            base.Initialize();
        }

        public override void Cleanup()
        {
            Room.TeamManager.Remove(Team.Alpha);
            base.Cleanup();
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            var teamMgr = Room.TeamManager;

            if (StateMachine.IsInState(GameRuleState.Playing) &&
                !StateMachine.IsInState(GameRuleState.EnteringResult) &&
                !StateMachine.IsInState(GameRuleState.Result))
            {
                if (StateMachine.IsInState(GameRuleState.FullGame))
                {
                    // Still have enough players?
                    if (teamMgr.PlayersPlaying.Count() < PlayersNeededToStart)
                        StateMachine.Fire(GameRuleStateTrigger.StartResult);

                    // Did we reach round limit?
                    if (RoundTime >= Room.Options.TimeLimit)
                        StateMachine.Fire(GameRuleStateTrigger.StartResult);

                    // ToDo - Is the chaser inside this room?

                    if (!_waitingNextChaser)
                    {
                        _chaserTimer += delta;
                        if (_chaserTimer >= _chaserRoundTime)
                        {
                            ChaserLose();
                        }

                        if (!teamMgr.Values.Any(team => team.Values.Any(plr =>
                                            plr != Chaser &&
                                            plr.RoomInfo.Mode == PlayerGameMode.Normal &&
                                            plr.RoomInfo.State != PlayerState.Dead)))
                        {
                            ChaserWin();
                        }
                    }
                    else
                    {
                        _nextChaserTimer += delta;
                        if (_nextChaserTimer >= s_nextChaserWaitTime)
                            NextChaser();
                    }
                }
            }
        }

        public override PlayerRecord GetPlayerRecord(Player plr)
        {
            return new ChaserPlayerRecord(plr);
        }

        public override void OnScoreKill(Player killer, Player assist, Player target, AttackAttribute attackAttribute)
        {
            if (target.RoomInfo.State == PlayerState.Waiting)
                return;

            if (target.RoomInfo.State == PlayerState.Alive)
                target.RoomInfo.State = PlayerState.Dead;

            var stats = GetRecord(killer);
            stats.Kills++;

            if (killer == Chaser && target == Bonus)
            {
                stats.BonusKills++;
                GetBonusPlayer();
            }

            if (Chaser == target)
            {
                ChaserLose();
            }

            base.OnScoreKill(killer, null, target, attackAttribute);
        }

        public override void OnScoreSuicide(Player plr)
        {
            if (plr.RoomInfo.State == PlayerState.Waiting)
                return;

            if (plr.RoomInfo.State == PlayerState.Alive)
                plr.RoomInfo.State = PlayerState.Dead;

            if (Chaser == plr)
            {
                ChaserLose();
            }

            if (plr == Bonus)
            {
                GetRecord(Chaser).BonusKills++;
                GetBonusPlayer();
            }

            base.OnScoreSuicide(plr);
        }

        //Triggered when attacking a friend, it's value should fill the SCORE BAR
        public virtual void OnChaserHit(Player chaser, Player attacker, float gunPoints, float meleePoints)
        {
            Logger.Info("[CSlaughterAttackPointReqMessage]-> Chaser " + chaser.Account.Nickname + " has been hit, attacked by " + attacker.Account.Nickname);
            Logger.Info("GunPoints: " + gunPoints + " | MeleePoints: " + meleePoints);
            //Room.Broadcast(new SSlaughterAttackPointAckMessage(chaser.Account.Id, gunPoints, meleePoints));
        }

        //Triggered when healing a friend, it's value should fill the SCORE BAR
        public virtual void OnChaserHeal(Player healer, float healPoints)
        {
            Logger.Info("[CSlaughterHealPointReq] PLAYER: " + healer.Account.Nickname + " | HealPoints: " + healPoints);
        }

        public void NextChaser()
        {
            if (Chaser != null && !_waitingNextChaser)
            {
                foreach (var plr in Room.TeamManager.PlayersPlaying)
                    plr.RoomInfo.State = PlayerState.Waiting;

                _waitingNextChaser = true;
                _nextChaserTimer = TimeSpan.Zero;
                Room.Broadcast(new SEventMessageAckMessage(GameEventMessage.ChaserIn, (ulong)s_nextChaserWaitTime.TotalMilliseconds, 0, 0, ""));
                return;
            }

            _chaserRoundTime = Room.Players.Count < 4 ? TimeSpan.FromSeconds(60) : TimeSpan.FromSeconds(Room.Players.Count * 15);
            _chaserRoundTime += TimeSpan.FromSeconds(Chaser != null ? 3 : 6);

            if (_chaserRoundTime + s_nextChaserWaitTime >= Room.Options.TimeLimit - RoundTime)
            {
                StateMachine.Fire(GameRuleStateTrigger.StartResult);
                return;
            }

            _waitingNextChaser = false;

            foreach (var plr in Room.TeamManager.PlayersPlaying)
                plr.RoomInfo.State = PlayerState.Alive;

            _chaserTimer = TimeSpan.Zero;
            var index = _random.Next(0, Room.Players.Count);
            Chaser = Room.Players.Values.ElementAt(index);
            GetRecord(Chaser).ChaserCount++;
            Room.Broadcast(new SChangeSlaughtererAckMessage(Chaser.Account.Id));
            GetBonusPlayer();
        }

        public void ChaserWin()
        {
            GetRecord(Chaser).Wins++;
            Room.Broadcast(new SScoreSLRoundWinAckMessage());
            NextChaser();
        }

        public void ChaserLose()
        {
            foreach (var plr in GetPlayersAlive())
                GetRecord(plr).Survived++;
            Room.Broadcast(new SScoreRoundWinAckMessage());
            NextChaser();
        }

        private bool CanStartGame()
        {
            if (!StateMachine.IsInState(GameRuleState.Waiting))
                return false;

            var countReady = Room.TeamManager.Values.Sum(team => team.Values.Count(plr => plr.RoomInfo.IsReady));
            if (countReady < PlayersNeededToStart - 1) // Sum doesn't include master so decrease players needed by 1
                return false;
            return true;
        }

        private void GetBonusPlayer()
        {
            Bonus = GetPlayersAlive().OrderByDescending(player => player.RoomInfo.Stats.TotalScore).FirstOrDefault();
        }

        private IEnumerable<Player> GetPlayersAlive()
        {
            return Room.TeamManager.PlayersPlaying.Where(plr => plr != Chaser && plr.RoomInfo.State == PlayerState.Alive);
        }

        private static ChaserPlayerRecord GetRecord(Player plr)
        {
            return (ChaserPlayerRecord)plr.RoomInfo.Stats;
        }
    }

    internal class ChaserBriefing : Briefing
    {
        public long CurrentChaser { get; set; }
        public long CurrentChaserTarget { get; set; }

        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
        public int Unk6 { get; set; } //  *(_BYTE *)(v7 + 60) = v23 == 1;

        public IList<int> Unk7 { get; set; }
        public IList<long> Unk8 { get; set; }
        public IList<long> Unk9 { get; set; }

        public ChaserBriefing(GameRuleBase gameRule)
            : base(gameRule)
        {
            Unk7 = new List<int>();
            Unk8 = new List<long>();
            Unk9 = new List<long>();
        }

        protected override void WriteData(BinaryWriter w, bool isResult)
        {
            base.WriteData(w, isResult);

            var gameRule = (ChaserGameRule)GameRule;
            CurrentChaser = (long)(gameRule.Chaser?.Account.Id ?? 0);
            //CurrentChaserTarget = 0;
            //Unk6 = 1;

            w.Write(CurrentChaser);
            w.Write(CurrentChaserTarget);

            w.Write(Unk3);
            w.Write(Unk4);
            w.Write(Unk5);
            w.Write(Unk6);

            w.Write(Unk7.Count);
            w.Write(Unk7);

            w.Write(Unk8.Count);
            w.Write(Unk8);

            w.Write(Unk9.Count);
            w.Write(Unk9);
        }
    }

    internal class ChaserPlayerRecord : PlayerRecord
    {
        public override uint TotalScore => GetTotalScore();

        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }
        public uint Unk4 { get; set; }
        public uint BonusKills { get; set; }
        public uint Unk5 { get; set; } //Increases points
        public uint Unk6 { get; set; } //Increases points
        public uint Unk7 { get; set; } //Increases points and did at some point instanced a second chaser
        public uint Unk8 { get; set; } //Increases points
        public uint Wins { get; set; } //Wins
        public uint Survived { get; set; }
        public uint Unk9 { get; set; } //Increases points
        public uint Unk10 { get; set; } //Increases points 
        public uint ChaserCount { get; set; }
        public uint Unk11 { get; set; } //Increases points
        public uint Unk12 { get; set; }
        public uint Unk13 { get; set; }
        public uint Unk14 { get; set; }
        public uint Unk15 { get; set; }
        public uint Unk16 { get; set; }

        public float Unk17 { get; set; }
        public float Unk18 { get; set; }
        public float Unk19 { get; set; }
        public float Unk20 { get; set; }

        public byte Unk21 { get; set; }

        public ChaserPlayerRecord(Player plr)
            : base(plr)
        { }

        public override void Serialize(BinaryWriter w, bool isResult)
        {
            base.Serialize(w, isResult);

            w.Write(TotalScore);
            w.Write(Unk2);
            w.Write(Unk3);
            w.Write(Unk4);
            w.Write(Kills);
            w.Write(BonusKills);
            w.Write(Unk5);
            w.Write(Unk6);
            w.Write(Unk7);
            w.Write(Unk8);
            w.Write(Wins);
            w.Write(Survived);
            w.Write(Unk9);
            w.Write(Unk10);
            w.Write(ChaserCount);
            w.Write(Unk11);
            w.Write(Unk12);
            w.Write(Unk13);
            w.Write(Unk14);
            w.Write(Unk15);
            w.Write(Unk16);

            w.Write(Unk17);
            w.Write(Unk18);
            w.Write(Unk19);
            w.Write(Unk20);

            w.Write(Unk21);
        }

        public override void Reset()
        {
            base.Reset();

            Unk1 = 0;
            Unk2 = 0;
            Unk3 = 0;
            Unk4 = 0;
            Kills = 0;
            BonusKills = 0;
            Unk5 = 0;
            Unk6 = 0;
            Unk7 = 0;
            Unk8 = 0;
            Wins = 0;
            Survived = 0;
            Unk9 = 0;
            Unk10 = 0;
            ChaserCount = 0;
            Unk11 = 0;
            Unk12 = 0;
            Unk13 = 0;
            Unk14 = 0;
            Unk15 = 0;
            Unk16 = 0;
            Unk17 = 0;
            Unk18 = 0;
            Unk19 = 0;
            Unk20 = 0;
            Unk21 = 0;
        }

        private uint GetTotalScore()
        {
            return Kills * 2 +
                BonusKills * 4 +
                Wins * 5 +
                Survived * 10;
        }
    }
}