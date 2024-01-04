using System;
using Core.UI.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace UI.Popups.EnterNamePopup
{
    public sealed class EnterNamePopupView : ScreenView
    {
        [SerializeField] private TextMeshProUGUI _defaultNameField;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _okButton;

        public event Action<string> OnNameConfirmed; 
        
        public override void Initialize()
        {
            _okButton.onClick.AddListener(OkButtonClickHandler);
        }

        public override void Dispose()
        {
            _okButton.onClick.RemoveListener(OkButtonClickHandler);
        }

        public void SetDefaultName(string defaultName)
        {
            _defaultNameField.SetText(defaultName);
        }

        private void OkButtonClickHandler()
        {
            var typedName = _inputField.text;
            OnNameConfirmed?.Invoke(typedName.IsNullOrEmpty() ? _defaultNameField.text : typedName);
        }
    }
}