using ProudNet.Serialization;

namespace Netsphere.Network.Message.GameRule
{
    public interface IGameRuleMessage
    {
    }

    public class GameRuleMessageFactory : MessageFactory<GameRuleOpCode, IGameRuleMessage>
    {
        public GameRuleMessageFactory()
        {
            // S2C

            // C2S
        }
    }
}
