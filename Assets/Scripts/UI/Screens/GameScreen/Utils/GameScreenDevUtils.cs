using System;
using System.Collections.Generic;
using System.Linq;
using App.Data.DevMode;
using App.Enums;
using App.Modules.GameSessions.Controller;
using Core.UI;
using Game.Data;
using Game.Field.Mediators;
using Game.Services;
using Game.Services.Utils;
using UnityEngine;
using Utils.Extensions;

namespace UI.Screens.GameScreen.Utils
{
    public sealed class GameScreenDevUtils : MonoBehaviour
    {
        [SerializeField] private DevModeGUIStyle _styleConfig;
        [SerializeField] private Texture _closeButtonTexture;

        private ILettersProvider _lettersProvider;
        private WordsProvider _wordsProvider;
        private IGamePlayController _gamePlayController;
        private IGameSessionController _iGameSessionController;
        private IGameTurnsProvider _turnsProvider;
        private UISystem _uiSystem;
        private bool _openDevPanel;
        private bool _openTurnsPanel;
        private string _resultWord;
        private Action _viewUpdate;

        private Vector2 _scrollPosition = Vector2.zero;
        
        private bool _isInitialized;

        private float ScreenLeftBorder => Screen.safeArea.xMin;
        private float ScreenRightBorder => Screen.safeArea.xMax;
        private float ScreenTopBorder => Screen.safeArea.yMin;
        private float ScreenBottomBorder => Screen.safeArea.yMax;

        public void Initialize(ILettersProvider lettersProvider, WordsProvider wordsProvider, IGamePlayController gamePlayController,
            IGameTurnsProvider turnsProvider, UISystem uiSystem, IGameSessionController iGameSessionController, Action viewUpdate)
        {
            _lettersProvider = lettersProvider;
            _wordsProvider = wordsProvider;
            _gamePlayController = gamePlayController;
            _turnsProvider = turnsProvider;
            _uiSystem = uiSystem;
            _iGameSessionController = iGameSessionController;
            _viewUpdate = viewUpdate;
            
            _isInitialized = true;
        }
        
        private void OnGUI()
        {
            if (!_isInitialized)
                return;

            var mainButtonRect = new Rect(new Vector2(
                ScreenRightBorder - _styleConfig.ButtonDefaultSize.x,
                ScreenBottomBorder - _styleConfig.ButtonDefaultSize.y),
                _styleConfig.ButtonDefaultSize);
            
            if (GUI.Button(mainButtonRect, "DevUtil", _styleConfig.DefaultStyle))
            {
                _openDevPanel = !_openDevPanel;
                _openTurnsPanel = false;
                _resultWord = string.Empty;
            }
            
            if (_openDevPanel)
                DrawDevPanel();
            
            if (!_resultWord.IsNullOrEmpty())
                DrawResultLabel();
            
            if (_openTurnsPanel)
                DrawTurnsPanel();
        }

        private void DrawDevPanel()
        {
            const int buttonsCount = 6;
            
            var groupWidth = _styleConfig.GroupButtonsDefaultSize.x + _styleConfig.DefaultGroupMargin.left + 
                             _styleConfig.DefaultGroupMargin.right;
            var groupHeight = _styleConfig.ButtonDefaultSize.y * buttonsCount + _styleConfig.DefaultGroupMargin.bottom + 
                              _styleConfig.DefaultGroupMargin.top + _styleConfig.DefaultGroupButtonsSpacing * (buttonsCount - 1);
            var groupRect = new Rect(ScreenRightBorder - groupWidth, ScreenBottomBorder - _styleConfig.ButtonDefaultSize.y - groupHeight -
                _styleConfig.DefaultGroupButtonsSpacing, groupWidth, groupHeight);
            
            GUI.BeginGroup(groupRect, _styleConfig.DefaultStyle);

            if (DrawGroupButton(0, "Get largest word"))
            {
                _openDevPanel = false;
                _resultWord = DetermineLargestAvailableWord();
            }
            
            if (DrawGroupButton(1, "Get shortest word"))
            {
                _openDevPanel = false;
                _resultWord = DetermineShortestAvailableWord();
            }
            
            if (DrawGroupButton(2, "Get word with apostrophe"))
            {
                _openDevPanel = false;
                _resultWord = DetermineAvailableWordWithApostrophe();
            }
            
            if (DrawGroupButton(3, "Show turns"))
            {
                _openDevPanel = false;
                _openTurnsPanel = true;
            }
            
            if (DrawGroupButton(4, "Delete game"))
            {
                _openDevPanel = false;
                _gamePlayController.DeleteGame();
                _uiSystem.ShowScreen(ScreenId.GamesManaging);
            }

            if (DrawGroupButton(5, "Add 100 points"))
            {
                _openDevPanel = false;
                _gamePlayController.CurrentPlayer.Points += 100;
                _iGameSessionController.Save();
                _viewUpdate?.Invoke();
            }
            
            GUI.EndGroup();
        }

        private bool DrawGroupButton(int order, string label)
        {
            return GUI.Button(new Rect(_styleConfig.DefaultGroupMargin.left,
                    _styleConfig.DefaultGroupMargin.top + _styleConfig.GroupButtonsDefaultSize.y * order +
                    _styleConfig.DefaultGroupButtonsSpacing * order,
                    _styleConfig.GroupButtonsDefaultSize.x,
                    _styleConfig.GroupButtonsDefaultSize.y), label,
                _styleConfig.GroupButtonsDefaultStyle);
        }

        private string DetermineLargestAvailableWord()
        {
            return GetAvailableWords().OrderByDescending(w => w.Length).First();
        }
        
        private string DetermineShortestAvailableWord()
        {
            return GetAvailableWords().OrderByDescending(w => w.Length).Last();
        }
        
        private string DetermineAvailableWordWithApostrophe()
        {
            return GetAvailableWords().FirstOrDefault(w => w.Contains('\''));
        }

        private void DrawResultLabel()
        {
            var labelRect = new Rect(0, ScreenBottomBorder - _styleConfig.ButtonDefaultSize.y,
                ScreenRightBorder - _styleConfig.ButtonDefaultSize.x, _styleConfig.ButtonDefaultSize.y);
            GUI.Label(labelRect, _resultWord, _styleConfig.DefaultStyle);
        }

        private void DrawTurnsPanel()
        {
            var groupWidth = Screen.safeArea.width / 2;
            var groupHeight = Screen.safeArea.height - _styleConfig.ButtonDefaultSize.y;
            var groupRect = new Rect(ScreenRightBorder - groupWidth, ScreenTopBorder, groupWidth, groupHeight);
            
            GUI.BeginGroup(groupRect, _styleConfig.DefaultStyle);
            var closeButtonRect = new Rect(groupWidth - _closeButtonTexture.width, 0, 70, 70);

            if (GUI.Button(closeButtonRect, _closeButtonTexture, _styleConfig.DefaultStyle))
            {
                _openTurnsPanel = false;
            }
            else
            {
                var scrollRect = new Rect(0, closeButtonRect.yMax, groupWidth - 20, groupHeight - closeButtonRect.height);
                var viewRect = new Rect(0, closeButtonRect.yMax, groupWidth - 20,
                    _turnsProvider.TurnsList.Count * _styleConfig.GroupButtonsDefaultSize.y +
                    (_turnsProvider.TurnsList.Count - 1) * _styleConfig.DefaultGroupButtonsSpacing);
                
                _scrollPosition = GUI.BeginScrollView(scrollRect, _scrollPosition, viewRect, 
                    false, true);

                var index = 0;
                var labelStyle = GUIStyle.none;
                labelStyle.alignment = TextAnchor.MiddleRight;
                labelStyle.fontSize = 45;
                labelStyle.normal = _styleConfig.GroupButtonsDefaultStyle.normal;
                labelStyle.padding = _styleConfig.DefaultContentPadding;
                
                foreach (var turn in _turnsProvider.TurnsList)
                {
                    var labelRect = new Rect(0, 
                        _styleConfig.ButtonDefaultSize.y * index + _styleConfig.DefaultGroupButtonsSpacing * index,
                        scrollRect.width, _styleConfig.ButtonDefaultSize.y);
                    GUI.Label(labelRect, turn, labelStyle);
                    index++;
                }
                
                GUI.EndScrollView();
            }
            
            GUI.EndGroup();
        }

        private IEnumerable<string> GetAvailableWords()
        {
            var letters = new List<char>(_lettersProvider.GetLettersForPlayer(_gamePlayController.CurrentPlayer.Uid));
            letters.Add('\'');
            return _wordsProvider.GetAvailableWords(letters, _turnsProvider.TurnsList);
        }
    }
}