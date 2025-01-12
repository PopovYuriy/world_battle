using System;
using System.Collections.Generic;
using System.Linq;
using App.Modules.GameSessions.Data;
using Game.Grid.Cells.Model;
using ModestTree;
using TMPro;
using UI.MainMenuScreen.Enums;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace UI.MainMenuScreen
{
    public sealed class MainMenuItemComponent : MonoBehaviour
    {
        [SerializeField] private Button          _button;
        [SerializeField] private TextMeshProUGUI _opponentName;
        [SerializeField] private List<CellsRow>  _rows;

        private string _gameUid;

        public event Action<string> OnClicked;

        private void Awake()
        {
            _button.onClick.AddListener(ClickHandler);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(ClickHandler);
        }

        public void Initialize(GameSessionData data, string ownerUid)
        {
            _gameUid = data.Uid;
            var opponentName = data.Players.First(p => p.Uid != ownerUid).Name;
            _opponentName.SetText($"Гра з {opponentName}");
            
            var playerIndex = data.Players.IndexOf(data.Players.First(p => p.Uid == ownerUid));
            var rowsCount = data.Grid.Cells.Count;
            var isActive = data.LastTurnPlayerId.IsNullOrEmpty() ? playerIndex == 0 : data.LastTurnPlayerId != ownerUid;
            if (playerIndex == 0)
            {
                for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    var row = data.Grid.Cells[rowIndex];
                    for (var columnIndex = 0; columnIndex < row.Count; columnIndex++)
                    {
                        var cell = _rows[rowIndex].Cells[columnIndex];
                        var model = row[columnIndex];
                        SetCellContent(cell, model, ownerUid, isActive);
                    }
                }
            }
            else
            {
                for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    var row = data.Grid.Cells[rowIndex].ToArray().Reverse().ToArray();
                    for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
                    {
                        var cell = _rows[rowsCount - rowIndex - 1].Cells[columnIndex];
                        var model = row[columnIndex];
                        SetCellContent(cell, model, ownerUid, isActive);
                    }
                }
            }
        }

        private void SetCellContent(MainMenuGridCellComponent cell, CellModel model, string ownerUid, bool isActive)
        {
            var state = model.PlayerId.IsNullOrEmpty()
                ? GridCellState.Default
                : model.PlayerId == ownerUid
                    ? GridCellState.Owner
                    : GridCellState.Opposite;
                        
            cell.SetContent(model.Letter.ToString(), state, isActive);
        }

        private void ClickHandler() => OnClicked?.Invoke(_gameUid);

        [Serializable]
        private sealed class CellsRow
        {
            [field: SerializeField] public MainMenuGridCellComponent[] Cells { get; private set; }
        }
    }
}