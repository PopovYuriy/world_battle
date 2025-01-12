using System.Threading.Tasks;

namespace App.Modules.Matchmaking
{
    public interface IMatchmakerInitializable
    {
        Task InitializeAsync();
    }
}