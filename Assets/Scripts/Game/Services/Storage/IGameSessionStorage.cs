using System;
using Game.Data;

namespace Game.Services.Storage
{
    public delegate void SessionStorageEventHandler(IGameSessionStorage sender);
    
    public interface IGameSessionStorage : IDisposable
    {
        GameSessionData Data { get; }
        
        event SessionStorageEventHandler Updated;
        event SessionStorageEventHandler Deleted;

        void Save();
        void Delete();
    }
}