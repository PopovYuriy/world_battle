using App.Data.Player;
using App.Services;
using App.Signals;
using Core.Commands;
using Core.UI;
using Core.UI.Enums;
using Game.Field;
using Game.Field.Mediators;
using UI.GameScreen.Data;
using Zenject;

namespace App.Commands
{
    public sealed class StartExistGameCommand : ICommand
    {
        [Inject] private StartExistGameSignal _signal;
        [Inject] private GameField _gameFieldPrefab;
        [Inject] private GameSessionsManager _gameSessionsManager;
        [Inject] private IPlayer _player;
        [Inject] private UISystem _uiSystem;
        
        public void Execute()
        {
            var gameSessionStorage = _gameSessionsManager.GetGame(_signal.GameUid);
            IGameMediator gameSessionMediator = _gameSessionsManager.IsLocalGame(_signal.GameUid) 
                ? new LocalGameMediator()
                : new OnlineGameMediator(_player);
            var screenData = new GameScreenData(_gameFieldPrefab, gameSessionMediator, gameSessionStorage, _player.Uid);
            _uiSystem.ShowScreen(ScreenId.Game, screenData);
        }
    }
}