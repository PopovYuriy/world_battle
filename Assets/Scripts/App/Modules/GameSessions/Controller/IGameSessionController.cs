using System;
using System.Linq;
using App.Modules.GameSessions.API.Enums;
using App.Modules.GameSessions.Data;

namespace App.Modules.GameSessions.Controller
{
    public delegate void SessionStorageEventHandler(IGameSessionController sender);
    
    public interface IGameSessionController : IDisposable
    {
        GameSessionType Type { get; }
        GameSessionData Data { get; }
        
        event SessionStorageEventHandler OnTurn;
        event SessionStorageEventHandler OnWin;
        event SessionStorageEventHandler OnDeleted;
        event SessionStorageEventHandler OnSurrenderDataUpdated;

        void Save();
        void Delete();

        public PlayerGameData GetPlayerData(string playerId) => Data.Players.First(p => p.Uid == playerId);
    }
}