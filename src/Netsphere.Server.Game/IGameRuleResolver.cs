namespace Netsphere.Server.Game
{
    public interface IGameRuleResolver
    {
        GameRuleBase Resolve(Room room);
    }
}
