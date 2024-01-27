using System;
using System.Collections.Generic;
using System.Linq;
using Game.Grid.Cells.Controller;
using UnityEngine;

namespace Game.Grid
{
    public sealed class GridController : MonoBehaviour
    {
        [SerializeField] private List<CellsRow> _rows;

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public event Action<Cell> OnCellClicked;

        public void Initialize()
        {
            Rows = _rows.Count;
            Columns = _rows.First().Cells.Length;
            
            foreach (var cellsRow in _rows)
            {
                for (var y = 0; y < _rows[0].Cells.Length; y++)
                {
                    var cell = cellsRow.Cells[y];
                    cell.OnClick += CellClickHandler;
                    cell.SetPicked(false);
                }
            }
        }
        
        private void OnDestroy()
        {
            foreach (var cell in _rows.SelectMany(t => t.Cells))
                cell.OnClick -= CellClickHandler;
        }

        public Cell GetCell(int rowIndex, int columnIndex) => _rows[rowIndex].Cells[columnIndex];

        public IEnumerable<Cell> GetRow(int rowIndex) => _rows[rowIndex].Cells;
        
        public void SetCellsInteractable(bool isInteractable)
        {
            foreach (var cellsRow in _rows)
            {
                for (var y = 0; y < _rows[0].Cells.Length; y++)
                {
                    var cell = cellsRow.Cells[y];
                    cell.SetInteractable(isInteractable);
                }
            }
        }

        public void ForEach(Action<Cell> action)
        {
            foreach (var cellsRow in _rows)
                foreach (var cell in cellsRow.Cells)
                    action?.Invoke(cell);
        }

        private void CellClickHandler(Cell cell)
        {
            OnCellClicked?.Invoke(cell);
        }
        
        [Serializable]
        private sealed class CellsRow
        {
            [field: SerializeField] public Cell[] Cells { get; private set; }
        }
    }
}