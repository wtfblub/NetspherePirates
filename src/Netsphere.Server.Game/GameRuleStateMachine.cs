using System;
using System.Diagnostics;
using System.Linq;
using Netsphere.Network.Message.GameRule;
using ProudNet.Hosting.Services;
using Stateless;

namespace Netsphere.Server.Game
{
    public class GameRuleStateMachine
    {
        private static readonly TimeSpan s_preHalfTimeWaitTime = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan s_preResultWaitTime = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan s_halfTimeWaitTime = TimeSpan.FromSeconds(25);
        private static readonly TimeSpan s_resultWaitTime = TimeSpan.FromSeconds(15);

        private readonly ISchedulerService _schedulerService;
        private readonly StateMachine<GameRuleState, GameRuleStateTrigger> _stateMachine;
        private readonly EventPipeline<ScheduleTriggerHookEventArgs> _scheduleTriggerHook;
        private GameRuleBase _gameRule;
        private Func<bool> _canStartGame;
        private bool _hasHalfTime;
        private DateTimeOffset _gameStartTime;
        private DateTimeOffset _roundStartTime;

        public event EventHandler GameStateChanged;
        public event EventHandler TimeStateChanged;
        public event EventPipeline<ScheduleTriggerHookEventArgs>.SubscriberDelegate ScheduleTriggerHook
        {
            add => _scheduleTriggerHook.Subscribe(value);
            remove => _scheduleTriggerHook.Unsubscribe(value);
        }

        protected virtual void OnGameStateChanged()
        {
            GameStateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnTimeStateChanged()
        {
            TimeStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public GameState GameState => GetGameState();
        public GameTimeState TimeState => GetTimeState();
        public TimeSpan GameTime => _gameStartTime == default ? TimeSpan.Zero : DateTimeOffset.Now - _gameStartTime;
        public TimeSpan RoundTime => _roundStartTime == default ? TimeSpan.Zero : DateTimeOffset.Now - _roundStartTime;

        public GameRuleStateMachine(ISchedulerService schedulerService)
        {
            _schedulerService = schedulerService;
            _stateMachine = new StateMachine<GameRuleState, GameRuleStateTrigger>(GameRuleState.Waiting);
            _stateMachine.OnTransitioned(OnTransition);
            _scheduleTriggerHook = new EventPipeline<ScheduleTriggerHookEventArgs>();
        }

        public void Initialize(GameRuleBase gameRule, Func<bool> canStartGame, bool hasHalfTime)
        {
            _gameRule = gameRule;
            _canStartGame = canStartGame;
            _hasHalfTime = hasHalfTime;

            _stateMachine.Configure(GameRuleState.Waiting)
                .PermitIf(GameRuleStateTrigger.StartGame, GameRuleState.FirstHalf, _canStartGame);

            var firstHalfStateMachine = _stateMachine.Configure(GameRuleState.FirstHalf)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.StartResult, GameRuleState.EnteringResult);

            if (hasHalfTime)
            {
                firstHalfStateMachine.Permit(GameRuleStateTrigger.StartHalfTime, GameRuleState.EnteringHalfTime);

                _stateMachine.Configure(GameRuleState.EnteringHalfTime)
                    .SubstateOf(GameRuleState.Playing)
                    .Permit(GameRuleStateTrigger.StartHalfTime, GameRuleState.HalfTime)
                    .Permit(GameRuleStateTrigger.StartResult, GameRuleState.EnteringResult);

                _stateMachine.Configure(GameRuleState.HalfTime)
                    .SubstateOf(GameRuleState.Playing)
                    .Permit(GameRuleStateTrigger.StartSecondHalf, GameRuleState.SecondHalf)
                    .Permit(GameRuleStateTrigger.StartResult, GameRuleState.EnteringResult);

                _stateMachine.Configure(GameRuleState.SecondHalf)
                    .SubstateOf(GameRuleState.Playing)
                    .Permit(GameRuleStateTrigger.StartResult, GameRuleState.EnteringResult);
            }

            _stateMachine.Configure(GameRuleState.EnteringResult)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.StartResult, GameRuleState.Result);

            _stateMachine.Configure(GameRuleState.Result)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.EndGame, GameRuleState.Waiting);
        }

        public bool StartGame()
        {
            if (!_stateMachine.CanFire(GameRuleStateTrigger.StartGame))
                return false;

            _stateMachine.Fire(GameRuleStateTrigger.StartGame);
            return true;
        }

        public bool StartHalfTime()
        {
            if (!_stateMachine.CanFire(GameRuleStateTrigger.StartHalfTime))
                return false;

            _stateMachine.Fire(GameRuleStateTrigger.StartHalfTime);
            return true;
        }

        public bool StartResult()
        {
            if (!_stateMachine.CanFire(GameRuleStateTrigger.StartResult))
                return false;

            _stateMachine.Fire(GameRuleStateTrigger.StartResult);
            return true;
        }

        private GameState GetGameState()
        {
            if (_stateMachine.IsInState(GameRuleState.Waiting))
                return GameState.Waiting;

            if (_stateMachine.IsInState(GameRuleState.Result))
                return GameState.Result;

            if (_stateMachine.IsInState(GameRuleState.Playing))
                return GameState.Playing;

            Debug.Assert(false, "Invalid state machine - THIS SHOULD NEVER HAPPEN");
            return default;
        }

        private GameTimeState GetTimeState()
        {
            if (_stateMachine.IsInState(GameRuleState.HalfTime))
                return GameTimeState.HalfTime;

            if (_stateMachine.IsInState(GameRuleState.SecondHalf))
                return GameTimeState.SecondHalf;

            return GameTimeState.FirstHalf;
        }

        private void OnTransition(StateMachine<GameRuleState, GameRuleStateTrigger>.Transition transition)
        {
            var room = _gameRule.Room;
            _roundStartTime = DateTimeOffset.Now;

            switch (transition.Destination)
            {
                case GameRuleState.EnteringHalfTime:
                    ScheduleTrigger(GameRuleStateTrigger.StartHalfTime, s_preHalfTimeWaitTime);
                    AnnounceHalfTime();
                    OnTimeStateChanged();
                    break;

                case GameRuleState.EnteringResult:
                    ScheduleTrigger(GameRuleStateTrigger.StartResult, s_preResultWaitTime);
                    AnnounceResult();
                    break;

                case GameRuleState.FirstHalf:
                    _gameStartTime = DateTimeOffset.Now;
                    foreach (var team in room.TeamManager.Values)
                        team.Score = 0;

                    foreach (var plr in room.TeamManager.Players)
                    {
                        if (!plr.IsReady && plr != room.Master)
                            continue;

                        for (var i = 0; i < plr.CharacterStartPlayTime.Length; ++i)
                            plr.CharacterStartPlayTime[i] = default;

                        plr.CharacterStartPlayTime[plr.CharacterManager.CurrentSlot] = DateTimeOffset.Now;
                        plr.StartPlayTime = DateTimeOffset.Now;
                        plr.IsReady = false;
                        plr.Score.Reset();
                        plr.State = plr.Mode == PlayerGameMode.Normal
                            ? PlayerState.Alive
                            : PlayerState.Spectating;
                        plr.Session.Send(new SBeginRoundAckMessage());
                    }

                    room.BroadcastBriefing();
                    room.Broadcast(new SChangeStateAckMessage(GameState.Playing));
                    room.Broadcast(new SChangeSubStateAckMessage(GameTimeState.FirstHalf));
                    var delay = _hasHalfTime
                        ? TimeSpan.FromSeconds(room.Options.TimeLimit.TotalSeconds / 2)
                        : room.Options.TimeLimit;
                    ScheduleTrigger(_hasHalfTime ? GameRuleStateTrigger.StartHalfTime : GameRuleStateTrigger.StartResult, delay);
                    OnTimeStateChanged();
                    OnGameStateChanged();
                    break;

                case GameRuleState.HalfTime:
                    ScheduleTrigger(GameRuleStateTrigger.StartSecondHalf, s_halfTimeWaitTime);
                    room.Broadcast(new SChangeSubStateAckMessage(GameTimeState.HalfTime));
                    break;

                case GameRuleState.SecondHalf:
                    ScheduleTrigger(GameRuleStateTrigger.StartResult,
                        TimeSpan.FromMinutes(room.Options.TimeLimit.TotalMinutes / 2));
                    room.Broadcast(new SChangeSubStateAckMessage(GameTimeState.SecondHalf));
                    OnTimeStateChanged();
                    break;

                case GameRuleState.Result:
                    ScheduleTrigger(GameRuleStateTrigger.EndGame, s_resultWaitTime);

                    foreach (var plr in room.TeamManager.Players.Where(plr => plr.State != PlayerState.Lobby))
                        plr.State = PlayerState.Waiting;

                    room.Broadcast(new SChangeStateAckMessage(GameState.Result));
                    _gameRule.OnResult();
                    OnGameStateChanged();
                    break;

                case GameRuleState.Waiting:
                    foreach (var plr in room.TeamManager.Players.Where(plr => plr.State != PlayerState.Lobby))
                        plr.State = PlayerState.Lobby;

                    room.Broadcast(new SChangeStateAckMessage(GameState.Waiting));
                    room.BroadcastBriefing();
                    OnGameStateChanged();
                    break;
            }
        }

        private void ScheduleTrigger(GameRuleStateTrigger trigger, TimeSpan delay)
        {
            var eventArgs = new ScheduleTriggerHookEventArgs(trigger, delay);
            _scheduleTriggerHook.Invoke(eventArgs);
            if (eventArgs.Cancel)
                return;

            trigger = eventArgs.Trigger;
            delay = eventArgs.Delay;

            _schedulerService.ScheduleAsync((ctx, state) =>
            {
                var This = (GameRuleStateMachine)ctx;
                var parameter = (GameRuleStateTrigger)state;

                if (This._stateMachine.CanFire(parameter))
                    This._stateMachine.Fire(parameter);
            }, this, trigger, delay);
        }

        private void AnnounceHalfTime(bool isFirst = true)
        {
            if (isFirst)
            {
                _gameRule.Room.Broadcast(new SEventMessageAckMessage(
                    GameEventMessage.HalfTimeIn, 2, 0, 0,
                    Math.Round((s_preHalfTimeWaitTime - RoundTime).TotalSeconds, 0).ToString("0")));
            }

            _schedulerService.ScheduleAsync((ctx, _) =>
            {
                var This = (GameRuleStateMachine)ctx;
                if (!This._stateMachine.IsInState(GameRuleState.EnteringHalfTime))
                    return;

                This._gameRule.Room.Broadcast(new SEventMessageAckMessage(
                    GameEventMessage.HalfTimeIn, 2, 0, 0,
                    Math.Round((s_preHalfTimeWaitTime - This.RoundTime).TotalSeconds, 0).ToString("0")));

                This.AnnounceHalfTime(false);
            }, this, null, TimeSpan.FromSeconds(1));
        }

        private void AnnounceResult(bool isFirst = true)
        {
            if (isFirst)
            {
                _gameRule.Room.Broadcast(new SEventMessageAckMessage(
                    GameEventMessage.ResultIn, 3, 0, 0,
                    (int)Math.Round((s_preResultWaitTime - RoundTime).TotalSeconds, 0) + " second(s)"));
            }

            _schedulerService.ScheduleAsync((ctx, _) =>
            {
                var This = (GameRuleStateMachine)ctx;
                if (!This._stateMachine.IsInState(GameRuleState.EnteringResult))
                    return;

                This._gameRule.Room.Broadcast(new SEventMessageAckMessage(
                    GameEventMessage.ResultIn, 3, 0, 0,
                    (int)Math.Round((s_preResultWaitTime - This.RoundTime).TotalSeconds, 0) + " second(s)"));

                This.AnnounceResult(false);
            }, this, null, TimeSpan.FromSeconds(1));
        }
    }
}
