using System;
using System.Collections.Generic;
using Core.UI.Components;
using Core.UI.Screens;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace UI.Popups.GameSettingsPopup
{
    public sealed class GameSettingsPanelView : ScreenView
    {
        [SerializeField] private Button _faderButton;
        [SerializeField] private TextButton _giveUpButton;
        [SerializeField] private Button _declareDefeatButton;
        
        [SerializeField] private Transform _wordsContainer;
        [SerializeField] private WordItem _ownWordPrefab;
        [SerializeField] private WordItem _opposedWordPrefab;
        [SerializeField] private CanvasGroup _contentCanvasGroup;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private ScrollRect _scrollRect;

        [Header("Animation settings:")]
        [SerializeField] private float _hidedXPosition;
        [SerializeField] private float _shownXPosition;
        [SerializeField] private float _duration;

        private Tween _tween;

        public event Action OnCloseClicked;
        public event Action OnGiveUpClicked; 
        public event Action OnDeclareDefeatClicked;

        public override void Initialize()
        {
            _faderButton.onClick.AddListener(CloseClickHandler);
            _giveUpButton.onClick.AddListener(GiveUpClickHandler);
            _declareDefeatButton.onClick.AddListener(DeclareDefeatClickHandler);
            _contentCanvasGroup.interactable = false;
        }

        public override void Dispose()
        {
            _tween?.Kill();
            _faderButton.onClick.RemoveListener(CloseClickHandler);
            _giveUpButton.onClick.RemoveListener(GiveUpClickHandler);
            _declareDefeatButton.onClick.RemoveListener(DeclareDefeatClickHandler);
        }

        public void SetWordsList(IEnumerable<string> words, bool isOwnersFirstTurn)
        {
            var ownerTurn = isOwnersFirstTurn;
            foreach (var word in words)
            {
                CreateWord(word, ownerTurn);
                ownerTurn = !ownerTurn;
            }
        }

        public void AddWord(string word, bool isOwnerTurn)
        {
            CreateWord(word, isOwnerTurn);
            _scrollRect.verticalNormalizedPosition = 0;
        }

        public void SetGiveUpButtonInteractable(bool isInteractable)
        {
            _giveUpButton.interactable = isInteractable;
        }
        
        public void SetDeclareDefeatButtonInteractable(bool isInteractable)
        {
            _declareDefeatButton.interactable = isInteractable;
        }

        public void SetGiveUpButtonLabel(string label)
        {
            _giveUpButton.SetLabelText(label);
        }
        
        public void PlayShowAnimation()
        {
            _tween?.Kill();
            _contentTransform.localPosition = new Vector2(_hidedXPosition, _contentTransform.localPosition.y);
            gameObject.SetActive(true);
            _contentCanvasGroup.interactable = false;
            _scrollRect.verticalNormalizedPosition = 0;

            var targetPosition = new Vector2(_shownXPosition, _contentTransform.localPosition.y);
            _tween = _contentTransform.DOLocalMove(targetPosition, _duration)
                .OnComplete(() => { _contentCanvasGroup.interactable = true; });
        }

        public void PlayHideAnimation(Action onComplete)
        {
            _tween?.Kill();
            
            _contentCanvasGroup.interactable = false;

            var targetPosition = new Vector2(_hidedXPosition, _contentTransform.localPosition.y);
            _tween = _contentTransform.DOLocalMove(targetPosition, _duration)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void SetDeclareDefeatButtonVisible(bool isVisible)
        {
            _declareDefeatButton.gameObject.SetActive(isVisible);
        }

        private void CreateWord(string word, bool isOwnerTurn)
        {
            var wordItemPrefab = isOwnerTurn ? _ownWordPrefab : _opposedWordPrefab;
            var wordItem = Instantiate(wordItemPrefab, _wordsContainer);
            wordItem.SetWord(word);
        }

        private void CloseClickHandler() => OnCloseClicked?.Invoke();
        private void GiveUpClickHandler() => OnGiveUpClicked?.Invoke();
        private void DeclareDefeatClickHandler() => OnDeclareDefeatClicked?.Invoke();
    }
}