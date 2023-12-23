using System.Linq;
using App.Data.Player;
using UnityEngine;

namespace Game.Field.Mediators
{
    public sealed class OnlineGameMediator : GameMediatorAbstract
    {
        private readonly IPlayer _player;

        public OnlineGameMediator(IPlayer player)
        {
            _player = player;
        }

        protected override void ProcessPostInitializing()
        {
            var currentPlayerId = CurrentPlayer.Uid;
            GameField.SetColors(ColorConfig.OwnerColor, ColorConfig.OpponentColor);
            GameField.SetGridForPlayer(SessionStorage.Data.Grid, _player.Uid);
            GameField.SetActivePlayerIndicators(currentPlayerId);
            if (currentPlayerId != _player.Uid)
                GameField.SetPendingState();
            else
                GameField.UpdateInteractableForPlayer(currentPlayerId);
        }

        protected override void ProcessWin()
        {
            Debug.Log("!!!___WIN___!!!");
            SessionStorage.Delete();
            //show popup
        }

        protected override void ProcessFinishTurn()
        {
            GameField.SetPendingState();
            var opposedPlayer = GetOpposedPlayer(SessionStorage.Data.Players.First(p => p.Uid == _player.Uid));
            GameField.SetActivePlayerIndicators(opposedPlayer.Uid);
        }

        protected override void StorageUpdatedImpl()
        {
            var currentPlayerId = CurrentPlayer.Uid;
            GameField.SetActivePlayerIndicators(currentPlayerId);
            GameField.SetGridForPlayer(SessionStorage.Data.Grid, currentPlayerId);
            GameField.UpdateInteractableForPlayer(currentPlayerId);
        }
    }
}