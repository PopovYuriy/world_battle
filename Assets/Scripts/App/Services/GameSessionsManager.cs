using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data.Player;
using App.Services.Database;
using App.Services.Database.Controllers;
using Firebase.Database;
using Game.Data;
using Game.Grid;
using Game.Grid.Cell.Model;
using Game.Services;
using Game.Services.Storage;
using Newtonsoft.Json;
using Tools.CSharp;
using UnityEngine;
using Zenject;

namespace App.Services
{
    public sealed class GameSessionsManager
    {
        private const string LocalSessionStorageKey = "local_session";
        private const int BasePointsNumber = 5;
        
        private RealtimeDatabase _database;
        private IPlayer _player;

        private List<IGameSessionStorage> _storages;
        private PendingGameController _pendingGameController;

        public IGameSessionStorage LocalStorage { get; private set; }

        public event Action<IGameSessionStorage> OnPendingGameConnected;

        [Inject]
        private void Construct(RealtimeDatabase database, IPlayer player)
        {
            _database = database;
            _player = player;
        }

        public async Task InitializeAsync()
        {
            _storages = new List<IGameSessionStorage>();
            TryInitializeLocalGameSessionStorage();
            _pendingGameController = new PendingGameController();
            await _pendingGameController.Initialize(_database);

            await LoadExistGames();
            
            _pendingGameController.OnPlayerConnected += PlayerConnectedHandler;
            _pendingGameController.OnGameFound += GameFoundHandler;
        }

        public IGameSessionStorage GetExistOrCreateLocalGame()
        {
            if (LocalStorage != null) 
                return LocalStorage;
            
            LocalStorage = new LocalGameSessionStorage();
            var guestUser = Player.CreateGuestUser();
            var players = new PlayerGameData[]
            {
                new (_player.Uid, _player.Name),
                new (guestUser.Uid, guestUser.Name)
            };
            ((LocalGameSessionStorage) LocalStorage).Data = 
                CreateSessionData(LocalSessionStorageKey, new Vector2Int(5, 5), players);

            LocalStorage.Save();

            return LocalStorage;
        }

        public IGameSessionStorage GetGame(string uid)
        {
            return uid == LocalSessionStorageKey ? LocalStorage : _storages.First(s => s.Data.Uid == uid);
        }

        public bool IsLocalGame(string uid) => LocalStorage?.Data.Uid == uid;

        public async Task CreatePendingGame(string secretWord)
        {
            await _pendingGameController.CreatePendingGame(secretWord, _player.Uid);
        }

        public async Task FindPendingGame(string secretWord)
        {
            await _pendingGameController.FindPendingGame(secretWord, _player.Uid);
        }

        public IEnumerable<GameSessionData> GetOnlineGameSessions() => _storages.Select(s => s.Data);

        private void PlayerConnectedHandler(string sessionId, string userId)
        {
            ProcessPlayerConnectedAsync(sessionId, userId).Run();
        }

        private async Task ProcessPlayerConnectedAsync(string sessionId, string userId)
        {
            var secondPlayer = await _database.GetPlayer(userId);
            var playersData = new PlayerGameData[]
            {
                new (_player.Uid, _player.Name),
                new (secondPlayer.Uid, secondPlayer.Name)
            };
            
            var gameSessionData = CreateSessionData(sessionId, new Vector2Int(5, 5), playersData);
            var gameDatabaseReference = await _database.CreateNewGame(gameSessionData);
            
            var gameSessionStorage = await CreateOnlineSessionStorage(gameDatabaseReference);
            OnPendingGameConnected?.Invoke(gameSessionStorage);
        }
        
        private void GameFoundHandler(DatabaseReference gameDatabaseReference)
        {
            ProcessGameFoundAsync(gameDatabaseReference).Run();
        }

        private async Task ProcessGameFoundAsync(DatabaseReference gameDatabaseReference)
        {
            var gameSessionStorage = await CreateOnlineSessionStorage(gameDatabaseReference);
            OnPendingGameConnected?.Invoke(gameSessionStorage);
        }

        private async Task<IGameSessionStorage> CreateOnlineSessionStorage(DatabaseReference gameDatabaseReference)
        {
            var gameSessionStorage = new OnlineGameSessionStorage();
            await gameSessionStorage.InitializeDataAsync(gameDatabaseReference, _player.Uid);
            
            _storages.Add(gameSessionStorage);
            return gameSessionStorage;
        }
        
        private GameSessionData CreateSessionData(string uid, Vector2Int gameFieldSize, PlayerGameData[] playersData)
        {
            var letters = LettersGenerator.Generate(gameFieldSize.x * gameFieldSize.y);
            var gridModel = CreateGridModel(letters, gameFieldSize, playersData);
            return new GameSessionData(uid, playersData, gridModel, new List<string>());
        }
        
        private GridModel CreateGridModel(IReadOnlyList<char> letters, Vector2Int size, PlayerGameData[] players)
        {
            var cellModels = new List<List<CellModel>>(size.x);
            for (var x = 0; x < size.x; x++)
            {
                cellModels.Add(new List<CellModel>(size.y));
                
                for (var y = 0; y < size.y; y++)
                    cellModels[x].Add(null);
            }

            var letterIndex = 0;
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var points = x == size.x - 1 || x == 0 ? BasePointsNumber : 0;
                    var playerId = x == size.x - 1 ? players[0].Uid : x == 0 ? players[1].Uid : string.Empty;
                    var model = new CellModel(letters[letterIndex++], points, playerId);
                    cellModels[x][y] = model;
                }
            }

            return new GridModel(cellModels);
        }

        private void TryInitializeLocalGameSessionStorage()
        {
            if (!PlayerPrefs.HasKey(LocalSessionStorageKey))
                return;

            var jsonData = PlayerPrefs.GetString(LocalSessionStorageKey);

            var data = JsonConvert.DeserializeObject<GameSessionData>(jsonData);
            data.Turns ??= new List<string>();

            LocalStorage = new LocalGameSessionStorage { Data = data };
        }

        private async Task LoadExistGames()
        {
            var games = await _database.GetExistGames();

            if (games == null)
                return;

            foreach (var gameSessionData in games)
            {
                var storage = new OnlineGameSessionStorage();
                await storage.InitializeDataAsync(gameSessionData, _player.Uid);
                
                _storages.Add(storage);
            }
        }
    }
}