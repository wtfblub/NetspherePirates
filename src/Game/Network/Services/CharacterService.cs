using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Game;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProudNet.Handlers;
using Serilog;
using Serilog.Core;

namespace Netsphere.Network.Services
{
    internal class CharacterService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(CharacterService));

        [MessageHandler(typeof(CCreateCharacterReqMessage))]
        public void CreateCharacterHandler(GameSession session, CCreateCharacterReqMessage message)
        {
            Logger.ForAccount(session)
                .Information("Creating character: {message}", JsonConvert.SerializeObject(message, new StringEnumConverter()));

            try
            {
                session.Player.CharacterManager.Create(message.Slot, message.Style.Gender, message.Style.Hair, message.Style.Face, message.Style.Shirt, message.Style.Pants);
            }
            catch (CharacterException ex)
            {
                Logger.ForAccount(session)
                    .Error(ex.Message);
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.CreateCharacterFailed));
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
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.SelectCharacterFailed));
                return;
            }

            Logger.ForAccount(session)
                .Information("Selecting character {slot}", message.Slot);

            try
            {
                plr.CharacterManager.Select(message.Slot);
            }
            catch (CharacterException ex)
            {
                Logger.ForAccount(session)
                    .Error(ex.Message);
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.SelectCharacterFailed));
            }
        }

        [MessageHandler(typeof(CDeleteCharacterReqMessage))]
        public void DeleteCharacterHandler(GameSession session, CDeleteCharacterReqMessage message)
        {
            Logger.ForAccount(session)
                .Information("Removing character {slot}", message.Slot);

            try
            {
                session.Player.CharacterManager.Remove(message.Slot);
            }
            catch (CharacterException ex)
            {
                Logger.ForAccount(session)
                    .Error(ex.Message);
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.DeleteCharacterFailed));
            }
        }
    }
}
