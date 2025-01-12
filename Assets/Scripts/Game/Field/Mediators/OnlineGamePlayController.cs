using System.Collections.Generic;
using System.Linq;
using App.Modules.GameSessions.Data;

namespace Game.Field.Mediators
{
    public sealed class OnlineGamePlayController : GamePlayControllerAbstract
    {
        private const int PlayersCount = 2;

        protected override void ProcessPostInitializing()
        {
            GameField.SetApostropheCellColor(ColorConfig.OwnerColor);
        }

        public override IReadOnlyList<PlayerGameData> GetOrderedPlayersList()
        {
            var result = new List<PlayerGameData>(2)
            {
                SessionController.Data.Players.First(p => p.Uid == OwnerPlayerId),
                SessionController.Data.Players.First(p => p.Uid != OwnerPlayerId)
            };
            return result;
        }

        protected override void ProcessPostActivating()
        {
            GameField.SetColors(ColorConfig.OwnerColor, ColorConfig.OpponentColor);
            GameField.SetGridForPlayer(SessionController.Data.Grid, OwnerPlayerId);
            
            if (CurrentPlayer.Uid != OwnerPlayerId)
                GameField.TurnOffCellsInteractable();
            else
                GameField.UpdateInteractableForPlayer(CurrentPlayer.Uid);
        }

        protected override void ProcessFinishTurn()
        {
            GameField.TurnOffCellsInteractable();
        }

        protected override void ProcessWinImpl(WinData winData)
        {
            winData.ProcessCount++;
            if (winData.ProcessCount == PlayersCount)
                SessionController.Delete();
            else
                SessionController.Save();
        }

        protected override void StorageUpdatedImpl()
        {
            GameField.SetGridForPlayer(SessionController.Data.Grid, CurrentPlayer.Uid);
            GameField.UpdateInteractableForPlayer(CurrentPlayer.Uid);
            GameField.SetApostropheCellInteractable(true);
        }
    }
}