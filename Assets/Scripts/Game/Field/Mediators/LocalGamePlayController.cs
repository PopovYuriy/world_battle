using System.Collections.Generic;
using Game.Data;

namespace Game.Field.Mediators
{
    public sealed class LocalGamePlayController : GamePlayControllerAbstract
    {
        public override IReadOnlyList<PlayerGameData> GetOrderedPlayersList() => SessionStorage.Data.Players;

        protected override void ProcessPostActivating()
        {
            DetermineUserColors();
            
            GameField.SetGridForPlayer(SessionStorage.Data.Grid, CurrentPlayer.Uid);
            GameField.UpdateInteractableForPlayer(CurrentPlayer.Uid);
        }

        protected override void ProcessWinImpl(WinData winData)
        {
            SessionStorage.Delete();
        }

        protected override void ProcessFinishTurn()
        {
            DetermineUserColors();
            
            GameField.SetGridForPlayer(SessionStorage.Data.Grid, CurrentPlayer.Uid);
            GameField.UpdateInteractableForPlayer(CurrentPlayer.Uid);
        }

        protected override void StorageUpdatedImpl() { }

        private void DetermineUserColors()
        {
            if (CurrentPlayer.Uid == SessionStorage.Data.Players[0].Uid)
                GameField.SetColors(ColorConfig.OwnerColor, ColorConfig.OpponentColor);
            else
                GameField.SetColors(ColorConfig.OpponentColor, ColorConfig.OwnerColor);
        }
    }
}