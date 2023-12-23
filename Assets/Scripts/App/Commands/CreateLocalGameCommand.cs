using App.Data.Player;
using App.Services;
using Core.Commands;
using Core.UI;
using Core.UI.Enums;
using Game.Field;
using Game.Field.Mediators;
using UI.GameScreen.Data;
using Zenject;

namespace App.Commands
{
    public sealed class CreateLocalGameCommand : ICommand
    {
        [Inject] private GameField _gameFieldPrefab;
        [Inject] private GameSessionsManager _gameSessionsManager;
        [Inject] private IPlayer _player;
        [Inject] private UISystem _uiSystem;
        
        public void Execute()
        {
            var sessionStorage = _gameSessionsManager.GetExistOrCreateLocalGame();
            var localGameSessionMediator = new LocalGameMediator();
            var screenData = new GameScreenData(_gameFieldPrefab, localGameSessionMediator, sessionStorage, _player.Uid);
            _uiSystem.ShowScreen(ScreenId.Game, screenData);
        }
    }
}