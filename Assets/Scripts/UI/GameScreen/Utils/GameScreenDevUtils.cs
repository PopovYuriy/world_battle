using System.Collections.Generic;
using System.Linq;
using App.Data.DevMode;
using Game.Data;
using Game.Field.Mediators;
using Game.Services;
using Game.Services.Utils;
using UnityEngine;
using Utils.Extensions;

namespace UI.GameScreen.Utils
{
    public sealed class GameScreenDevUtils : MonoBehaviour
    {
        [SerializeField] private DevModeGUIStyle _styleConfig;
        [SerializeField] private Texture _closeButtonTexture;

        private ILettersProvider _lettersProvider;
        private WordsProvider _wordsProvider;
        private IGameMediator _gameMediator;
        private IGameTurnsProvider _turnsProvider;
        private bool _openDevPanel;
        private bool _openTurnsPanel;
        private string _resultWord;

        private Vector2 _scrollPosition = Vector2.zero;
        
        private bool _isInitialized;

        public void Initialize(ILettersProvider lettersProvider, WordsProvider wordsProvider, IGameMediator gameMediator,
            IGameTurnsProvider turnsProvider)
        {
            _lettersProvider = lettersProvider;
            _wordsProvider = wordsProvider;
            _gameMediator = gameMediator;
            _turnsProvider = turnsProvider;
            
            _isInitialized = true;
        }
        
        private void OnGUI()
        {
            if (!_isInitialized)
                return;

            var mainButtonRect = new Rect(new Vector2(
                Screen.width - _styleConfig.ButtonDefaultSize.x,
                Screen.height - _styleConfig.ButtonDefaultSize.y),
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
            const int buttonsCount = 2;
            
            var groupWidth = _styleConfig.GroupButtonsDefaultSize.x + _styleConfig.DefaultGroupMargin.left + 
                             _styleConfig.DefaultGroupMargin.right;
            var groupHeight = _styleConfig.ButtonDefaultSize.y * buttonsCount + _styleConfig.DefaultGroupMargin.bottom + 
                              _styleConfig.DefaultGroupMargin.top + _styleConfig.DefaultGroupButtonsSpacing * (buttonsCount - 1);
            var groupRect = new Rect(Screen.width - groupWidth, Screen.height - _styleConfig.ButtonDefaultSize.y - groupHeight -
                _styleConfig.DefaultGroupButtonsSpacing, groupWidth, groupHeight);
            
            GUI.BeginGroup(groupRect, _styleConfig.DefaultStyle);

            if (DrawGroupButton(0, "Get word"))
            {
                _openDevPanel = false;
                _resultWord = DetermineAvailableWord();
            }
            
            if (DrawGroupButton(1, "Show turns"))
            {
                _openDevPanel = false;
                _openTurnsPanel = true;
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

        private string DetermineAvailableWord()
        {
            var letters = _lettersProvider.GetLettersForPlayer(_gameMediator.CurrentPlayer.Uid);
            var words = _wordsProvider.GetAvailableWords(letters, _turnsProvider.TurnsList);
            return words.First();
        }

        private void DrawResultLabel()
        {
            var labelRect = new Rect(0, Screen.height - _styleConfig.ButtonDefaultSize.y,
                Screen.width - _styleConfig.ButtonDefaultSize.x, _styleConfig.ButtonDefaultSize.y);
            GUI.Label(labelRect, _resultWord, _styleConfig.DefaultStyle);
        }

        private void DrawTurnsPanel()
        {
            var groupWidth = Screen.width >> 1;
            var groupHeight = Screen.height - _styleConfig.ButtonDefaultSize.y;
            var groupRect = new Rect(Screen.width - groupWidth, 0, groupWidth, groupHeight);
            
            GUI.BeginGroup(groupRect, _styleConfig.DefaultStyle);
            var closeButtonRect = new Rect(groupWidth - _closeButtonTexture.width, 0, 70, 70);

            if (GUI.Button(closeButtonRect, _closeButtonTexture, _styleConfig.DefaultStyle))
            {
                _openTurnsPanel = false;
            }
            else
            {
                var scrollRect = new Rect(0, closeButtonRect.yMax, groupWidth, groupHeight - closeButtonRect.height);
                var viewRect = new Rect(0, closeButtonRect.yMax, groupWidth,
                    _turnsProvider.TurnsList.Count * _styleConfig.GroupButtonsDefaultSize.y +
                    (_turnsProvider.TurnsList.Count - 1) * _styleConfig.DefaultGroupButtonsSpacing);
                
                _scrollPosition = GUI.BeginScrollView(scrollRect, _scrollPosition, viewRect, 
                    false, false);

                var index = 0;
                var labelStyle = GUIStyle.none;
                labelStyle.alignment = TextAnchor.MiddleRight;
                labelStyle.fontSize = 45;
                labelStyle.normal = _styleConfig.GroupButtonsDefaultStyle.normal;
                
                foreach (var turn in _turnsProvider.TurnsList)
                {
                    var labelRect = new Rect(0, 
                        _styleConfig.ButtonDefaultSize.y * index + _styleConfig.DefaultGroupButtonsSpacing * index,
                        scrollRect.width - 40, _styleConfig.ButtonDefaultSize.y);
                    GUI.Label(labelRect, turn, labelStyle);
                    index++;
                }
                
                GUI.EndScrollView();
            }
            
            
            
            GUI.EndGroup();
        }
    }
}