using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Grid;
using Game.Grid.Cells.Controller;
using Game.Grid.Cells.Enum;
using Game.Grid.Cells.Model;
using ModestTree;
using UnityEngine;

namespace Game.Field
{
    public sealed class GameField
    {
        private const int OpposedBaseRowIndex = 0;
        private const int OwnBaseRowIndex = 4;

        private GridController _grid;
        
        private PlayerGameData[] _players;
        private Color _capturedStateCellColor;
        private Color _opposedStateCellColor;

        public List<Cell> PickedCells { get; }

        public event Action<string> OnPickedLettersChanged;

        public GameField(PlayerGameData[] players, GridController grid)
        {
            _players = players;
            _grid = grid;
            _grid.ForEach(cell => cell.SetPicked(false));
            
            PickedCells = new List<Cell>(_grid.Rows * _grid.Columns);
        }

        public void Activate()
        {
            _grid.OnCellClicked += CellClickHandler;
        }
        
        public void Deactivate()
        {
            _grid.OnCellClicked -= CellClickHandler;
        }

        public void SetColors(Color capturedCellColor, Color opposedCellColor)
        {
            _capturedStateCellColor = capturedCellColor;
            _opposedStateCellColor = opposedCellColor;
        }

        public Cell GetCellById(int cellId) => _grid.GetCell(cellId);
        
        public void ResetPickedCells()
        {
            foreach (var cellController in PickedCells)
            {
                cellController.SetPicked(false);
                cellController.SetInteractable(true);
            }
            PickedCells.Clear();
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
                        var cell = _grid.GetCell(rowIndex, columnIndex);
                        var model = row[columnIndex];
                        UpdateCellModel(cell, model, uid);
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
                        var cell = _grid.GetCell(rowsCount - rowIndex - 1, columnIndex);
                        var model = row[columnIndex];
                        UpdateCellModel(cell, model, uid);
                    }
                }
            }
        }

        public void TurnOffCellsInteractable()
        {
            for (var rowIndex = 0; rowIndex < _grid.Rows; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < _grid.Columns; columnIndex++)
                {
                    var cell = _grid.GetCell(rowIndex, columnIndex);
                    cell.SetInteractable(false);
                }
            }
        }
        
        public void UpdateInteractableForPlayer(string uid)
        {
            var availableCells = GetAvailableCellsForPlayer(uid);
            for (var rowIndex = OwnBaseRowIndex; rowIndex >= 0; rowIndex--)
            {
                for (var columnIndex = 0; columnIndex < _grid.Columns; columnIndex++)
                {
                    var cell = _grid.GetCell(rowIndex, columnIndex);
                    if (cell.Model.IsLocked)
                        continue;
                    
                    var isReachable = availableCells.Contains(cell);
                    cell.SetInteractable(isReachable);
                    cell.SetReachable(isReachable);
                }
            }
        }

        public void ApplyWordForPlayer(string uid)
        {
            foreach (var cell in PickedCells)
            {
                switch (cell.State)
                {
                    case CellState.Captured:
                        cell.Model.SetPoints(cell.Model.Points + 1);
                        cell.UpdatePoints();
                        break;
                    case CellState.Default:
                        cell.Model.SetPoints(cell.Model.Points + 1);
                        cell.Model.SetPlayerId(uid);
                        cell.SetState(CellState.Captured);
                        cell.SetColor(_capturedStateCellColor);
                        cell.UpdatePoints();
                        break;
                    case CellState.Opposed:
                        cell.Model.SetPoints(0);
                        cell.SetState(CellState.Default);
                        cell.Model.SetPlayerId(string.Empty);
                        break;
                }
            }
        }

        public void UpdateGridCellsStatesForPlayer(string playerUid)
        {
            for (var rowIndex = 0; rowIndex < _grid.Rows; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < _grid.Columns; columnIndex++)
                {
                    var cell = _grid.GetCell(rowIndex, columnIndex);
                    UpdateCellStateForPlayer(cell, playerUid);
                }
            }
        }

        public IReadOnlyList<char> GetAvailableLettersForPlayer(string uid)
        {
            return GetAvailableCellsForPlayer(uid).Select(c => c.Model.Letter).ToList();
        }

        public IEnumerable<CellModel> GetOpposedBaseCellModels()
        {
            return _grid.GetRow(OpposedBaseRowIndex).Select(c => c.Model);
        }

        private void CellClickHandler(Cell cell)
        {
            if (PickedCells.Contains(cell))
            {
                PickedCells.Remove(cell);
                cell.SetPicked(false);
            }
            else
            {
                PickedCells.Add(cell);
                cell.SetPicked(true);
            }
            
            OnPickedLettersChanged?.Invoke(new string(PickedCells.Select(c => c.Model.Letter).ToArray()));
        }

        private void UpdateCellModel(Cell cell, CellModel model, string playerId)
        {
            cell.SetModel(model);
            UpdateCellStateForPlayer(cell, playerId);
        }

        private void UpdateCellStateForPlayer(Cell cell, string playerId)
        {
            var state = cell.Model.PlayerId == string.Empty
                ? CellState.Default
                : cell.Model.PlayerId == playerId
                    ? CellState.Captured
                    : CellState.Opposed;
            cell.SetState(state);
            
            if (state != CellState.Default)
                cell.SetColor(state == CellState.Captured ? _capturedStateCellColor : _opposedStateCellColor);
        }
        
        private IReadOnlyList<Cell> GetAvailableCellsForPlayer(string uid)
        {
            var result = new List<Cell>(_grid.Rows * _grid.Columns);
            for (var rowIndex = OwnBaseRowIndex; rowIndex >= 0; rowIndex--)
            {
                for (var columnIndex = 0; columnIndex < _grid.Columns; columnIndex++)
                {
                    var ownCell = new Vector2Int(rowIndex, columnIndex);
                    var leftCell = new Vector2Int(rowIndex, Mathf.Max(0, columnIndex - 1));
                    var rightCell = new Vector2Int(rowIndex, Mathf.Min(_grid.Columns - 1, columnIndex + 1));
                    var upperCell = new Vector2Int(Mathf.Max(0, rowIndex - 1), columnIndex);
                    var lowerCell = new Vector2Int(Mathf.Min(OwnBaseRowIndex, rowIndex + 1), columnIndex);

                    var isReachable = !_grid.GetCell(rowIndex,columnIndex).Model.IsLocked
                                      && (IsCaptured(ownCell) 
                                      || IsCaptured(leftCell)
                                      || IsCaptured(rightCell)
                                      || IsCaptured(upperCell)
                                      || IsCaptured(lowerCell));
                    
                    if (isReachable)
                        result.Add(_grid.GetCell(rowIndex, columnIndex));
                }
            }

            return result;

            bool IsCaptured(Vector2Int cellPos) => _grid.GetCell(cellPos.x,cellPos.y).Model.PlayerId == uid;
        }
    }
}