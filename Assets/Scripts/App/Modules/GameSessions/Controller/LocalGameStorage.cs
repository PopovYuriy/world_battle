using App.Modules.GameSessions.API.Enums;
using App.Modules.GameSessions.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace App.Modules.GameSessions.Controller
{
    public sealed class LocalIGameSessionController : IGameSessionController
    {
        public GameSessionType Type => GameSessionType.Local;
        public GameSessionData Data { get; set; }

        public event SessionStorageEventHandler OnTurn;
        public event SessionStorageEventHandler OnWin;
        public event SessionStorageEventHandler OnDeleted;
        public event SessionStorageEventHandler OnSurrenderDataUpdated;

        public void Dispose() { }

        public void Save()
        {
            var jsonData = JsonConvert.SerializeObject(Data);
            PlayerPrefs.SetString(Data.Uid, jsonData);
            OnTurn?.Invoke(this);
        }

        public void Delete()
        {
            if (!PlayerPrefs.HasKey(Data.Uid))
                return;
            
            PlayerPrefs.DeleteKey(Data.Uid);
            OnDeleted?.Invoke(this);
        }
    }
}