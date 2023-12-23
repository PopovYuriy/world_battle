using System.Linq;
using App.Data.Player;
using App.Enums;
using App.Services;
using App.Signals;
using Core.UI;
using Core.UI.Enums;
using Core.UI.Screens;
using Game.Data;
using UnityEngine;
using Zenject;

namespace UI.GamesManagingScreen
{
    public sealed class GamesManagingScreenController : ScreenControllerAbstract<GamesManagingScreenView>
    {
        [Inject] private GameSessionsManager _gameSessionsManager;
        [Inject] private GameFieldColorsConfig _colorsConfig;
        [Inject] private IPlayer _player;
        [Inject] private SignalBus _signalBus;
        [Inject] private UISystem _uiSystem;
        
        public override void Initialize()
        {
            View.OnGameSelected += GameSelectedHandler;
            View.OnBackClicked += BackClickedHandler;
            View.OnNewGameClicked += NewGameClickHandler;
            
            View.Initialize();

            var localGame = _gameSessionsManager.LocalStorage;
            if (localGame != null)
                View.SetGames(new []{localGame.Data}, _colorsConfig, _player.Uid, true);
            
            
            var games = _gameSessionsManager.GetOnlineGameSessions().ToArray();
            if (games.Length != 0)
                View.SetGames(games, _colorsConfig, _player.Uid, false);

            View.SetNoGamesInfoVisible(localGame == null && games.Length == 0);
        }

        public override void Dispose()
        {
            View.OnGameSelected -= GameSelectedHandler;
            View.OnBackClicked -= BackClickedHandler;
            View.OnNewGameClicked -= NewGameClickHandler;
        }

        public override void Show()
        {
            View.Show();
        }

        public override void Close()
        {
            Object.Destroy(View.gameObject);
            Dispose();
        }

        private void NewGameClickHandler()
        {
            _uiSystem.ShowPopup(PopupId.NewGame);
        }

        private void BackClickedHandler()
        {
            _uiSystem.ShowScreen(ScreenId.MainMenu);
        }
        
        private void GameSelectedHandler(string gameId)
        {
            _signalBus.Fire(new StartExistGameSignal(gameId));
        }
    }
}