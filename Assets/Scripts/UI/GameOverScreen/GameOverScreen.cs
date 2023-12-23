using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameOverScreen
{
    public sealed class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private Button _gameReloadButton;

        public event Action OnReloadClicked;
        
        private void Awake() => _gameReloadButton.onClick.AddListener(GameReloadButtonClickHandler);

        private void OnDestroy() => _gameReloadButton.onClick.RemoveListener(GameReloadButtonClickHandler);

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

        public void SetPlayerName(string playerName) => _playerName.SetText(playerName);

        private void GameReloadButtonClickHandler() => OnReloadClicked?.Invoke();
    }
}