using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Game;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using NLog.Fluent;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class CharacterService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CharacterCreateReqMessage))]
        public void CreateCharacterHandler(GameSession session, CharacterCreateReqMessage message)
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
                session.SendAsync(new ServerResultAckMessage(ServerResult.CreateCharacterFailed));
            }
        }

        [MessageHandler(typeof(CharacterSelectReqMessage))]
        public void SelectCharacterHandler(GameSession session, CharacterSelectReqMessage message)
        {
            var plr = session.Player;

            // Prevents player from changing characters while playing
            if (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby &&
                !plr.Room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.HalfTime))
            {
                session.SendAsync(new ServerResultAckMessage(ServerResult.SelectCharacterFailed));
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
                session.SendAsync(new ServerResultAckMessage(ServerResult.SelectCharacterFailed));
            }
        }

        [MessageHandler(typeof(CharacterDeleteReqMessage))]
        public void DeleteCharacterHandler(GameSession session, CharacterDeleteReqMessage message)
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
                session.SendAsync(new ServerResultAckMessage(ServerResult.DeleteCharacterFailed));
            }
        }
    }
}
