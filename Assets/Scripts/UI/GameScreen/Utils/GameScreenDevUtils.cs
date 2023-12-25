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

        private ILettersProvider _lettersProvider;
        private WordsProvider _wordsProvider;
        private IGameMediator _gameMediator;
        private IGameTurnsProvider _turnsProvider;
        private bool _openDevPanel;
        private string _resultWord;
        
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
            }
            
            if (_openDevPanel)
                DrawDevPanel();
            
            if (!_resultWord.IsNullOrEmpty())
                DrawResultLabel();
        }

        private void DrawDevPanel()
        {
            var groupWidth = _styleConfig.GroupButtonsDefaultSize.x + _styleConfig.DefaultGroupMargin.left + 
                             _styleConfig.DefaultGroupMargin.right;
            var groupHeight = _styleConfig.ButtonDefaultSize.y + _styleConfig.DefaultGroupMargin.bottom + 
                              _styleConfig.DefaultGroupMargin.top;
            var groupRect = new Rect(Screen.width - groupWidth, Screen.height - _styleConfig.ButtonDefaultSize.y - groupHeight,
                groupWidth, groupHeight);
            
            GUI.BeginGroup(groupRect, _styleConfig.DefaultStyle);

            if (GUI.Button(new Rect(_styleConfig.DefaultGroupMargin.left, _styleConfig.DefaultGroupMargin.top,
                _styleConfig.GroupButtonsDefaultSize.x, _styleConfig.GroupButtonsDefaultSize.y), "Get available word",
                _styleConfig.GroupButtonsDefaultStyle))
            {
                _openDevPanel = false;
                _resultWord = DetermineAvailableWord();
            }
            
            GUI.EndGroup();
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
    }
}