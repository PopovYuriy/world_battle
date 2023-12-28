using System;
using Core.UI.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.WinPopup
{
    public sealed class WinPopupView : ScreenView
    {
        [SerializeField] private TextMeshProUGUI _playerName;
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

        public void SetPlayerName(string name)
        {
            _playerName.SetText(name);
        }

        private void OkClickHandler()
        {
            OnOkClicked?.Invoke();
        }
    }
}