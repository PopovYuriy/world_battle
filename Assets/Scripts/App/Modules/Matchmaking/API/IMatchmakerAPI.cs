using System.Collections.Generic;
using System.Threading.Tasks;
using App.Modules.Matchmaking.API.Data;
using App.Modules.Matchmaking.Data;
using Core.API.Common;

namespace App.Modules.Matchmaking.API
{
    public interface IMatchmakerAPI
    {
        Task<PendingGameData>     GetExistPendingGameAsync(string userId);
        Task<IEnumerable<string>> GetKeywordsInUseAsync();
        Task<CreateGameResult>    CreatePendingGameAsync(string keyword, string userId);
        Task<IDataObserver<bool>> CreateSessionCreatedObserverAsync(string userId);
        Task<JoinGameResult>      TryJoinToGameAsync(string keyword, string userUid, float expirationTime);
        Task<JoinGameResult>      MarkPendingGameSessionCreatedAsync(string gameId); 
        Task                      DeletePendingGameAsync();
        Task<bool>                CheckPendingGameIsExpiredAsync(PendingGameData data, float expirationTime);
    }
}