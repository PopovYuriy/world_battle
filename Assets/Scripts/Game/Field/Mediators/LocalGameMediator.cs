using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game.Field.Mediators
{
    public sealed class LocalGameMediator : GameMediatorAbstract
    {
        public override IReadOnlyList<PlayerGameData> GetOrderedPlayersList() => SessionStorage.Data.Players;

        protected override void ProcessPostInitializing()
        {
            DetermineUserColors();
            
            GameField.SetGridForPlayer(SessionStorage.Data.Grid, CurrentPlayer.Uid);
            GameField.UpdateInteractableForPlayer(CurrentPlayer.Uid);
        }

        protected override void ProcessWin()
        {
            Debug.Log("!!!___WIN___!!!");
            SessionStorage.Delete();
            //show popup
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