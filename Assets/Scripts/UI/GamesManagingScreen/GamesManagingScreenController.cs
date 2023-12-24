using System.Linq;
using App.Data.Player;
using App.Enums;
using App.Services;
using App.Signals;
using Core.UI;
using Core.UI.Screens;
using Game.Data;
using Zenject;
using Object = UnityEngine.Object;

namespace UI.GamesManagingScreen
{
    public sealed class GamesManagingScreenController : ScreenControllerAbstract<GamesManagingScreenView>
    {
        private GameSessionsManager _gameSessionsManager;
        private GameFieldColorsConfig _colorsConfig;
        private IPlayer _player;
        private SignalBus _signalBus;
        private UISystem _uiSystem;

        [Inject]
        private void Construct(GameSessionsManager gameSessionsManager, GameFieldColorsConfig colorsConfig, 
            IPlayer player, SignalBus signalBus, UISystem uiSystem)
        {
            _gameSessionsManager = gameSessionsManager;
            _colorsConfig = colorsConfig;
            _player = player;
            _signalBus = signalBus;
            _uiSystem = uiSystem;
        }

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