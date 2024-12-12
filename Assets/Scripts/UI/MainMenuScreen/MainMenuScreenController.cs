using System;
using System.Linq;
using App.Data.Player;
using App.Enums;
using App.Services;
using App.Signals;
using Core.UI;
using Core.UI.Screens;
using Game.Data;
using UI.MainMenuScreen.Enums;
using Zenject;
using Object = UnityEngine.Object;

namespace UI.MainMenuScreen
{
    public sealed class MainMenuScreenController : ScreenControllerAbstract<MainMenuScreenView>
    {
        private GameSessionsManager   _gameSessionsManager;
        private IPlayer               _player;
        private SignalBus             _signalBus;
        private UISystem              _uiSystem;

        private MainMenuTabId _currentTab;

        [Inject]
        private void Construct(GameSessionsManager gameSessionsManager, IPlayer player, SignalBus signalBus, UISystem uiSystem)
        {
            _gameSessionsManager = gameSessionsManager;
            _player = player;
            _signalBus = signalBus;
            _uiSystem = uiSystem;
        }

        public override void Initialize()
        {
            View.OnGameSelected += GameSelectedHandler;
            View.OnNewGameClicked += NewGameClickHandler;
            View.OnTabClicked += TabClickHandler;

            View.Initialize();
            _currentTab = MainMenuTabId.Online;
            SetCurrentTabGames();
        }

        public override void Dispose()
        {
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
            View.SetGames(_currentTab, games, _player.Uid);
        }

        private GameSessionData[] GetGames(MainMenuTabId tabId)
        {
            var result = tabId switch
            {
                MainMenuTabId.Local  => _gameSessionsManager.LocalStorage == null ? null : new[] { _gameSessionsManager.LocalStorage.Data },
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