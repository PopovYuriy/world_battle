using App.Enums;
using Game.Data;
using Game.Grid.Cells.Controller;
using UI.Popups.PickLetterPopup;

namespace Game.Abilities.Runners
{
    public sealed class TransformationAbilityRunner : AbilityRunnerAbstract
    {
        public override void Run(string initiatorUid)
        {
            base.Run(initiatorUid);
            GridController.SetCellsInteractable(true);
            GridController.ForEach(cell => cell.SetReachable(true));
        }

        protected override void ProcessCellPick(Cell cell)
        {
            PickedCell.SetPicked(true);
            var confirmationPopupData = new PickLetterPopupData(cell.Model.Letter, ConfirmHandler, DeclineHandler);
            UiSystem.ShowPopup(PopupId.PickLetter, confirmationPopupData);
        }
        
        private void ConfirmHandler(char letter)
        {
            PickedCell.Model.SetLetter(letter);
            PickedCell.SetPicked(false);
            
            var data = new AbilityData(AbilityType.Transform, InitiatorUid, PickedCell.Model.Id, null);
            ProcessApply(data);
        }

        private void DeclineHandler()
        {
            PickedCell?.SetPicked(false);
            ProcessDecline();
        }
    }
}