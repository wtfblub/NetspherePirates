using BlubLib.Network.Pipes;
using Netsphere.Network.Message.Game;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using NLog.Fluent;

namespace Netsphere.Network.Services
{
    internal class CharacterService : MessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CCreateCharacterReqMessage))]
        public void CreateCharacterHandler(GameSession session, CCreateCharacterReqMessage message)
        {
            Logger.Info()
                .Account(session)
                .Message($"Creating character: {JsonConvert.SerializeObject(message, new StringEnumConverter())}")
                .Write();

            try
            {
                session.Player.CharacterManager.Create(message.Slot, message.Style.Gender, message.Style.Hair, message.Style.Face, message.Style.Shirt, message.Style.Pants);
            }
            catch (CharacterException ex)
            {
                Logger.Error()
                    .Account(session)
                    .Message(ex.Message)
                    .Write();
                session.Send(new SServerResultInfoAckMessage(ServerResult.CreateCharacterFailed));
            }
        }

        [MessageHandler(typeof(CSelectCharacterReqMessage))]
        public void SelectCharacterHandler(GameSession session, CSelectCharacterReqMessage message)
        {
            var plr = session.Player;

            // Prevents player from changing characters while playing
            if (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby &&
                !plr.Room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.HalfTime))
            {
                session.Send(new SServerResultInfoAckMessage(ServerResult.SelectCharacterFailed));
                return;
            }

            Logger.Info()
                .Account(session)
                .Message($"Selecting character {message.Slot}")
                .Write();

            try
            {
                plr.CharacterManager.Select(message.Slot);
            }
            catch (CharacterException ex)
            {
                Logger.Error()
                    .Account(session)
                    .Message(ex.Message)
                    .Write();
                session.Send(new SServerResultInfoAckMessage(ServerResult.SelectCharacterFailed));
            }
        }

        [MessageHandler(typeof(CDeleteCharacterReqMessage))]
        public void DeleteCharacterHandler(GameSession session, CDeleteCharacterReqMessage message)
        {
            Logger.Info()
                .Account(session)
                .Message($"Removing character {message.Slot}")
                .Write();

            try
            {
                session.Player.CharacterManager.Remove(message.Slot);
            }
            catch (CharacterException ex)
            {
                Logger.Error()
                    .Account(session)
                    .Message(ex.Message)
                    .Write();
                session.Send(new SServerResultInfoAckMessage(ServerResult.DeleteCharacterFailed));
            }
        }
    }
}
