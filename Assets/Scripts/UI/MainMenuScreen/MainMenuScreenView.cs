using System;
using System.Collections.Generic;
using System.Linq;
using Core.UI.Screens;
using Game.Data;
using TMPro;
using UI.MainMenuScreen.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenuScreen
{
    public sealed class MainMenuScreenView : ScreenView
    {
        [SerializeField] private Button                _newGameButton;
        [SerializeField] private Button                _localGamesButton;
        [SerializeField] private Button                _onlineGamesButton;
        [SerializeField] private TextMeshProUGUI       _localGamesTabLabel;
        [SerializeField] private TextMeshProUGUI       _onlineGamesTabLabel;
        [SerializeField] private RectTransform         _itemsContainer;
        [SerializeField] private MainMenuItemComponent _itemPrefab;

        private List<MainMenuItemComponent> _gameItems;

        public event Action<string>        OnGameSelected;
        public event Action                OnNewGameClicked;
        public event Action<MainMenuTabId> OnTabClicked;

        public override void Initialize()
        {
            _newGameButton.onClick.AddListener(StartGameClickHandler);
            _localGamesButton.onClick.AddListener(LocalTabClickHandler);
            _onlineGamesButton.onClick.AddListener(OnlineTabClickHandler);
            
            _gameItems = new List<MainMenuItemComponent>();
        }

        public override void Dispose()
        {
            _newGameButton.onClick.RemoveListener(StartGameClickHandler);
            _localGamesButton.onClick.RemoveListener(LocalTabClickHandler);
            _onlineGamesButton.onClick.RemoveListener(OnlineTabClickHandler);
        }

        public void SetGames(MainMenuTabId tabId, IReadOnlyCollection<GameSessionData> games, string ownerUid)
        {
            var gameIndex = 0;

            if (games != null)
            {
                foreach (var gameSessionData in games.OrderByDescending(g => g.LastTurnPlayerId == ownerUid ? 0 : 1))
                {
                    if (gameIndex < _gameItems.Count)
                    {
                        _gameItems[gameIndex].Initialize(gameSessionData, ownerUid);
                        _gameItems[gameIndex].gameObject.SetActive(true);
                    }
                    else
                    {
                        var gameItemView = Instantiate(_itemPrefab, _itemsContainer);
                        gameItemView.Initialize(gameSessionData, ownerUid);
                        gameItemView.OnClicked += GameSelectedHandler;
                        _gameItems.Add(gameItemView);
                    }

                    gameIndex++;
                }
            }

            for (var i = gameIndex; i < _gameItems.Count; i++)
                _gameItems[i].gameObject.SetActive(false);

            SetActiveTab(tabId);
        }

        private void SetActiveTab(MainMenuTabId tabId)
        {
            _localGamesButton.interactable = tabId == MainMenuTabId.Online;
            _onlineGamesButton.interactable = tabId == MainMenuTabId.Local;

            _localGamesTabLabel.fontStyle = tabId == MainMenuTabId.Local ? FontStyles.Underline : FontStyles.Normal;
            _onlineGamesTabLabel.fontStyle = tabId == MainMenuTabId.Online ? FontStyles.Underline : FontStyles.Normal;
        }

        private void GameSelectedHandler(string gameUid) => OnGameSelected?.Invoke(gameUid);
        private void StartGameClickHandler() => OnNewGameClicked?.Invoke();
        private void LocalTabClickHandler()  => OnTabClicked?.Invoke(MainMenuTabId.Local);
        private void OnlineTabClickHandler() => OnTabClicked?.Invoke(MainMenuTabId.Online);
    }
}