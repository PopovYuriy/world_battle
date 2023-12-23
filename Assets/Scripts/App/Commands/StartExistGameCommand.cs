using App.Services;
using App.Signals;
using Core.Commands;
using Core.UI;
using Core.UI.Enums;
using Game.Field.Mediators;
using UI.GameScreen.Data;
using Zenject;

namespace App.Commands
{
    public sealed class StartExistGameCommand : ICommand
    {
        private StartExistGameSignal _signal;
        private GameSessionsManager _gameSessionsManager;
        private UISystem _uiSystem;

        [Inject]
        private void Construct(StartExistGameSignal signal, GameSessionsManager gameSessionsManager, UISystem uiSystem)
        {
            _signal = signal;
            _gameSessionsManager = gameSessionsManager;
            _uiSystem = uiSystem;
        }

        public void Execute()
        {
            var gameSessionStorage = _gameSessionsManager.GetGame(_signal.GameUid);
            IGameMediator gameSessionMediator = _gameSessionsManager.IsLocalGame(_signal.GameUid) 
                ? new LocalGameMediator()
                : new OnlineGameMediator();
            var screenData = new GameScreenData(gameSessionMediator, gameSessionStorage);
            _uiSystem.ShowScreen(ScreenId.Game, screenData);
        }
    }
}