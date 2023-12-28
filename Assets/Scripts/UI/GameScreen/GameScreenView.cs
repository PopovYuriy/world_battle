using System;
using System.Collections.Generic;
using Core.UI.Screens;
using DG.Tweening;
using Game.Data;
using Game.Field;
using TMPro;
using UI.GameScreen.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
{
    public sealed class GameScreenView : ScreenView
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _clearButton;
        [SerializeField] private Button _applyButton;
        
        [SerializeField] private TextMeshProUGUI _resultField;
        
        [SerializeField] private TextMeshProUGUI _infoField;
        [SerializeField, Range(0f, 10f)] private float _infoShowDuration;
        [SerializeField, Range(0f, 1f)] private float _infoFadeDuration;

        [SerializeField] private PlayerAreaView[] _playerAreaViews;
        [SerializeField] private GameFieldColorsConfig _colorsConfig;
        
        [field: SerializeField] public GameField GameField { get; private set; }
        [field: SerializeField] public GameScreenDevUtils DevUtils { get; private set; }
        
        private readonly Dictionary<string, PlayerAreaView> _playersMap = new(2);
        private Tween _showInfoFieldTween;

        public event Action OnBack;
        public event Action OnApply;
        public event Action OnClear;

        public override void Initialize()
        {
            _backButton.onClick.AddListener(BackButtonClickHandler);
            _applyButton.onClick.AddListener(ApplyButtonClickHandler);
            _clearButton.onClick.AddListener(ClearButtonClickHandler);
            _infoField.gameObject.SetActive(false);
            
            _playerAreaViews[0].HideLasWord();
            _playerAreaViews[1].HideLasWord();
        }

        public override void Dispose()
        {
            _backButton.onClick.RemoveListener(BackButtonClickHandler);
            _applyButton.onClick.RemoveListener(ApplyButtonClickHandler);
            _clearButton.onClick.RemoveListener(ClearButtonClickHandler);
            
            _showInfoFieldTween?.Kill();
        }

        public void SetPlayers(IReadOnlyList<PlayerGameData> players)
        {
            const string ownerPlayerNick = "Ви";
            var firstPlayerArea = _playerAreaViews[0];
            _playersMap[players[0].Uid] = firstPlayerArea;
            firstPlayerArea.Initialize(ownerPlayerNick, _colorsConfig.OwnerColor);
            
            var secondPlayerArea = _playerAreaViews[1];
            _playersMap[players[1].Uid] = secondPlayerArea;
            secondPlayerArea.Initialize(players[1].Name, _colorsConfig.OpponentColor);
        }

        public void SetCurrentPlayer(string uid)
        {
            foreach (var playerId in _playersMap.Keys)
                _playersMap[playerId].SetActive(playerId == uid);
        }

        public void ClearResult()
        {
            _resultField.text = string.Empty;
        }

        public void ApplyToResult(char letter)
        {
            _resultField.text += letter;
        }

        public void ShowInfoField(string message)
        {
            _showInfoFieldTween?.Kill();
            
            _infoField.gameObject.SetActive(true);
            _infoField.text = message;
            _infoField.alpha = 0f;
            _showInfoFieldTween = DOTween.Sequence()
                .Append(_infoField.DOFade(1f, _infoFadeDuration))
                .AppendInterval(_infoShowDuration)
                .Append(_infoField.DOFade(0f, _infoFadeDuration))
                .OnComplete(() => _infoField.gameObject.SetActive(false));
        }

        public void SetButtonsVisible(bool isVisible)
        {
            _applyButton.gameObject.SetActive(isVisible);
            _clearButton.gameObject.SetActive(isVisible);
        }

        public void ShowLastTurn(string playerUid, string word)
        {
            foreach (var uid in _playersMap.Keys)
            {
                if (uid == playerUid)
                    _playersMap[uid].ShowLastWord(word);
                else
                    _playersMap[uid].HideLasWord();
            }
        }

        private void BackButtonClickHandler() => OnBack?.Invoke();
        private void ApplyButtonClickHandler() => OnApply?.Invoke();
        private void ClearButtonClickHandler() => OnClear?.Invoke();
    }
}