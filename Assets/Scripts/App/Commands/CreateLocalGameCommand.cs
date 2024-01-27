using System.Threading;
using System.Threading.Tasks;
using App.Enums;
using App.Services;
using Core.Commands;
using Core.Services.Scene;
using Core.UI;
using Game.Field.Mediators;
using UI.GameScreen.Data;
using Zenject;

namespace App.Commands
{
    public sealed class CreateLocalGameCommand : ICommandAsync
    {
        private GameSessionsManager _gameSessionsManager;
        private UISystem _uiSystem;
        private ScenesLoader _scenesLoader;

        [Inject]
        private void Construct(GameSessionsManager gameSessionsManager, UISystem uiSystem, ScenesLoader scenesLoader)
        {
            _gameSessionsManager = gameSessionsManager;
            _uiSystem = uiSystem;
            _scenesLoader = scenesLoader;
        }

        public async Task Execute()
        {
            var cancellationToken = new CancellationTokenSource();
            await _scenesLoader.LoadTransitionSceneWithCancellationToken(cancellationToken);
            
            var sessionStorage = _gameSessionsManager.GetExistOrCreateLocalGame();
            var localGameSessionMediator = new LocalGamePlayController();
            var screenData = new GameScreenData(localGameSessionMediator, sessionStorage);
            
            _uiSystem.ShowScreen(ScreenId.Game, screenData);
            
            cancellationToken.Cancel();
        }
    }
}