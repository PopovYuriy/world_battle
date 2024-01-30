using System;
using Core.UI.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.PickLetterPopup
{
    public sealed class PickLetterPopupView : ScreenView
    {
        [SerializeField] private LetterItem[] _letterItems;
        [SerializeField] private TextMeshProUGUI _letterToChangeField;
        [SerializeField] private TextMeshProUGUI _newLetterField;
        [SerializeField] private Button _faderButton;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _closeButton;

        public event Action<LetterItem> OnLetterPicked;
        public event Action OnConfirmClicked;
        public event Action OnCloseClicked;
        
        private void OnValidate()
        {
            _letterItems ??= GetComponentsInChildren<LetterItem>();
        }

        public override void Initialize()
        {
            foreach (var letterItem in _letterItems)
            {
                letterItem.SetPickedState(false);
                letterItem.OnClicked += LetterPickedHandler;
            }
            
            _newLetterField.SetText(string.Empty);
            
            _confirmButton.onClick.AddListener(ConfirmClicked);
            _closeButton.onClick.AddListener(CloseClicked);
            _faderButton.onClick.AddListener(CloseClicked);
        }

        public override void Dispose()
        {
            foreach (var letterItem in _letterItems)
                letterItem.OnClicked -= LetterPickedHandler;
            
            _confirmButton.onClick.RemoveListener(ConfirmClicked);
            _closeButton.onClick.RemoveListener(CloseClicked);
            _faderButton.onClick.RemoveListener(CloseClicked);
        }

        public void SetConfirmButtonVisible(bool isVisible)
        {
            _confirmButton.gameObject.SetActive(isVisible);
        }

        public void SetLetterToChange(char letter)
        {
            _letterToChangeField.SetText(letter.ToString());
            foreach (var letterItem in _letterItems)
                letterItem.gameObject.SetActive(letterItem.Letter != letter);
        }

        private void LetterPickedHandler(LetterItem letterItem)
        {
            _newLetterField.SetText(letterItem.Letter.ToString());
            OnLetterPicked?.Invoke(letterItem);
        }

        private void ConfirmClicked()
        {
            OnConfirmClicked?.Invoke();
        }

        private void CloseClicked()
        {
            OnCloseClicked?.Invoke();
        }
    }
}