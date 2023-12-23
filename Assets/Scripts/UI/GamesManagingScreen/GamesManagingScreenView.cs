using System;
using System.Collections.Generic;
using Core.UI.Screens;
using Game.Data;
using TMPro;
using UI.GamesManagingScreen.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GamesManagingScreen
{
    public sealed class GamesManagingScreenView : ScreenView
    {
        [SerializeField] private GameItemView _gameItemViewPrefab;
        [SerializeField] private Transform _gamesListContainer;
        [SerializeField] private TextMeshProUGUI _noGamesInfoTF;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _newGameButton;
        
        private List<GameItemView> _gameItems;

        public event Action<string> OnGameSelected;
        public event Action OnBackClicked;
        public event Action OnNewGameClicked;

        public override void Initialize()
        {
            _backButton.onClick.AddListener(BackClickedHandler);
            _newGameButton.onClick.AddListener(NewGameClickedHandler);
        }

        public override void Dispose()
        {
            _backButton.onClick.RemoveListener(BackClickedHandler);
            _newGameButton.onClick.RemoveListener(NewGameClickedHandler);
            
            if (_gameItems == null)
                return;

            foreach (var gameItemView in _gameItems)
                gameItemView.OnClicked -= GameSelectedHandler;
        }
        
        public void SetNoGamesInfoVisible(bool isVisible) => _noGamesInfoTF.gameObject.SetActive(isVisible);

        public void SetGames(GameSessionData[] games, GameFieldColorsConfig colorConfig, string ownerUid, bool isLocal)
        {
            _gameItems ??= new List<GameItemView>();
            
            var gameItemViews = new List<GameItemView>();
            foreach (var gameSessionData in games)
            {
                var gameItemView = Instantiate(_gameItemViewPrefab, _gamesListContainer);
                gameItemView.Initialize(gameSessionData, colorConfig, ownerUid, isLocal);
                gameItemViews.Add(gameItemView);
                gameItemView.OnClicked += GameSelectedHandler;
                _gameItems.Add(gameItemView);
            }
        }
        
        private void GameSelectedHandler(string gameUid) => OnGameSelected?.Invoke(gameUid);

        private void BackClickedHandler() => OnBackClicked?.Invoke();
        
        private void NewGameClickedHandler() => OnNewGameClicked?.Invoke();
    }
}