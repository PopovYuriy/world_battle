using System;
using Core.UI.Components;
using Core.UI.Screens;
using TMPro;
using UnityEngine;

namespace UI.Popups.ConfirmationPopup
{
    public sealed class ConfirmationPopupView : ScreenView
    {
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private GameObject _singleButtonContainer;
        [SerializeField] private TextButton _okButton;

        [SerializeField] private GameObject _twoButtonsContainer;
        [SerializeField] private TextButton _confirmButton;
        [SerializeField] private TextButton _declineButton;

        public event Action OnConfirmClicked;
        public event Action OnDeclineClicked;

        public override void Initialize()
        {
            _okButton.onClick.AddListener(ConfirmClickedHandler);
            _confirmButton.onClick.AddListener(ConfirmClickedHandler);
            _declineButton.onClick.AddListener(DeclineClickedHandler);
        }

        public override void Dispose()
        {
            _okButton.onClick.RemoveListener(ConfirmClickedHandler);
            _confirmButton.onClick.RemoveListener(ConfirmClickedHandler);
            _declineButton.onClick.RemoveListener(DeclineClickedHandler);
        }
        
        public void SetConfirmationButtonLabel(string label)
        {
            _okButton.SetLabelText(label);
            _confirmButton.SetLabelText(label);
        }

        public void SetDeclineButtonLabel(string label) => _declineButton.SetLabelText(label);
        public void SetHeader(string header) => _header.SetText(header);
        public void SetText(string text) => _text.SetText(text);

        public void SetType(ConfirmationPopupType type)
        {
            switch (type)
            {
                case ConfirmationPopupType.SingleButton:
                    _singleButtonContainer.SetActive(true);
                    _twoButtonsContainer.SetActive(false);
                    break;
                
                case ConfirmationPopupType.TwoButtons:
                    _singleButtonContainer.SetActive(false);
                    _twoButtonsContainer.SetActive(true);
                    break;
                
                default:
                    _singleButtonContainer.SetActive(false);
                    _twoButtonsContainer.SetActive(false);
                    Debug.LogError("Invalid confirmation popup type");
                    break;
            }
        }

        private void ConfirmClickedHandler() => OnConfirmClicked?.Invoke();
        private void DeclineClickedHandler() => OnDeclineClicked?.Invoke();
    }
}