using System.Threading;
using System.Threading.Tasks;
using App.Enums;
using App.Modules.GameSessions;
using App.Signals;
using Core.Commands;
using Core.Services.Scene;
using Core.UI;
using Game.Field.Mediators;
using UI.Screens.GameScreen.Data;
using Zenject;

namespace App.Commands
{
    public sealed class StartExistGameCommand : ICommandAsync
    {
        [Inject] private StartExistGameSignal _signal;
        [Inject] private IGameSessionsManager _gameSessionsManager;
        [Inject] private UISystem _uiSystem;
        [Inject] private ScenesLoader _scenesLoader;

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