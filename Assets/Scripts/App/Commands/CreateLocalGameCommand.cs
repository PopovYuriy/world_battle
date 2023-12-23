using App.Services;
using Core.Commands;
using Core.UI;
using Core.UI.Enums;
using Game.Field.Mediators;
using UI.GameScreen.Data;
using Zenject;

namespace App.Commands
{
    public sealed class CreateLocalGameCommand : ICommand
    {
        private GameSessionsManager _gameSessionsManager;
        private UISystem _uiSystem;

        [Inject]
        private void Construct(GameSessionsManager gameSessionsManager, UISystem uiSystem)
        {
            _gameSessionsManager = gameSessionsManager;
            _uiSystem = uiSystem;
        }

        public void Execute()
        {
            var sessionStorage = _gameSessionsManager.GetExistOrCreateLocalGame();
            var localGameSessionMediator = new LocalGameMediator();
            var screenData = new GameScreenData(localGameSessionMediator, sessionStorage);
            _uiSystem.ShowScreen(ScreenId.Game, screenData);
        }
    }
}