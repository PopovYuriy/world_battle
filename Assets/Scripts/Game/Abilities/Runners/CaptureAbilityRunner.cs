using App.Enums;
using App.Modules.GameSessions.Data;
using Game.Grid.Cells.Controller;
using Game.Grid.Cells.Enum;
using UI.Popups.ConfirmationPopup;

namespace Game.Abilities.Runners
{
    public sealed class CaptureAbilityRunner : AbilityRunnerAbstract
    {
        public override void Run(string initiatorUid)
        {
            base.Run(initiatorUid);
            
            GridView.ForEach(cell =>
            {
                var isCaptured = cell.State == CellState.Captured || cell.IsBase;
                cell.SetInteractable(!isCaptured);
                cell.SetReachable(!isCaptured);
            });
        }
        
        protected override void ProcessCellPick(Cell cell)
        {
            if (cell.State == CellState.Captured || cell.IsBase)
                return;
            
            PickedCell.SetPicked(true);
            var confirmationPopupData = new ConfirmationPopupData(
                ConfirmationPopupType.TwoButtons,
                string.Empty,
                ConfirmationPopupText.MainText.CaptureCellConfirmation,
                ConfirmationPopupText.ConfirmButton.Yes,
                OnCaptureConfirmed,
                ConfirmationPopupText.DeclineButton.No,
                OnCaptureDeclined
            );
            UiSystem.ShowPopup(PopupId.ConfirmationPopup, confirmationPopupData);
        }

        private void OnCaptureConfirmed()
        {
            PickedCell.SetPicked(false);
            switch (PickedCell.State)
            {
                case CellState.Captured:
                    break;
                case CellState.Default:
                    PickedCell.Model.SetPlayerId(InitiatorUid);
                    break;
                case CellState.Opposed:
                    PickedCell.Model.SetPlayerId(string.Empty);
                    break;
            }
            GameSessionData.AbilityData = new AbilityData(AbilityType.Capture, InitiatorUid, PickedCell.Model.Id, -1);
            ProcessApply();
        }

        private void OnCaptureDeclined()
        {
            PickedCell?.SetPicked(false);
            ProcessDecline();
        }
    }
}