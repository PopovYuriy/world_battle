using System.Threading.Tasks;
using App.Data.Player;
using App.Services;
using App.Signals;
using Core.Commands;
using Core.UI;
using Core.UI.Enums;
using Game.Field;
using Game.Field.Mediators;
using Game.Services.Storage;
using UI.GameScreen.Data;
using Zenject;

namespace App.Commands
{
    public sealed class CreateOnlineGameCommand : ICommandAsync
    {
        [Inject] private CreateOnlineGameSignal _signal;
        [Inject] private GameField _gameFieldPrefab;
        [Inject] private GameSessionsManager _gameSessionsManager;
        [Inject] private IPlayer _player;
        [Inject] private UISystem _uiSystem;
        
        public async Task Execute()
        {
            _gameSessionsManager.OnPendingGameConnected += PendingGameConnectedHandler;
            
            if (_signal.Type == CreateOnlineGameSignalType.Create)
                await _gameSessionsManager.CreatePendingGame(_signal.SecretWord);
            else
                await _gameSessionsManager.FindPendingGame(_signal.SecretWord);
        }

        private void PendingGameConnectedHandler(IGameSessionStorage gameSessionSessionStorage)
        {
            _signal.DispatchGameStarted();
            var onlineGameSessionMediator = new OnlineGameMediator(_player);
            var screenData = new GameScreenData(_gameFieldPrefab, onlineGameSessionMediator, gameSessionSessionStorage,
                _player.Uid);
            _uiSystem.ShowScreen(ScreenId.Game, screenData);
        }
    }
}