using Game.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Services.Storage
{
    public sealed class LocalGameSessionStorage : IGameSessionStorage
    { 
        public GameSessionData Data { get; set; }

        public event SessionStorageEventHandler Updated;
        public event SessionStorageEventHandler Deleted;

        public void Dispose() { }

        public void Save()
        {
            var jsonData = JsonConvert.SerializeObject(Data);
            PlayerPrefs.SetString(Data.Uid, jsonData);
        }

        public void Delete()
        {
            if (!PlayerPrefs.HasKey(Data.Uid))
                return;
            
            PlayerPrefs.DeleteKey(Data.Uid);
            Deleted?.Invoke(this);
        }
    }
}