using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data.Player;
using App.Modules.GameSessions.API;
using App.Modules.GameSessions.API.Enums;
using App.Modules.GameSessions.Controller;
using App.Modules.GameSessions.Data;
using App.Services.Database;
using Game.Abilities;
using Game.Grid;
using Game.Grid.Cells.Model;
using Game.Services;
using Newtonsoft.Json;
using Tools.CSharp;
using UnityEngine;
using Zenject;

namespace App.Modules.GameSessions
{
    public sealed class GameSessionsManager : IGameSessionsManager, IGameSessionsManagerInitializable
    {
        private const string LocalSessionStorageKey = "local_session";
        private const int    BasePointsNumber       = 5;

        [Inject] private IGameSessionsAPI      _api;
        [Inject] private IPlayer               _player;
        [Inject] private AbilityConfigsStorage _abilitiesConfig;
        [Inject] private SignalBus             _signalBus;

        private List<IGameSessionController> _onlineStorages;
        private RealtimeDatabase             _database;
        public  IGameSessionController       LocalController { get; private set; }

        public void Initialize(IEnumerable<IGameSessionController> onlineControllers)
        {
            Debug.Log("GameSessionsManager :: Initialize");
            TryInitializeLocalGameSessionStorage();

            _onlineStorages = new List<IGameSessionController>();

            if (onlineControllers == null)
                return;

            foreach (var controller in onlineControllers)
                InitializeSessionController(controller);
        }

        public async Task<IGameSessionController> CreateOnlineGame(string sessionId, string opponentId)
        {
            Debug.Log("GameSessionsManager :: Creating online game...");
            
            var user = await _api.GetUserAsync(opponentId);
            if (user.Error != UserError.None)
            {
                Debug.LogError($"Create game error: {user.Error}");
                return null;
            }

            var playersData = new PlayerGameData[]
            {
                new(user.User.Uid, user.User.Name, _abilitiesConfig.GetDefaultCosts(), false),
                new(_player.Uid, _player.Name, _abilitiesConfig.GetDefaultCosts(), true)
            };

            var gameSessionData = CreateSessionData(sessionId, new Vector2Int(5, 5), playersData);

            var controller = await _api.CreateGameSessionAsync(gameSessionData, _player.Uid);
            return controller;
        }

        public async Task<bool> TryLoadAndAddGameAsync(string sessionId, string playerId)
        {
            var controller = await _api.LoadGamesAsync(playerId, sessionId);

            if (controller == null)
                return false;
            
            AddGame(controller);
            return true;
        }

        public void AddGame(IGameSessionController controller)
        {
            InitializeSessionController(controller);
            _signalBus.Fire(new GameSessionsSignal.GameCreatedSignal(controller.Type, controller.Data));
        }

        public void CreateLocalGame()
        {
            if (LocalController != null)
                return;
            
            LocalController = new LocalIGameSessionController();
            var guestUser = App.Data.Player.Player.CreateGuestUser();
            var players = new PlayerGameData[]
            {
                new(_player.Uid, _player.Name, _abilitiesConfig.GetDefaultCosts(), true),
                new(guestUser.Uid, guestUser.Name, _abilitiesConfig.GetDefaultCosts(), true)
            };
            ((LocalIGameSessionController)LocalController).Data = CreateSessionData(LocalSessionStorageKey, new Vector2Int(5, 5), players);

            LocalController.Save();
            LocalController.OnDeleted += ControllerDeleteRequestedHandler;
        }
        
        public IGameSessionController GetGame(string uid)
        {
            return uid == LocalSessionStorageKey ? LocalController : _onlineStorages.First(s => s.Data.Uid == uid);
        }

        public bool IsLocalGame(string uid) => LocalController?.Data.Uid == uid;
        
        public IEnumerable<GameSessionData> GetOnlineGameSessions() => _onlineStorages.Select(s => s.Data);

        private void InitializeSessionController(IGameSessionController controller)
        {
            UpdateOnlinePlayersIsControllable(controller.Data.Players);
            controller.OnDeleted += ControllerDeleteRequestedHandler;
            _onlineStorages.Add(controller);
        }

        private void ControllerDeleteRequestedHandler(IGameSessionController sender)
        {
            DeleteGameForUserAsync(sender).Run();
        }

        private async Task DeleteGameForUserAsync(IGameSessionController gameSessionController)
        {
            gameSessionController.OnDeleted -= ControllerDeleteRequestedHandler;
            if (gameSessionController == LocalController)
            {
                LocalController = null;
                _signalBus.Fire(new GameSessionsSignal.GameDeletedSignal(GameSessionType.Local, gameSessionController.Data));
            }
            else
            {
                _onlineStorages.Remove(gameSessionController);
                await _api.DeleteGameFromListAsync(_player.Uid, gameSessionController.Data.Uid);
                _signalBus.Fire(new GameSessionsSignal.GameDeletedSignal(GameSessionType.Online, gameSessionController.Data));
            }
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
                    var model = new CellModel(letterIndex, letters[letterIndex++], points, playerId);
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
            foreach (var player in data.Players)
                player.IsControllable = true;

            LocalController = new LocalIGameSessionController { Data = data };

            LocalController.OnDeleted += ControllerDeleteRequestedHandler;
        }

        private void UpdateOnlinePlayersIsControllable(PlayerGameData[] playerGamesData)
        {
            foreach (var playerGameData in playerGamesData)
                playerGameData.IsControllable = playerGameData.Uid == _player.Uid;
        }
    }
}