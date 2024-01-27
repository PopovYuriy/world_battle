using System;
using System.Linq;
using Game.Data;

namespace Game.Services.Storage
{
    public delegate void SessionStorageEventHandler(IGameSessionStorage sender);
    
    public interface IGameSessionStorage : IDisposable
    {
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