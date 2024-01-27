using App.Enums;
using Core.UI;
using Game.Data;
using Game.Grid;
using Game.Grid.Cells.Controller;
using Game.Grid.Cells.Enum;
using UI.Popups.ConfirmationPopup;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Abilities.Runners
{
    public sealed class CaptureAbilityRunnerAbstract : AbilityRunnerAbstract
    {
        [SerializeField] private GridController _gridController;
        [SerializeField] private Button _faderButton;

        private UISystem _uiSystem;

        private string _initiatorUid;
        private Cell _pickedCell;

        public override void Initialize(UISystem uiSystem)
        {
            _uiSystem = uiSystem;
        }
        
        public override void Run(string initiatorUid)
        {
            _initiatorUid = initiatorUid;
            _gridController.OnCellClicked += CellClickedHandler;
            _gridController.SetCellsInteractable(true);
            _gridController.ForEach(cell =>
            {
                var isCaptured = cell.State == CellState.Captured || cell.IsBase;
                cell.SetInteractable(!isCaptured);
                cell.SetReachable(!isCaptured);
            });
            _faderButton.gameObject.SetActive(true);
            _faderButton.onClick.AddListener(FaderClickHandler);
        }

        public override void Finalize()
        {
            _gridController.OnCellClicked -= CellClickedHandler;
            _faderButton.gameObject.SetActive(false);
            _faderButton.onClick.RemoveListener(FaderClickHandler);
        }
        
        private void CellClickedHandler(Cell cell)
        {
            if (cell.State == CellState.Captured || cell.IsBase)
                return;
            
            _pickedCell = cell;
            _pickedCell.SetPicked(true);
            var confirmationPopupData = new ConfirmationPopupData(
                ConfirmationPopupType.TwoButtons,
                string.Empty,
                ConfirmationPopupText.MainText.CaptureCellConfirmation,
                ConfirmationPopupText.ConfirmButton.Yes,
                OnCaptureConfirmed,
                ConfirmationPopupText.DeclineButton.No,
                OnCaptureDeclined
            );
            _uiSystem.ShowPopup(PopupId.ConfirmationPopup, confirmationPopupData);
        }

        private void OnCaptureConfirmed()
        {
            switch (_pickedCell.State)
            {
                case CellState.Captured:
                    break;
                case CellState.Default:
                    _pickedCell.Model.SetPlayerId(_initiatorUid);
                    break;
                case CellState.Opposed:
                    _pickedCell.Model.SetPlayerId(string.Empty);
                    break;
            }

            _pickedCell.SetPicked(false);
            
            var data = new AbilityData(AbilityType.Capture, _initiatorUid, _pickedCell.Model.Id, null);
            ProcessApply(data);
        }

        private void OnCaptureDeclined()
        {
            _pickedCell?.SetPicked(false);
            ProcessDecline();
        }

        private void FaderClickHandler()
        {
            _pickedCell?.SetPicked(false);
            ProcessDecline();
        }
    }
}