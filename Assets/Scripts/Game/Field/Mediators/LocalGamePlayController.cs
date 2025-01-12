using System.Collections.Generic;
using App.Modules.GameSessions.Data;

namespace Game.Field.Mediators
{
    public sealed class LocalGamePlayController : GamePlayControllerAbstract
    {
        public override IReadOnlyList<PlayerGameData> GetOrderedPlayersList() => SessionController.Data.Players;

        protected override void ProcessPostInitializing()
        {
            UpdateApostropheColor();
        }

        protected override void ProcessPostActivating()
        {
            DetermineUserColors();
            
            GameField.SetGridForPlayer(SessionController.Data.Grid, CurrentPlayer.Uid);
            GameField.UpdateInteractableForPlayer(CurrentPlayer.Uid);
        }

        protected override void ProcessWinImpl(WinData winData)
        {
            SessionController.Delete();
        }

        protected override void ProcessFinishTurn()
        {
            DetermineUserColors();
            UpdateApostropheColor();
            GameField.SetGridForPlayer(SessionController.Data.Grid, CurrentPlayer.Uid);
            GameField.UpdateInteractableForPlayer(CurrentPlayer.Uid);
        }

        protected override void StorageUpdatedImpl() { }

        private void DetermineUserColors()
        {
            if (CurrentPlayer.Uid == SessionController.Data.Players[0].Uid)
                GameField.SetColors(ColorConfig.OwnerColor, ColorConfig.OpponentColor);
            else
                GameField.SetColors(ColorConfig.OpponentColor, ColorConfig.OwnerColor);
        }

        private void UpdateApostropheColor()
        {
            var color = CurrentPlayer.Uid == OwnerPlayerId ? ColorConfig.OwnerColor : ColorConfig.OpponentColor;
            GameField.SetApostropheCellColor(color);
        }
    }
}