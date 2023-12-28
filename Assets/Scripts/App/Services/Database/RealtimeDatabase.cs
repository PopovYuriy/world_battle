using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Data.Player;
using App.Services.Database.Utils;
using Firebase.Database;
using Game.Data;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace App.Services.Database
{
    public sealed class RealtimeDatabase
    {
        [Inject] private IPlayer _player;
        
        private DatabasePathProvider _pathProvider;
        private DatabaseReference _dataBaseRoot;
        
        public void Initialize(DatabaseReference rootReference)
        {
            _pathProvider = new DatabasePathProvider();
            _dataBaseRoot = rootReference;
        }
        
        public DatabaseReference GetGamesRoot() => _dataBaseRoot.Child(_pathProvider.Games); 

        public async Task<DatabaseReference> GetExistPendingGame()
        {
            var pendingGamesRoot = GetPendingGamesRoot();
            var pendingGameSnapshot = await pendingGamesRoot
                .OrderByChild(PendingGameData.OwnerUidKey)
                .EqualTo(_player.Uid)
                .GetValueAsync();

            if (!pendingGameSnapshot.Exists)
                return null;
            
            return pendingGameSnapshot.Reference;
        }
        
        public async Task<DatabaseReference> CreatePendingGame(string secretWord, string userId)
        {
            var pendingGameRoot = GetPendingGamesRoot().Push();
            var pendingGameData = new PendingGameData(secretWord, userId);
            await pendingGameRoot.SetRawJsonValueAsync(JsonConvert.SerializeObject(pendingGameData));
            return pendingGameRoot;
        }

        public async Task<List<DatabaseReference>> GetExistGames()
        {
            var usersGamesRoot = GetUsersGamesRoot();
            var gamesSnapshot = await usersGamesRoot
                .OrderByKey()
                .EqualTo(_player.Uid)
                .LimitToFirst(1)
                .GetValueAsync();
            
            if (!gamesSnapshot.Exists)
                return null;

            var gameIdsListSnapshots = gamesSnapshot.Children.First().Children.ToArray();
            var result = new List<DatabaseReference>(gameIdsListSnapshots.Length);
            var gamesRoot = GetGamesRoot();
            foreach (var gameSnapshot in gameIdsListSnapshots)
            {
                var gameSessionSnapshot = await gamesRoot
                    .OrderByChild(GameSessionData.UidKey)
                    .EqualTo(gameSnapshot.Key)
                    .LimitToFirst(1)
                    .GetValueAsync();

                if (!gameSessionSnapshot.Exists)
                {
                    await gamesSnapshot.Children.First().Child(gameSnapshot.Key).Reference.RemoveValueAsync();
                    continue;
                }

                result.Add(gameSessionSnapshot.Children.First().Reference);
            }

            return result;
        }

        public async Task<DatabaseReference> CreateNewGame(GameSessionData data)
        {
            var newGameRoot = GetGamesRoot().Push();
            await newGameRoot.SetRawJsonValueAsync(JsonConvert.SerializeObject(data));
            foreach (var playerGameData in data.Players)
            {
                await AddGameToUser(data.Uid, playerGameData.Uid);
            }
            return newGameRoot;
        }

        public async Task<DatabaseReference> FindPendingGame(string secretWord)
        {
            var pendingGamesRoot = GetPendingGamesRoot();
            var pendingGameSnapshot = await pendingGamesRoot
                .OrderByChild(PendingGameData.SecretWordKey)
                .EqualTo(secretWord)
                .GetValueAsync();

            if (!pendingGameSnapshot.Exists)
            {
                Debug.LogWarning($"Game with secret word {secretWord} not found.");
                return null;
            }

            return pendingGameSnapshot.Children.First().Reference;
        }

        public async Task<Player> GetPlayer(string uid)
        {
            var usersRoot = GetUsersRoot();
            var userDataSnapshot = await usersRoot
                .OrderByChild(Player.UidKey)
                .EqualTo(uid)
                .GetValueAsync();

            if (!userDataSnapshot.Exists)
            {
                Debug.LogError($"User with id {uid} not found");
                return null;
            }
            
            var playerDataSnapshot = userDataSnapshot.Children.First();
            var jsonData = playerDataSnapshot.GetRawJsonValue();
            var playerData = JsonConvert.DeserializeObject<Player>(jsonData);
            return playerData;
        }

        public async Task TrySaveUser(IPlayer player)
        {
            var usersRoot = GetUsersRoot();
            var userDataSnapshot = await usersRoot
                .OrderByChild(Player.UidKey)
                .EqualTo(player.Uid)
                .GetValueAsync();

            if (userDataSnapshot.Exists)
            {
                var playerDataSnapshot = userDataSnapshot.Children.First();
                var jsonData = playerDataSnapshot.GetRawJsonValue();
                var existPlayerData = JsonConvert.DeserializeObject<Player>(jsonData);
                if (existPlayerData.Name == player.Name)
                    return;
                
                await playerDataSnapshot.Reference.UpdateChildrenAsync(new Dictionary<string, object>
                {
                    {Player.NameKey, player.Name}
                });
            }
            else
            {
                var userPush = usersRoot.Push();
                await userPush.SetRawJsonValueAsync(JsonConvert.SerializeObject(player));
            }
        }

        private async Task AddGameToUser(string gameId, string playerId)
        {
            var userToGameMapRoot = GetUsersGamesRoot();
            var userNode = userToGameMapRoot.Child(_pathProvider.GeneratePath(playerId, gameId));
            await userNode.SetValueAsync(1);
        }
        
        private DatabaseReference GetPendingGamesRoot() => _dataBaseRoot.Child(_pathProvider.PendingGames);
        private DatabaseReference GetUsersRoot() => _dataBaseRoot.Child(_pathProvider.Users);
        private DatabaseReference GetUsersGamesRoot() => _dataBaseRoot.Child(_pathProvider.UserGames);
    }
}