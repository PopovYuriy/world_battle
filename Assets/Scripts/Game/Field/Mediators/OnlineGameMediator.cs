using System.Collections.Generic;
using System.Linq;
using Game.Data;
using UnityEngine;

namespace Game.Field.Mediators
{
    public sealed class OnlineGameMediator : GameMediatorAbstract
    {
        public override IReadOnlyList<PlayerGameData> GetOrderedPlayersList()
        {
            var result = new List<PlayerGameData>(2)
            {
                SessionStorage.Data.Players.First(p => p.Uid == OwnerPlayerId),
                SessionStorage.Data.Players.First(p => p.Uid != OwnerPlayerId)
            };
            return result;
        }

        protected override void ProcessPostInitializing()
        {
            GameField.SetColors(ColorConfig.OwnerColor, ColorConfig.OpponentColor);
            GameField.SetGridForPlayer(SessionStorage.Data.Grid, OwnerPlayerId);
            
            if (CurrentPlayer.Uid != OwnerPlayerId)
                GameField.TurnOffCellsInteractable();
            else
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
            GameField.TurnOffCellsInteractable();
        }

        protected override void StorageUpdatedImpl()
        {
            GameField.SetGridForPlayer(SessionStorage.Data.Grid, CurrentPlayer.Uid);
            GameField.UpdateInteractableForPlayer(CurrentPlayer.Uid);
        }
    }
}