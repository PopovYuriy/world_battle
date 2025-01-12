using System.Collections.Generic;
using System.Threading.Tasks;
using App.Modules.GameSessions.Controller;
using App.Modules.GameSessions.Data;

namespace App.Modules.GameSessions
{
    public interface IGameSessionsManager
    {
        IGameSessionController       LocalController { get; }
        Task<IGameSessionController> CreateOnlineGame(string sessionId, string opponentId);
        Task<bool>                   TryLoadAndAddGameAsync(string sessionId, string playerId);
        void                         CreateLocalGame();
        void                         AddGame(IGameSessionController controller);
        IGameSessionController       GetGame(string uid);
        bool                         IsLocalGame(string uid);
        IEnumerable<GameSessionData> GetOnlineGameSessions();
    }
}