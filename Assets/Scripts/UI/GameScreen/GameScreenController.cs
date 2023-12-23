using System.Linq;
using App.Data.Player;
using Core.UI;
using Core.UI.Enums;
using Core.UI.Screens;
using Game.Data;
using Game.Field.Mediators;
using Game.Services;
using UI.GameScreen.Data;
using UI.GameScreen.Validator;
using UnityEngine;
using Zenject;

namespace UI.GameScreen
{
    public sealed class GameScreenController : ScreenControllerAbstract<GameScreenView, GameScreenData>
    {
        private const string NotFoundWordMessage = "Слово не знайдено у словнику";
        private const string AlreadyUsedWordMessage = "Слово вже використовувалось";
        
        private UISystem _uiSystem;
        private GameFieldColorsConfig _colorsConfig;
        private IPlayer _player;
        
        private IGameMediator _gameMediator;
        private WordValidator _wordValidator;

        [Inject]
        private void Construct(UISystem uiSystem, GameFieldColorsConfig colorsConfig, IPlayer player)
        {
            _uiSystem = uiSystem;
            _colorsConfig = colorsConfig;
            _player = player;
        }
        
        public override void Initialize()
        {
            var letters = Data.GameSessionStorage.Data.Grid.Cells
                .SelectMany(list => list.Select(c => c.Letter)).ToArray();
            var wordsProvider = new WordsProvider();
            wordsProvider.Initialize(letters);
            
            View.GameField.Initialize(Data.GameSessionStorage.Data.Players);
            _gameMediator = Data.GameMediator;
            _gameMediator.OnLetterPicked += LetterPickHandler;
            _gameMediator.Initialize(View.GameField, wordsProvider, Data.GameSessionStorage, _colorsConfig, _player.Uid);

            _wordValidator = new WordValidator(Data.GameSessionStorage.Data.Turns, wordsProvider);

            View.OnBack += BackClickHandler;
            View.OnApply += ApplyClickHandler;
            View.OnClear += ClearClickHandler;
            View.Initialize();
            View.ClearResult();
            View.SetButtonsVisible(false);
            View.SetPlayers(_gameMediator.GetOrderedPlayersList());
            View.SetCurrentPlayer(_gameMediator.CurrentPlayer.Uid);
        }

        public override void Dispose()
        {
            _gameMediator.OnLetterPicked -= LetterPickHandler;
            
            View.OnBack -= BackClickHandler;
            View.OnApply -= ApplyClickHandler;
            View.OnClear -= ClearClickHandler;
        }

        public override void Show()
        {
            View.Show();
        }

        public override void Close()
        {
            Dispose();
            View.Hide();
            Object.Destroy(View.gameObject);
        }
        
        private void LetterPickHandler(char letter)
        {
            View.ApplyToResult(letter);
            View.SetButtonsVisible(true);
        }

        private void BackClickHandler()
        {
            _uiSystem.ShowScreen(ScreenId.MainMenu);
        }
        
        private void ApplyClickHandler()
        {
            View.ClearResult();
            View.SetButtonsVisible(false);
            
            var validationResult = _wordValidator.Validate(_gameMediator.CurrentWord);
            switch (validationResult)
            {
                case ValidationResultType.Valid:
                    _gameMediator.ApplyCurrentWord();
                    View.SetCurrentPlayer(_gameMediator.CurrentPlayer.Uid);
                    break;
                
                case ValidationResultType.AlreadyUsed:
                    _gameMediator.ClearCurrentWord();
                    View.ShowInfoField(AlreadyUsedWordMessage);
                    break;
                
                case ValidationResultType.NotFoundInVocabulary:
                    _gameMediator.ClearCurrentWord();
                    View.ShowInfoField(NotFoundWordMessage);
                    break;
                
                default:
                    _gameMediator.ClearCurrentWord();
                    Debug.LogWarning("Invalid validation result");
                    break;
            }
        }
        
        private void ClearClickHandler()
        {
            View.ClearResult();
            View.SetButtonsVisible(false);
            _gameMediator.ClearCurrentWord();
        }
    }
}