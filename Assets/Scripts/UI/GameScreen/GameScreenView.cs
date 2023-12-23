using System;
using Core.UI.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
{
    public sealed class GameScreenView : ScreenView
    {
        [SerializeField] private Button _backButton;
        [field: SerializeField] public Transform GameFieldPlaceholder { get; private set; }

        public event Action OnBackClicked;

        public override void Initialize()
        {
            _backButton.onClick.AddListener(BackButtonClickHandler);
        }

        public override void Dispose()
        {
            _backButton.onClick.RemoveListener(BackButtonClickHandler);
        }

        private void BackButtonClickHandler() => OnBackClicked?.Invoke();
    }
}