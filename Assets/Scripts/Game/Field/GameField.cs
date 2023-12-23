using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Grid;
using Game.Grid.Cell.Controller;
using Game.Grid.Cell.Enum;
using Game.Grid.Cell.Model;
using ModestTree;
using UnityEngine;

namespace Game.Field
{
    public sealed class GameField : MonoBehaviour
    {
        private const int OwnRow = 4;

        [SerializeField] private List<CellsRow> _rows;
        
        private PlayerGameData[] _players;
        private Color _capturedStateCellColor;
        private Color _opposedStateCellColor;
        
        private List<Cell> _pickedCells;
        
        public event Action<char> OnLetterPick;

        public void Initialize(PlayerGameData[] players)
        {
            _players = players;

            InitializeGrid();
            
            _pickedCells = new List<Cell>();
        }
        
        private void OnDestroy()
        {
            foreach (var cell in _rows.SelectMany(t => t.Cells))
                cell.OnClick -= CellClickHandler;
        }

        public void SetColors(Color capturedCellColor, Color opposedCellColor)
        {
            _capturedStateCellColor = capturedCellColor;
            _opposedStateCellColor = opposedCellColor;
        }
        
        public void ResetPickedCells()
        {
            foreach (var cellController in _pickedCells)
            {
                cellController.SetPicked(false);
                cellController.SetInteractable(true);
            }
            _pickedCells.Clear();
        }

        public void SetGridForPlayer(GridModel grid, string uid)
        {
            var playerIndex = _players.IndexOf(_players.First(p => p.Uid == uid));
            var rowsCount = grid.Cells.Count;
            if (playerIndex == 0)
            {
                for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    var row = grid.Cells[rowIndex];
                    for (var columnIndex = 0; columnIndex < row.Count; columnIndex++)
                    {
                        var cell = _rows[rowIndex].Cells[columnIndex];
                        var model = row[columnIndex];
                        UpdateCell(cell, model, uid);
                    }
                }
            }
            else
            {
                for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    var row = grid.Cells[rowIndex].ToArray().Reverse().ToArray();
                    for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
                    {
                        var cell = _rows[rowsCount - rowIndex - 1].Cells[columnIndex];
                        var model = row[columnIndex];
                        UpdateCell(cell, model, uid);
                    }
                }
            }
        }

        public void TurnOffCellsInteractable()
        {
            for (var rowIndex = OwnRow; rowIndex >= 0; rowIndex--)
            {
                var row = _rows[rowIndex];
                for (var columnIndex = 0; columnIndex < row.Cells.Length; columnIndex++)
                {
                    var cell = _rows[rowIndex].Cells[columnIndex];
                    cell.SetInteractable(false);
                }
            }
        }
        
        public void UpdateInteractableForPlayer(string uid)
        {
            for (var rowIndex = OwnRow; rowIndex >= 0; rowIndex--)
            {
                var row = _rows[rowIndex];
                for (var columnIndex = 0; columnIndex < row.Cells.Length; columnIndex++)
                {
                    var leftCell = new Vector2Int(rowIndex, Mathf.Max(0, columnIndex - 1));
                    var rightCell = new Vector2Int(rowIndex, Mathf.Min(row.Cells.Length - 1, columnIndex + 1));
                    var upperCell = new Vector2Int(Mathf.Max(0, rowIndex - 1), columnIndex);
                    var lowerCell = new Vector2Int(Mathf.Min(OwnRow, rowIndex + 1), columnIndex);
                    
                    var cell = _rows[rowIndex].Cells[columnIndex];
                    var isReachable = IsCaptured(leftCell) 
                                      || IsCaptured(rightCell) 
                                      || IsCaptured(upperCell) 
                                      || IsCaptured(lowerCell);
                    cell.SetInteractable(isReachable);
                    cell.SetReachable(isReachable);
                }
            }

            bool IsCaptured(Vector2Int cellPosition) =>
                _rows[cellPosition.x].Cells[cellPosition.y].Model.PlayerId == uid;
        }

        public void ApplyWordForPlayer(string uid)
        {
            foreach (var cell in _pickedCells)
            {
                switch (cell.State)
                {
                    case CellState.Captured:
                        cell.Model.SetPoints(cell.Model.Points + 1);
                        break;
                    case CellState.Default:
                        cell.Model.SetPoints(cell.Model.Points + 1);
                        cell.SetState(CellState.Captured);
                        cell.Model.SetPlayerId(uid);
                        break;
                    case CellState.Opposed:
                        cell.SetState(CellState.Default);
                        cell.Model.SetPlayerId(string.Empty);
                        break;
                }
            }
        }

        private void InitializeGrid()
        {
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

        private void CellClickHandler(Cell cell)
        {
            cell.SetPicked(true);
            cell.SetInteractable(false);
            _pickedCells.Add(cell);
            OnLetterPick?.Invoke(cell.Model.Letter);
        }

        private void UpdateCell(Cell cell, CellModel model, string playerId)
        {
            cell.SetModel(model);
            var state = model.PlayerId == string.Empty
                ? CellState.Default
                : model.PlayerId == playerId
                    ? CellState.Captured
                    : CellState.Opposed;
            cell.SetState(state);
            
            if (state != CellState.Default)
                cell.SetColor(state == CellState.Captured ? _capturedStateCellColor : _opposedStateCellColor);
        }

        [Serializable]
        private sealed class CellsRow
        {
            [field: SerializeField] public Cell[] Cells { get; private set; }
        }
    }
}