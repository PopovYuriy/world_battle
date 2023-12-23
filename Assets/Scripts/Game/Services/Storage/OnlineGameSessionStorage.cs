using System.Collections.Generic;
using System.Threading.Tasks;
using App.Services.Database.Observers;
using Firebase.Database;
using Game.Data;
using Game.Grid;
using Newtonsoft.Json;
using Tools.CSharp;

namespace Game.Services.Storage
{
    public sealed class OnlineGameSessionStorage : IGameSessionStorage
    {
        private DatabaseReference _databaseReference;
        private IDataObserver<string> _playerTurnObserver;
        private string _ownPlayerId;
        
        public GameSessionData Data { get; private set; }
        
        public event SessionStorageEventHandler Updated;
        public event SessionStorageEventHandler Deleted;
        
        public async Task InitializeDataAsync(DatabaseReference databaseReference, string ownPlayerId)
        {
            _databaseReference = databaseReference;
            _ownPlayerId = ownPlayerId;
            
            var dataSnapShot = await databaseReference.GetValueAsync();
            Data = JsonConvert.DeserializeObject<GameSessionData>(dataSnapShot.GetRawJsonValue());
            
            _playerTurnObserver = new ValueChangeObserver<string>(_databaseReference
                .Child(GameSessionData.LastTurnPlayerIdKey).Reference);
            _playerTurnObserver.OnChangeOccured += PlayerMadeTurnHandler;
            _playerTurnObserver.Observe();
        }

        public void Dispose()
        {
            _playerTurnObserver.Dispose();
            _playerTurnObserver.OnChangeOccured -= PlayerMadeTurnHandler;
            _playerTurnObserver = null;
        }

        public void Save()
        {
            SaveAsync().Run();
        }

        public void Delete()
        {
            Dispose();
            Deleted?.Invoke(this);
        }

        private async Task SaveAsync()
        {
            await _databaseReference.Child(GameSessionData.TurnsKey).SetRawJsonValueAsync(JsonConvert.SerializeObject(Data.Turns));
            await _databaseReference.Child(GameSessionData.GridKey).SetRawJsonValueAsync(JsonConvert.SerializeObject(Data.Grid));
            await _databaseReference.Child(GameSessionData.LastTurnPlayerIdKey).SetValueAsync(Data.LastTurnPlayerId);
        }

        private void PlayerMadeTurnHandler(DatabaseReference dataReference, string playerId)
        {
            if (playerId == _ownPlayerId)
                return;
            
            UpdateDataAsync().Run();
        }

        private async Task UpdateDataAsync()
        {
            Data.Turns = await LoadData<List<string>>(GameSessionData.TurnsKey);
            Data.Grid = await LoadData<GridModel>(GameSessionData.GridKey);
            Data.LastTurnPlayerId = await LoadData<string>(GameSessionData.LastTurnPlayerIdKey);
            Updated?.Invoke(this);
        }

        private async Task<T> LoadData<T>(string key)
        {
            var dataReference = _databaseReference.Child(key);
            var dataSnapshot = await dataReference.GetValueAsync();
            var json = dataSnapshot.GetRawJsonValue();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}