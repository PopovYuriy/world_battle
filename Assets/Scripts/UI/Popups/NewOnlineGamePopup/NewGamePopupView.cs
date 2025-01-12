using System;
using Core.UI.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.NewOnlineGamePopup
{
    public sealed class NewGamePopupView : ScreenView
    {
        [SerializeField] private Button _fader;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _createOnlineGameButton;
        [SerializeField] private Button _findOnlineGameButton;
        [SerializeField] private Button _createLocalGameButton;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private GameObject _pendingState;
        
        [SerializeField] private TextMeshProUGUI _infoField;
        [SerializeField] private float _infoShowDuration;
        
        public string InputText => _inputField.text;

        public event Action OnCloseClick;
        public event Action OnCreateOnlineClick;
        public event Action OnFindOnlineClick;
        public event Action OnCreateLocalClick;
        
        public override void Initialize()
        {
            _fader.onClick.AddListener(CloseButtonClickHandler);
            _closeButton.onClick.AddListener(CloseButtonClickHandler);
            _createOnlineGameButton.onClick.AddListener(CreateOnlineButtonClickHandler);
            _findOnlineGameButton.onClick.AddListener(FindButtonClickHandler);
            _createLocalGameButton.onClick.AddListener(CreateLocalClickHandler);
            _pendingState.SetActive(false);
        }

        public override void Dispose()
        {
            _fader.onClick.RemoveListener(CloseButtonClickHandler);
            _closeButton.onClick.RemoveListener(CloseButtonClickHandler);
            _createOnlineGameButton.onClick.RemoveListener(CreateOnlineButtonClickHandler);
            _createLocalGameButton.onClick.RemoveListener(CreateLocalClickHandler);
            _findOnlineGameButton.onClick.RemoveListener(FindButtonClickHandler);
        }

        public void SetButtonsInteractable(bool isInteractable)
        {
            _findOnlineGameButton.interactable = isInteractable;
            _createOnlineGameButton.interactable = isInteractable;
        }

        public void ShowPendingState() => _pendingState.SetActive(true);
        public void HidePendingState() => _pendingState.SetActive(false);

        public void ShowInfoText(string text)
        {
            Debug.LogWarning($"SecretWord must be more than 5 symbols and less than 15 symbols");
        }

        private void CloseButtonClickHandler() => OnCloseClick?.Invoke();

        private void CreateOnlineButtonClickHandler() => OnCreateOnlineClick?.Invoke();

        private void FindButtonClickHandler() => OnFindOnlineClick?.Invoke();

        private void CreateLocalClickHandler() => OnCreateLocalClick?.Invoke();
    }
}