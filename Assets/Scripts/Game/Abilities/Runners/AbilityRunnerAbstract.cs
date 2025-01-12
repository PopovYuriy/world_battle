using System;
using App.Modules.GameSessions.Data;
using Core.UI;
using Game.Grid;
using Game.Grid.Cells.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Abilities.Runners
{
    public abstract class AbilityRunnerAbstract : MonoBehaviour
    {
        [field: SerializeField] public AbilityType AbilityType { get; private set; }
        [field: SerializeField] protected GridView GridView { get; private set; }
        [field: SerializeField] protected Button FaderButton { get; private set; }

        protected string InitiatorUid { get; private set; }
        protected UISystem UiSystem { get; private set; }
        protected GameSessionData GameSessionData { get; private set; }
        protected Cell PickedCell { get; private set; }
        
        public event Action OnApplied;
        public event Action OnDeclined;

        public virtual void Initialize(UISystem uiSystem, GameSessionData data)
        {
            UiSystem = uiSystem;
            GameSessionData = data;
        }
        
        public virtual void Run(string initiatorUid)
        {
            InitiatorUid = initiatorUid;
            
            GridView.OnCellClicked += CellClickedHandler;
            GridView.SetCellsInteractable(true);
            GridView.ApostropheCell.SetInteractable(false);
            
            FaderButton.gameObject.SetActive(true);
            FaderButton.onClick.AddListener(FaderClickHandler);
        }

        public virtual void Finalize()
        {
            GridView.OnCellClicked -= CellClickedHandler;
            FaderButton.gameObject.SetActive(false);
            GridView.ApostropheCell.SetInteractable(true);
            FaderButton.onClick.RemoveListener(FaderClickHandler);
        }

        private void CellClickedHandler(Cell cell)
        {
            PickedCell = cell;
            ProcessCellPick(cell);
        }

        protected abstract void ProcessCellPick(Cell cell);

        protected void ProcessApply()
        {
            OnApplied?.Invoke();
        }

        protected void ProcessDecline()
        {
            OnDeclined?.Invoke();
        }
        
        private void FaderClickHandler()
        {
            PickedCell?.SetPicked(false);
            ProcessDecline();
        }
    }
}