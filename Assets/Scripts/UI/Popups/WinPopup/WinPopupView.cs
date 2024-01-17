using System;
using Core.UI.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.WinPopup
{
    public sealed class WinPopupView : ScreenView
    {
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _okButton;

        public event Action OnOkClicked;
        
        public override void Initialize()
        {
            _okButton.onClick.AddListener(OkClickHandler);
        }

        public override void Dispose()
        {
            _okButton.onClick.RemoveListener(OkClickHandler);
        }

        public void SetText(string text)
        {
            // _playerName.SetText(text);
        }

        private void OkClickHandler()
        {
            OnOkClicked?.Invoke();
        }
    }
}