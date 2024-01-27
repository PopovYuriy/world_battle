using System.Threading;
using System.Threading.Tasks;
using App.Enums;
using App.Services;
using App.Signals;
using Core.Commands;
using Core.Services.Scene;
using Core.UI;
using Cysharp.Threading.Tasks;
using Game.Field.Mediators;
using UI.GameScreen.Data;
using Zenject;

namespace App.Commands
{
    public sealed class StartExistGameCommand : ICommandAsync
    {
        private StartExistGameSignal _signal;
        private GameSessionsManager _gameSessionsManager;
        private UISystem _uiSystem;
        private ScenesLoader _scenesLoader;

        [Inject]
        private void Construct(StartExistGameSignal signal, GameSessionsManager gameSessionsManager, UISystem uiSystem,
            ScenesLoader scenesLoader)
        {
            _signal = signal;
            _gameSessionsManager = gameSessionsManager;
            _uiSystem = uiSystem;
            _scenesLoader = scenesLoader;
        }

        public async Task Execute()
        {
            var cancellationToken = new CancellationTokenSource();
            await _scenesLoader.LoadTransitionSceneWithCancellationToken(cancellationToken);
            
            var gameSessionStorage = _gameSessionsManager.GetGame(_signal.GameUid);
            IGamePlayController gameSessionPlayController = _gameSessionsManager.IsLocalGame(_signal.GameUid) 
                ? new LocalGamePlayController()
                : new OnlineGamePlayController();
            
            var screenData = new GameScreenData(gameSessionPlayController, gameSessionStorage);
            _uiSystem.ShowScreen(ScreenId.Game, screenData);
            
            cancellationToken.Cancel();
        }
    }
}