using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data;
using App.Services.Database.Observers;
using Firebase.Database;
using Game.Data;
using Tools.CSharp;
using UnityEngine;
using Utils.Extensions;

namespace App.Services.Database.Controllers
{
    public delegate void UserConnectedHandler(string sessionId, string userId);
    public delegate void GameFoundHandler(DatabaseReference gameReference);
    
    public sealed class PendingGameController : IDisposable
    {
        private RealtimeDatabase _database;
        private DatabaseReference _pendingGameReference;
        private IDataObserver<string> _userConnectedObserver;
        private IDataObserver<GameSessionData> _createdGameObserver;

        public event UserConnectedHandler OnPlayerConnected;
        public event GameFoundHandler OnGameFound; 

        public async Task Initialize(RealtimeDatabase realtimeDatabase)
        {
            _database = realtimeDatabase;
            _pendingGameReference = await realtimeDatabase.GetExistPendingGame();
            
            if (_pendingGameReference == null)
                return;

            CreateUserConnectedObserver();
        }

        public void Dispose()
        {
            DisposeGameCreatedObserver();
            DisposeUserConnectedObserver();
        }

        public async Task CreatePendingGame(string secretWord, string playerId)
        {
            _pendingGameReference = await _database.CreatePendingGame(secretWord, playerId);
            CreateUserConnectedObserver();
        }

        public async Task FindPendingGame(string secretWord, string playerId)
        {
            _pendingGameReference = await _database.FindPendingGame(secretWord);
            
            if (_pendingGameReference == null)
                return;

            var gamesRootReference = _database.GetGamesRoot();
            _createdGameObserver = new ValueAddedObserver<GameSessionData>(gamesRootReference
                .OrderByChild(GameSessionData.UidKey)
                .EqualTo(_pendingGameReference.Key));
            _createdGameObserver.Observe();
            
            _createdGameObserver.OnChangeOccured += GameCreatedHandler;
            
            await _pendingGameReference.UpdateChildrenAsync(new Dictionary<string, object>
            {
                {PendingGameData.SecondUidKey, playerId}
            });
        }

        private void GameCreatedHandler(DatabaseReference gameDatabaseReference, GameSessionData data)
        {
            DisposeGameCreatedObserver();
            OnGameFound?.Invoke(gameDatabaseReference);
        }

        private void CreateUserConnectedObserver()
        {
            var secondUserDataReference = _pendingGameReference.Reference.Child(PendingGameData.SecondUidKey);
            _userConnectedObserver = new ValueChangeObserver<string>(secondUserDataReference);
            _userConnectedObserver.OnChangeOccured += UserConnectedHandler;
            _userConnectedObserver.Observe();
        }

        private void UserConnectedHandler(DatabaseReference daraReference, string userId)
        {
            ProcessPlayerConnectAsync(userId).Run();
        }

        private async Task ProcessPlayerConnectAsync(string userId)
        {
            if (userId.IsNullOrEmpty())
                Debug.LogError($"Invalid second user id ({userId})");
            else
                OnPlayerConnected?.Invoke(_pendingGameReference.Key, userId);
            
            await DeletePendingGame();
        }

        private async Task DeletePendingGame()
        {
            if (_pendingGameReference == null)
                return;
            
            await _pendingGameReference.RemoveValueAsync();
            _pendingGameReference = null;
        }

        private void DisposeGameCreatedObserver()
        {
            if (_createdGameObserver == null) 
                return;
            
            _createdGameObserver.OnChangeOccured -= GameCreatedHandler;
            _createdGameObserver.Dispose();
            _createdGameObserver = null;
        }

        private void DisposeUserConnectedObserver()
        {
            if (_userConnectedObserver == null) 
                return;
            
            _userConnectedObserver.OnChangeOccured -= UserConnectedHandler;
            _userConnectedObserver.Dispose();
            _userConnectedObserver = null;
        }
    }
}