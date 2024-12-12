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
            GridView.ForEach(cell => cell.SetReachable(true));
        }

        protected override void ProcessCellPick(Cell cell)
        {
            PickedCell.SetPicked(true);
            var confirmationPopupData = new PickLetterPopupData(cell.Model.Letter, ConfirmHandler, DeclineHandler);
            UiSystem.ShowPopup(PopupId.PickLetter, confirmationPopupData);
        }
        
        private void ConfirmHandler(char letter)
        {
            PickedCell.SetPicked(false);
            PickedCell.Model.SetLetter(letter);
            
            GameSessionData.AbilityData = new AbilityData(AbilityType.Transform, InitiatorUid, PickedCell.Model.Id, -1);
            ProcessApply();
        }

        private void DeclineHandler()
        {
            PickedCell?.SetPicked(false);
            ProcessDecline();
        }
    }
}