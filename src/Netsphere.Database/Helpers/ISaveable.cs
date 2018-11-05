using System.Threading.Tasks;

namespace Netsphere.Database.Helpers
{
    public interface ISaveable
    {
        Task Save(GameContext db);
    }
}
