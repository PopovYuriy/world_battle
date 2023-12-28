using System.Collections.Generic;
using System.Threading.Tasks;
using App.Services.Database.Observers;
using Firebase.Database;
using Game.Data;
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
            Data.Turns ??= new List<string>();
            
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
            DeleteAsync().Run();
        }

        private async Task SaveAsync()
        {
            await _databaseReference.SetRawJsonValueAsync(JsonConvert.SerializeObject(Data));
        }

        private async Task DeleteAsync()
        {
            await _databaseReference.RemoveValueAsync();
            Deleted?.Invoke(this);
        }

        private void PlayerMadeTurnHandler(DatabaseReference dataReference, string playerId)
        {
            if (playerId == _ownPlayerId)
                return;
            
            UpdateDataAsync().Run();
        }

        private async Task UpdateDataAsync()
        { 
            var dataSnapshot = await _databaseReference.GetValueAsync();
            var json = dataSnapshot.GetRawJsonValue();
            var data =  JsonConvert.DeserializeObject<GameSessionData>(json);
            
            Data.Turns = data.Turns;
            Data.Grid = data.Grid;
            Data.LastTurnPlayerId = data.LastTurnPlayerId;
            Data.WinnerPlayerId = data.WinnerPlayerId;
            Updated?.Invoke(this);
        }
    }
}