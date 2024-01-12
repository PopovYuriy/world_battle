using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace UI.GameScreen.SettingsPopup
{
    public sealed class SettingsPanelView : MonoBehaviour
    {
        [SerializeField] private Transform _wordsContainer;
        [SerializeField] private WordItem _ownWordPrefab;
        [SerializeField] private WordItem _opposedWordPrefab;
        [SerializeField] private Button _faderButton;
        [SerializeField] private CanvasGroup _contentCanvasGroup;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private ScrollRect _scrollRect;

        [Header("Animation settings:")]
        [SerializeField] private float _hidedXPosition;
        [SerializeField] private float _shownXPosition;
        [SerializeField] private float _duration;

        private Tween _tween;
        
        private void Start()
        {
            _faderButton.onClick.AddListener(CloseClickHandler);
            _contentCanvasGroup.interactable = false;
        }

        private void OnDestroy()
        {
            _tween?.Kill();
            _faderButton.onClick.RemoveListener(CloseClickHandler);
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
        
        public void Show()
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

        private void Hide()
        {
            _tween?.Kill();
            
            _contentCanvasGroup.interactable = false;

            var targetPosition = new Vector2(_hidedXPosition, _contentTransform.localPosition.y);
            _tween = _contentTransform.DOLocalMove(targetPosition, _duration)
                .OnComplete(() => { gameObject.SetActive(false); });
        }

        private void CreateWord(string word, bool isOwnerTurn)
        {
            var wordItemPrefab = isOwnerTurn ? _ownWordPrefab : _opposedWordPrefab;
            var wordItem = Instantiate(wordItemPrefab, _wordsContainer);
            wordItem.SetWord(word);
        }

        private void CloseClickHandler()
        {
            Hide();
        }
    }
}