using System.Threading;
using System.Threading.Tasks;
using App.Enums;
using App.Modules.GameSessions;
using App.Modules.GameSessions.API.Enums;
using Core.Commands;
using Core.Services.Scene;
using Core.UI;
using Cysharp.Threading.Tasks;
using Game.Field.Mediators;
using UI.Screens.GameScreen.Data;
using Zenject;

namespace App.Commands
{
    public sealed class CreateLocalGameCommand : ICommandAsync
    {
        [Inject] private IGameSessionsManager GameSessionsManager { get; set; }
        [Inject] private UISystem             UiSystem            { get; set; }
        [Inject] private ScenesLoader         ScenesLoader        { get; set; }
        [Inject] private SignalBus            SignalBus           { get; set; }

        private bool _hold;

        public async Task Execute()
        {
            if (GameSessionsManager.LocalController != null)
                return;

            using var cancellationToken = new CancellationTokenSource();
            await ScenesLoader.LoadTransitionSceneWithCancellationToken(cancellationToken);

            _hold = true;
            SignalBus.Subscribe<GameSessionsSignal.GameCreatedSignal>(GameCreatedHandler);
            GameSessionsManager.CreateLocalGame();
            
            await UniTask.WaitWhile(() => _hold);
            
            cancellationToken.Cancel();
        }

        private void GameCreatedHandler(GameSessionsSignal.GameCreatedSignal signal)
        {
            if (signal.Arg1 != GameSessionType.Local)
            {
                _hold = false;
                return;
            }
            
            var localGameSessionMediator = new LocalGamePlayController();
            var screenData = new GameScreenData(localGameSessionMediator, GameSessionsManager.GetGame(signal.Arg2.Uid));

            UiSystem.ShowScreen(ScreenId.Game, screenData);
            _hold = false;
        }
    }
}