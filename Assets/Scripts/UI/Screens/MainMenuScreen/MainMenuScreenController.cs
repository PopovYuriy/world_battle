using System;
using System.Linq;
using App.Data.Player;
using App.Enums;
using App.Modules.GameSessions;
using App.Modules.GameSessions.Data;
using App.Signals;
using Core.UI;
using Core.UI.Screens;
using UI.Screens.MainMenuScreen.Enums;
using Zenject;
using Object = UnityEngine.Object;

namespace UI.Screens.MainMenuScreen
{
    public sealed class MainMenuScreenController : ScreenControllerAbstract<MainMenuScreenView>
    {
        [Inject] private IGameSessionsManager   _gameSessionsManager;
        [Inject] private IPlayer               _player;
        [Inject] private SignalBus             _signalBus;
        [Inject] private UISystem              _uiSystem;

        private MainMenuTabId _currentTab;

        public override void Initialize()
        {
            _signalBus.Subscribe<GameSessionsSignal.GameCreatedSignal>(SetCurrentTabGames);
            _signalBus.Subscribe<GameSessionsSignal.GameDeletedSignal>(SetCurrentTabGames);
            
            View.OnGameSelected += GameSelectedHandler;
            View.OnNewGameClicked += NewGameClickHandler;
            View.OnTabClicked += TabClickHandler;

            View.Initialize();
            _currentTab = MainMenuTabId.Online;
            View.SetInitialTabId(_currentTab);
            SetCurrentTabGames();
        }

        public override void Dispose()
        {
            _signalBus.Unsubscribe<GameSessionsSignal.GameCreatedSignal>(SetCurrentTabGames);
            _signalBus.Unsubscribe<GameSessionsSignal.GameDeletedSignal>(SetCurrentTabGames);
            
            View.OnGameSelected -= GameSelectedHandler;
            View.OnNewGameClicked -= NewGameClickHandler;
            View.OnTabClicked -= TabClickHandler;
        }

        public override void Show()
        {
            View.Show();
        }

        public override void Close()
        {
            View.Hide();
            Object.Destroy(View.gameObject);
            Dispose();
        }

        private void SetCurrentTabGames()
        {
            var games = GetGames(_currentTab);
            View.SetGames(games, _player.Uid);
        }

        private GameSessionData[] GetGames(MainMenuTabId tabId)
        {
            var result = tabId switch
            {
                MainMenuTabId.Local  => _gameSessionsManager.LocalController == null ? null : new[] { _gameSessionsManager.LocalController.Data },
                MainMenuTabId.Online => _gameSessionsManager.GetOnlineGameSessions().ToArray(),
                _                    => throw new ArgumentOutOfRangeException(nameof(tabId), tabId, null)
            };
            return result;
        }

        private void GameSelectedHandler(string gameId)
        {
            _signalBus.Fire(new StartExistGameSignal(gameId));
        }

        private void NewGameClickHandler()
        {
            _uiSystem.ShowPopup(PopupId.NewGame);
        }

        private void TabClickHandler(MainMenuTabId tabId)
        {
            if (_currentTab == tabId)
                return;

            _currentTab = tabId;
            SetCurrentTabGames();
        }
    }
}