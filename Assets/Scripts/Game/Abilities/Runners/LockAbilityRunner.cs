using App.Enums;
using App.Modules.GameSessions.Data;
using Game.Grid.Cells.Controller;
using UI.Popups.ConfirmationPopup;

namespace Game.Abilities.Runners
{
    public sealed class LockAbilityRunner : AbilityRunnerAbstract
    {
        public override void Run(string initiatorUid)
        {
            base.Run(initiatorUid);
            GridView.ForEach(cell => cell.SetReachable(true));
        }

        protected override void ProcessCellPick(Cell cell)
        {
            PickedCell.SetPicked(true);
            var confirmationPopupData = new ConfirmationPopupData(
                ConfirmationPopupType.TwoButtons,
                string.Empty,
                ConfirmationPopupText.MainText.LockCellConfirmation,
                ConfirmationPopupText.ConfirmButton.Yes,
                ConfirmHandler,
                ConfirmationPopupText.DeclineButton.No,
                DeclineHandler
            );
            UiSystem.ShowPopup(PopupId.ConfirmationPopup, confirmationPopupData);
        }

        private void ConfirmHandler()
        {
            PickedCell.SetPicked(false);

            PickedCell.Model.SetIsLocked(true);
            GameSessionData.ModificationsData ??= new ModificationsData();
            GameSessionData.ModificationsData.LockedCells.Add(new LockedCellData(PickedCell.Model.Id, GameSessionData.Turns.Count + 1));
            GameSessionData.AbilityData = new AbilityData(AbilityType.Lock, InitiatorUid, PickedCell.Model.Id,
                GameSessionData.Turns.Count + 1);
            ProcessApply();
        }

        private void DeclineHandler()
        {
            PickedCell?.SetPicked(false);
            ProcessDecline();
        }
    }
}