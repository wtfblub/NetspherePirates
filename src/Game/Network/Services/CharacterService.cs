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

        [MessageHandler(typeof(CCreateCharacterReqMessage))]
        public async Task CreateCharacterHandler(GameSession session, CCreateCharacterReqMessage message)
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
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.CreateCharacterFailed))
                    .ConfigureAwait(false);
            }
        }

        [MessageHandler(typeof(CSelectCharacterReqMessage))]
        public async Task SelectCharacterHandler(GameSession session, CSelectCharacterReqMessage message)
        {
            var plr = session.Player;

            // Prevents player from changing characters while playing
            if (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby &&
                !plr.Room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.HalfTime))
            {
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.SelectCharacterFailed))
                    .ConfigureAwait(false);
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
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.SelectCharacterFailed))
                    .ConfigureAwait(false);
            }
        }

        [MessageHandler(typeof(CDeleteCharacterReqMessage))]
        public async Task DeleteCharacterHandler(GameSession session, CDeleteCharacterReqMessage message)
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
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.DeleteCharacterFailed))
                    .ConfigureAwait(false);
            }
        }
    }
}
