using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data.Player;
using App.Modules.GameSessions.API.Data;
using App.Modules.GameSessions.API.Enums;
using App.Modules.GameSessions.Controller;
using App.Modules.GameSessions.Data;
using App.Services.Database.Utils;
using Firebase.Database;
using Newtonsoft.Json;

namespace App.Modules.GameSessions.API
{
    public sealed class RDBGameSessionsAPI : IGameSessionsAPI
    {
        public async Task<IEnumerable<IGameSessionController>> LoadExistGamesAsync(string userId)
        {
            var root = FirebaseDatabase.DefaultInstance.RootReference;
            var usersGamesRoot = root.Child(DatabasePathProvider.UserGames);
            var gamesSnapshot = await usersGamesRoot
                                      .OrderByKey()
                                      .EqualTo(userId)
                                      .LimitToFirst(1)
                                      .GetValueAsync();
            
            if (!gamesSnapshot.Exists)
                return null;

            var gameIdsListSnapshots = gamesSnapshot.Children.First().Children.ToArray();
            var result = new List<IGameSessionController>(gameIdsListSnapshots.Length);
            
            foreach (var gameSnapshot in gameIdsListSnapshots)
            {
                var controller = await LoadGamesAsync(userId, gameSnapshot.Key);

                if (controller == null)
                {
                    await gamesSnapshot.Children.First().Child(gameSnapshot.Key).Reference.RemoveValueAsync();
                    continue;
                }
                
                result.Add(controller);
            }

            return result;
        }

        public async Task<IGameSessionController> LoadGamesAsync(string userId, string gameSessionId)
        {
            var gameRootSnapshot = await FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.Games)
                                                     .OrderByChild(GameSessionData.UidKey)
                                                     .EqualTo(gameSessionId)
                                                     .LimitToFirst(1)
                                                     .GetValueAsync();
            
            if (!gameRootSnapshot.Exists)
                return null;

            var gameSnapshot = gameRootSnapshot.Children.First();
            var json = gameSnapshot.GetRawJsonValue();
            var gameSessionData = JsonConvert.DeserializeObject<GameSessionData>(json);
            
            var controller =  new OnlineGameSessionController(gameSnapshot.Reference, userId, gameSessionData);
            return controller;
        }

        public async Task<UserResult> GetUserAsync(string userId)
        {
            var usersRoot = FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.Users);
            var userDataSnapshot = await usersRoot
                                         .OrderByChild(Player.UidKey)
                                         .EqualTo(userId)
                                         .GetValueAsync();

            if (!userDataSnapshot.Exists)
                return new UserResult(UserError.UserNotFound);
            
            var playerDataSnapshot = userDataSnapshot.Children.First();
            var jsonData = playerDataSnapshot.GetRawJsonValue();
            var user = JsonConvert.DeserializeObject<Player>(jsonData);
            return new UserResult(user);
        }

        public async Task<IGameSessionController> CreateGameSessionAsync(GameSessionData data, string hostId)
        {
            var newGameRoot = FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.Games).Push();
            await newGameRoot.SetRawJsonValueAsync(JsonConvert.SerializeObject(data));
            
            var userToGameMapRoot = FirebaseDatabase.DefaultInstance.RootReference.Child(DatabasePathProvider.UserGames);
            foreach (var playerGameData in data.Players)
            {
                var userNode = userToGameMapRoot.Child(DatabasePathProvider.GeneratePath(playerGameData.Uid, data.Uid));
                await userNode.SetValueAsync(1);
            }

            var controller = new OnlineGameSessionController(newGameRoot, hostId, data);
            return controller;
        }

        public async Task DeleteGameFromListAsync(string userId, string gameSessionId)
        {
            
        }
    }
}