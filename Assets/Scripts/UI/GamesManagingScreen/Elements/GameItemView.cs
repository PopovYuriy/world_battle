using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace UI.GamesManagingScreen.Elements
{
    public sealed class GameItemView : MonoBehaviour
    {
        private const string OwnerTurnText = "Ваш хід";
        private const string OpponentTurnText = "Хід суперника";
        private const string LocalGameTitleText = "Локальна гра з ";
        private const string OnlineGameTitleText = "Онлайн гра з ";
        
        [SerializeField] private List<CellsRow> _rows;
        [SerializeField] private TextMeshProUGUI _turnInfo;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _button;

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

        public void Initialize(GameSessionData data, GameFieldColorsConfig colorsConfig, string ownerUid, bool isLocal)
        {
            _gameUid = data.Uid;
            _turnInfo.SetText(data.LastTurnPlayerId == ownerUid ? OpponentTurnText : OwnerTurnText);
            var opponentName = data.Players.First(p => p.Uid != ownerUid).Name;
            _title.SetText($"{(isLocal ? LocalGameTitleText : OnlineGameTitleText)}{opponentName}");
            
            var playerIndex = data.Players.IndexOf(data.Players.First(p => p.Uid == ownerUid));
            var rowsCount = data.Grid.Cells.Count;
            if (playerIndex == 0)
            {
                for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    var row = data.Grid.Cells[rowIndex];
                    for (var columnIndex = 0; columnIndex < row.Count; columnIndex++)
                    {
                        var cell = _rows[rowIndex].Cells[columnIndex];
                        var model = row[columnIndex];
                        cell.color = model.PlayerId.IsNullOrEmpty()
                            ? colorsConfig.DefaultColor
                            : model.PlayerId == ownerUid
                                ? colorsConfig.OwnerColor
                                : colorsConfig.OpponentColor;
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
                        cell.color = model.PlayerId.IsNullOrEmpty()
                            ? colorsConfig.DefaultColor
                            : model.PlayerId == ownerUid
                                ? colorsConfig.OwnerColor
                                : colorsConfig.OpponentColor;
                    }
                }
            }
        }

        private void ClickHandler() => OnClicked?.Invoke(_gameUid);

        [Serializable]
        private sealed class CellsRow
        {
            [field: SerializeField] public Image[] Cells { get; private set; }
        }
    }
}