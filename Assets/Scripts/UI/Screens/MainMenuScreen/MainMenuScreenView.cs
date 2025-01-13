using System;
using System.Collections.Generic;
using System.Linq;
using App.Modules.GameSessions.Data;
using Core.UI.Screens;
using TMPro;
using UI.Components.Controls.Switcher;
using UI.Screens.MainMenuScreen.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.MainMenuScreen
{
    public sealed class MainMenuScreenView : ScreenView
    {
        [SerializeField] private Button                _newGameButton;
        [SerializeField] private Switcher         _switcher;
        [SerializeField] private RectTransform         _itemsContainer;
        [SerializeField] private MainMenuItemComponent _itemPrefab;

        private List<MainMenuItemComponent> _gameItems;

        public event Action<string>        OnGameSelected;
        public event Action                OnNewGameClicked;
        public event Action<MainMenuTabId> OnTabClicked;

        public override void Initialize()
        {
            _newGameButton.onClick.AddListener(StartGameClickHandler);
            _switcher.OnSwitchStarted += SwitchStartHandler;

            _gameItems = new List<MainMenuItemComponent>();
        }

        public override void Dispose()
        {
            _newGameButton.onClick.RemoveListener(StartGameClickHandler);
            _switcher.OnSwitchStarted -= SwitchStartHandler;
        }

        public void SetInitialTabId(MainMenuTabId tabId)
        {
            _switcher.SetActiveItemIndex(ConvertIdToIndex(tabId));
        }

        public void SetGames(IReadOnlyCollection<GameSessionData> games, string ownerUid)
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
        }
        
        private void SwitchStartHandler(int activeGroup)
        {
            OnTabClicked?.Invoke(ConvertIndexToId(activeGroup));
        }

        private void SetActiveTab(MainMenuTabId tabId)
        {
            _switcher.SetActiveItemIndex(ConvertIdToIndex(tabId));
        }

        private void GameSelectedHandler(string gameUid) => OnGameSelected?.Invoke(gameUid);
        private void StartGameClickHandler()             => OnNewGameClicked?.Invoke();

        private MainMenuTabId ConvertIndexToId(int index)
        {
            return index switch
            {
                0 => MainMenuTabId.Local,
                1 => MainMenuTabId.Online,
                _ => MainMenuTabId.Online
            };
        }
        
        private int ConvertIdToIndex(MainMenuTabId id)
        {
            return id switch
            {
                MainMenuTabId.Local => 0,
                MainMenuTabId.Online => 1,
                _ => 1
            };
        }
    }
}