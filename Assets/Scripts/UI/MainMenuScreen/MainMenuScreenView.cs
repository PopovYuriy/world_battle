using System;
using Core.UI.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenuScreen
{
    public sealed class MainMenuScreenView : ScreenView
    {
        [SerializeField] private Button _startGameButton;

        public event Action OnStartClicked;
        
        public override void Initialize()
        {
            _startGameButton.onClick.AddListener(StartGameClickHandler);
        }

        public override void Dispose() 
        {
            _startGameButton.onClick.RemoveListener(StartGameClickHandler);
        }

        private void StartGameClickHandler() => OnStartClicked?.Invoke();
    }
}