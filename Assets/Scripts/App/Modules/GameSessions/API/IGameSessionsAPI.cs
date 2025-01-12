using System.Collections.Generic;
using System.Threading.Tasks;
using App.Modules.GameSessions.API.Data;
using App.Modules.GameSessions.Controller;
using App.Modules.GameSessions.Data;

namespace App.Modules.GameSessions.API
{
    public interface IGameSessionsAPI
    {
        Task<IEnumerable<IGameSessionController>> LoadExistGamesAsync(string userId);
        Task<IGameSessionController>              LoadGamesAsync(string userId, string gameSessionId);
        Task<UserResult>                          GetUserAsync(string userId);
        Task<IGameSessionController>              CreateGameSessionAsync(GameSessionData data, string hostId);
        Task                                      DeleteGameFromListAsync(string userId, string gameSessionId);
    }
}