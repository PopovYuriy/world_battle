using System;
using System.Collections.Generic;
using Game.Grid.Cell.Model;
using Newtonsoft.Json;
using UnityEngine;
using Utils.Extensions;

namespace Game.Grid
{
    [Serializable]
    public sealed class GridModel
    {
        [JsonProperty(PropertyName = "Cells")] public List<List<CellModel>> Cells { get; }

        public GridModel(int rows, int columns)
        {
            Cells = new List<List<CellModel>>(rows);
            rows.Iterate(i => Cells.Add(new List<CellModel>(columns)));
        }

        [JsonConstructor]
        public GridModel(List<List<CellModel>> cells) => Cells = cells;

        public void SetCell(int rowIndex, int columnIndex, CellModel cellModel) => Cells[rowIndex][columnIndex] = cellModel;

        public void RotateCellsMatrix()
        {
            var n = Cells.Count;
            for (var i = 0; i < n / 2; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    var current = Cells[i][j];
                    Cells[i][j] = Cells[n - i - 1][j];
                    Cells[n - i - 1][j] = current;
                }
            }

            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n / 2; j++)
                {
                    var current = Cells[i][j];
                    Cells[i][j] = Cells[i][n - j - 1];
                    Cells[i][n - j - 1] = current;
                }
            }
        }
        
        public IEnumerable<CellModel> GetAvailableCellsForUser(string userId)
        {
            var cells = new List<CellModel>();
            Cells.Count.Iterate(rowIndex =>
            {
                var row = Cells[rowIndex];
                for (var columnIndex = 0; columnIndex < row.Count; columnIndex++)
                {
                    var leftCell = new Vector2Int(rowIndex, Mathf.Max(0, columnIndex - 1));
                    var rightCell = new Vector2Int(rowIndex, Mathf.Min(row.Count - 1, columnIndex + 1));
                    var upperCell = new Vector2Int(Mathf.Max(0, rowIndex - 1), columnIndex);
                    var lowerCell = new Vector2Int(Mathf.Min(row.Count - 1, rowIndex + 1), columnIndex);
                    
                    var isCaptured = IsCaptured(leftCell) || IsCaptured(rightCell) || IsCaptured(upperCell) || IsCaptured(lowerCell);
                    if (isCaptured)
                        cells.Add(Cells[rowIndex][columnIndex]);
                }
            });

            return cells;
            
            bool IsCaptured(Vector2Int cellPosition) => Cells[cellPosition.x][cellPosition.y].PlayerId == userId;
        }
    }
}