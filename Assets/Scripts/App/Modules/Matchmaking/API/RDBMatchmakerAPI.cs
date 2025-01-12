using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Modules.GameSessions.Data;
using App.Modules.Matchmaking.API.Data;
using App.Modules.Matchmaking.Data;
using App.Modules.Matchmaking.Enums;
using App.Services.Database.Observers;
using App.Services.Database.Utils;
using Core.API.Common;
using Firebase.Database;
using Newtonsoft.Json;

namespace App.Modules.Matchmaking.API
{
    internal sealed class RDBMatchmakerAPI : IMatchmakerAPI
    {
        private DatabaseReference _pendingGameReference;

        public async Task<PendingGameData> GetExistPendingGameAsync(string userId)
        {
            var pendingGamesRoot = FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.PendingGames);
            var pendingGameObject = await pendingGamesRoot
                                            .OrderByChild(PendingGameData.HostUidKey)
                                            .EqualTo(userId)
                                            .GetValueAsync();

            if (!pendingGameObject.Exists)
                return null;

            _pendingGameReference = pendingGameObject.Children.First().Reference;

            var pendingGameSnapshot = await _pendingGameReference.GetValueAsync();
            var json = pendingGameSnapshot.GetRawJsonValue();
            var data = JsonConvert.DeserializeObject<PendingGameData>(json);

            return data;
        }

        public async Task<IEnumerable<string>> GetKeywordsInUseAsync()
        {
            var pendingGamesRoot = FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.PendingGames);
            var gamesSnapshot = await pendingGamesRoot.GetValueAsync();

            if (!gamesSnapshot.Exists)
                return null;
            
            var keywords = new List<string>(gamesSnapshot.Children.Count());
            foreach (var childSnapshot in gamesSnapshot.Children)
            {
                if (!childSnapshot.Child(PendingGameData.KeywordKey).Exists)
                    continue;
                
                var fieldValue = childSnapshot.Child(PendingGameData.KeywordKey).GetValue(false).ToString();
                keywords.Add(fieldValue);
            }

            return keywords;
        }

        public async Task<CreateGameResult> CreatePendingGameAsync(string keyword, string userId)
        {
            var pendingGameRoot = FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.PendingGames).Push();
            
            var pendingGameSnapshot = await pendingGameRoot
                                            .OrderByChild(PendingGameData.KeywordKey)
                                            .EqualTo(keyword)
                                            .GetValueAsync();

            if (pendingGameSnapshot.Exists)
                return new CreateGameResult(PendingGameCreateError.AlreadyExists);
            
            _pendingGameReference = pendingGameSnapshot.Reference;
            
            var pendingGameData = new PendingGameData(keyword, userId);
            await pendingGameRoot.SetRawJsonValueAsync(JsonConvert.SerializeObject(pendingGameData));
            return new CreateGameResult(pendingGameData);
        }

        public async Task<IDataObserver<bool>> CreateSessionCreatedObserverAsync(string userId)
        {
            var pendingGamesRoot = FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.PendingGames);
            var pendingGameSnapshot = await pendingGamesRoot
                                      .OrderByChild(PendingGameData.HostUidKey)
                                      .EqualTo(userId)
                                      .LimitToFirst(1)
                                      .GetValueAsync();
            
            if (!pendingGameSnapshot.Exists)
                return null;
            
            var pendingGameNode = pendingGameSnapshot.Children.First().Reference.Child(PendingGameData.SessionCreatedKey);
            
            var observer = new ValueChangeObserver<bool>(pendingGameNode);
            return observer;
        }

        public async Task<JoinGameResult> TryJoinToGameAsync(string keyword, string userUid, float expirationTime)
        {
            var pendingGameRootSnapshot = await FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.PendingGames)
                                                                .OrderByChild(PendingGameData.KeywordKey)
                                                                .EqualTo(keyword)
                                                                .GetValueAsync();

            if (!pendingGameRootSnapshot.Exists)
                return new JoinGameResult(PendingGameJoinError.DoesNotExist);

            var pendingGameSnapshot = pendingGameRootSnapshot.Children.First();
            
            var json = pendingGameSnapshot.GetRawJsonValue();
            var data = JsonConvert.DeserializeObject<PendingGameData>(json);
            
            if (await CheckPendingGameIsExpiredAsync(data, expirationTime))
                return new JoinGameResult(PendingGameJoinError.Expired);

            if (await CheckGameIsExistAsync(data.Id))
                return new JoinGameResult(PendingGameJoinError.IsAlreadyCreated);

            await pendingGameSnapshot.Reference.Child(PendingGameData.OpponentUidKey).SetValueAsync(userUid);
            
            return new JoinGameResult(data);
        }

        public async Task<JoinGameResult> MarkPendingGameSessionCreatedAsync(string gameId)
        {
            var pendingGamesRoot = FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.PendingGames);
            var pendingGameSnapshot = await pendingGamesRoot
                                            .OrderByChild(PendingGameData.IdKey)
                                            .EqualTo(gameId)
                                            .GetValueAsync();
            
            if (!pendingGameSnapshot.Exists)
                return new JoinGameResult(PendingGameJoinError.DoesNotExist);
            
            await pendingGameSnapshot.Children.First().Reference.Child(PendingGameData.SessionCreatedKey).SetValueAsync(true.ToString());
            
            return new JoinGameResult(PendingGameJoinError.None);
        }

        public async Task DeletePendingGameAsync()
        {
            if (_pendingGameReference == null)
                throw new NullReferenceException("Pending game doesn't exist.");

            await _pendingGameReference.RemoveValueAsync();
            _pendingGameReference = null;
        }

        public async Task<bool> CheckPendingGameIsExpiredAsync(PendingGameData data, float expirationTime)
        {
            var serverTime = await GetServerTimeAsync();
            return serverTime - data.CreatedAtTimestamp >= expirationTime;
        }

        private async Task<long> GetServerTimeAsync()
        {
            var timeRef = FirebaseDatabase.DefaultInstance.RootReference.Child("serverTime");
            await timeRef.SetValueAsync(ServerValue.Timestamp);
            var timeSnapshot = await timeRef.GetValueAsync();
            return Convert.ToInt64(timeSnapshot.Value);
        }
        
        private async Task<bool> CheckGameIsExistAsync(string gameId)
        {
            var gamesRoot = FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.Games);
            var snapshot = await gamesRoot
                                 .OrderByChild(GameSessionData.UidKey)
                                 .EqualTo(gameId)
                                 .GetValueAsync();

            return snapshot.Exists;
        }
    }
}