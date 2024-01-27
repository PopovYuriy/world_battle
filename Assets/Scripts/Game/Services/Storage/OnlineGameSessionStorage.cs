using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Database.Observers;
using Firebase.Database;
using Game.Data;
using Newtonsoft.Json;
using Tools.CSharp;
using UnityEngine;

namespace Game.Services.Storage
{
    public sealed class OnlineGameSessionStorage : IGameSessionStorage
    {
        private DatabaseReference _databaseReference;
        private IDataObserver<string> _playerTurnObserver;
        private IDataObserver<SurrenderData> _giveUpDataObserver;
        private IDataObserver<SurrenderData> _giveUpDataRemovedObserver;
        private IDataObserver<WinData> _winDataObserver;
        private string _ownPlayerId;
        
        public GameSessionData Data { get; private set; }
        
        public event SessionStorageEventHandler OnTurn;
        public event SessionStorageEventHandler OnWin;
        public event SessionStorageEventHandler OnDeleted;
        public event SessionStorageEventHandler OnSurrenderDataUpdated;

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

            _giveUpDataObserver = new ValueChangeObserver<SurrenderData>(_databaseReference
                .Child(GameSessionData.GiveUpDataKey).Reference);
            _giveUpDataObserver.OnChangeOccured += GiveUpDataUpdatedHandler;
            _giveUpDataObserver.Observe();

            _giveUpDataRemovedObserver = new ValueRemovedObserver<SurrenderData>(_databaseReference
                .Child(GameSessionData.GiveUpDataKey).Reference);
            _giveUpDataRemovedObserver.OnChangeOccured += GiveUpDataRemovedHandler;
            _giveUpDataRemovedObserver.Observe();
            
            _winDataObserver = new ValueChangeObserver<WinData>(_databaseReference
                .Child(GameSessionData.WinDataKey).Reference);
            _winDataObserver.OnChangeOccured += WinDataUpdatedHandler;
            _winDataObserver.Observe();
        }

        private void ReferenceOnChildRemoved(object sender, ChildChangedEventArgs e)
        {
            Debug.Log("Deleted");
        }

        public void Dispose()
        {
            _playerTurnObserver.Dispose();
            _playerTurnObserver.OnChangeOccured -= PlayerMadeTurnHandler;
            _playerTurnObserver = null;
            
            _giveUpDataObserver.Dispose();
            _giveUpDataObserver.OnChangeOccured -= GiveUpDataUpdatedHandler;
            _giveUpDataObserver = null;
            
            _giveUpDataRemovedObserver.Dispose();
            _giveUpDataRemovedObserver.OnChangeOccured -= GiveUpDataRemovedHandler;
            _giveUpDataRemovedObserver = null;
            
            _winDataObserver.Dispose();
            _winDataObserver.OnChangeOccured -= WinDataUpdatedHandler;
            _winDataObserver = null;
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
            OnDeleted?.Invoke(this);
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
            Data.WinData = data.WinData;
            Data.SurrenderData = data.SurrenderData;
            Data.AbilityData = data.AbilityData;
            MergePlayersData(Data.Players, data.Players);
            OnTurn?.Invoke(this);
        }
        
        private void GiveUpDataUpdatedHandler(DatabaseReference dataReference, SurrenderData data)
        {
            Data.SurrenderData = data;
            OnSurrenderDataUpdated?.Invoke(this);
        }
        
        private void GiveUpDataRemovedHandler(DatabaseReference dataReference, SurrenderData data)
        {
            Data.SurrenderData = null;
            OnSurrenderDataUpdated?.Invoke(this);
        }

        private void WinDataUpdatedHandler(DatabaseReference dataReference, WinData data)
        {
            Data.WinData = data;
            OnWin?.Invoke(this);
        }

        private void MergePlayersData(PlayerGameData[] origin, PlayerGameData[] source)
        {
            foreach (var originPlayerData in origin)
            {
                var sourcePlayerData = source.First(p => p.Uid == originPlayerData.Uid);
                originPlayerData.Points = sourcePlayerData.Points;
                originPlayerData.AbilitiesCosts = sourcePlayerData.AbilitiesCosts;
            }
        }
    }
}