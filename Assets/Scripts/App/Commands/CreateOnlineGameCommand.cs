using System.Threading.Tasks;
using App.Enums;
using App.Services;
using App.Signals;
using Core.Commands;
using Core.UI;
using Game.Field.Mediators;
using Game.Services.Storage;
using UI.GameScreen.Data;
using UnityEngine;
using Zenject;

namespace App.Commands
{
    public sealed class CreateOnlineGameCommand : ICommandAsync
    {
        private CreateOnlineGameSignal _signal;
        private GameSessionsManager _gameSessionsManager;
        private UISystem _uiSystem;

        [Inject]
        private void Construct(CreateOnlineGameSignal signal, GameSessionsManager gameSessionsManager, UISystem uiSystem)
        {
            _signal = signal;
            _gameSessionsManager = gameSessionsManager;
            _uiSystem = uiSystem;
        }

        public async Task Execute()
        {
            _gameSessionsManager.OnPendingGameConnected += PendingGameConnectedHandler;

            switch (_signal.Type)
            {
                case CreateOnlineGameSignalType.Create:
                    await _gameSessionsManager.CreatePendingGame(_signal.SecretWord);
                    break;
                
                case CreateOnlineGameSignalType.Find:
                    await _gameSessionsManager.FindPendingGame(_signal.SecretWord);
                    break;
                
                default:
                    Debug.LogError("Invalid game creation type");
                    break;
            }
        }

        private void PendingGameConnectedHandler(IGameSessionStorage gameSessionSessionStorage)
        {
            _gameSessionsManager.OnPendingGameConnected -= PendingGameConnectedHandler;
            
            _signal.DispatchGameStarted();
            var onlineGameSessionMediator = new OnlineGamePlayController();
            var screenData = new GameScreenData(onlineGameSessionMediator, gameSessionSessionStorage);
            _uiSystem.ShowScreen(ScreenId.Game, screenData);
        }
    }
}