using System.Linq;
using App.Data.Player;
using App.Enums;
using App.Services;
using Core.UI;
using Core.UI.Screens;
using Game.Data;
using Game.Field.Mediators;
using Game.Services;
using Game.Services.Storage;
using Game.Services.Utils;
using Tools.CSharp;
using UI.GameScreen.Data;
using UI.GameScreen.Validator;
using UI.Popups.ConfirmationPopup;
using UI.Popups.GameSettingsPopup;
using UI.Popups.WinPopup;
using UnityEngine;
using Utils.Extensions;
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
        private GameSessionsManager _gameSessionsManager;
        
        private IGameMediator _gameMediator;
        private WordValidator _wordValidator;
        private AvailableLettersProvider _availableLettersProvider;

        [Inject]
        private void Construct(UISystem uiSystem, GameFieldColorsConfig colorsConfig, IPlayer player, GameSessionsManager gameSessionsManager)
        {
            _uiSystem = uiSystem;
            _colorsConfig = colorsConfig;
            _player = player;
            _gameSessionsManager = gameSessionsManager;
        }
        
        public override void Initialize()
        {
            var letters = Data.GameSessionStorage.Data.Grid.Cells
                .SelectMany(list => list.Select(c => c.Letter)).ToArray();
            var wordsProvider = new WordsProvider();
            wordsProvider.Initialize(letters);
            
            View.GameField.Initialize(Data.GameSessionStorage.Data.Players);
            _gameMediator = Data.GameMediator;
            _gameMediator.OnWordChanged += WordChangedHandler;
            _gameMediator.OnStorageUpdated += StorageUpdatedHandler;
            _gameMediator.OnWin += WinHandler;
            _gameMediator.Initialize(View.GameField, Data.GameSessionStorage, _colorsConfig, _player.Uid);
            
            Data.GameSessionStorage.SurrenderDataUpdated += SurrenderDataUpdated;

            _wordValidator = new WordValidator(Data.GameSessionStorage.Data, wordsProvider);
            _availableLettersProvider = new AvailableLettersProvider(View.GameField);

            View.OnBack += BackClickHandler;
            View.OnApply += ApplyClickHandler;
            View.OnClear += ClearClickHandler;
            View.OnSettingsClick += SettingsClickHandler;
            View.Initialize();
            View.ClearResult();
            View.SetButtonsVisible(false);
            View.SetPlayers(_gameMediator.GetOrderedPlayersList());
            View.SetCurrentPlayer(_gameMediator.CurrentPlayer.Uid);

            if (Data.GameSessionStorage.Data.Turns.Count > 0)
                View.ShowLastTurn(Data.GameSessionStorage.Data.LastTurnPlayerId, Data.GameSessionStorage.Data.Turns.Last());
            
            View.DevUtils.Initialize(_availableLettersProvider, wordsProvider, _gameMediator, Data.GameSessionStorage.Data,
                _uiSystem);
        }

        public override void Dispose()
        {
            _gameMediator.OnWordChanged -= WordChangedHandler;
            _gameMediator.OnStorageUpdated -= StorageUpdatedHandler;
            _gameMediator.OnWin -= WinHandler;
            
            Data.GameSessionStorage.SurrenderDataUpdated -= SurrenderDataUpdated;
            
            _gameMediator.Dispose();
            
            View.OnBack -= BackClickHandler;
            View.OnApply -= ApplyClickHandler;
            View.OnClear -= ClearClickHandler;
            View.OnSettingsClick -= SettingsClickHandler;
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
        
        private void WordChangedHandler()
        {
            View.UpdateResultWord(_gameMediator.CurrentWord);
            var isEmptyWord = _gameMediator.CurrentWord.IsNullOrEmpty();
            View.SetButtonsVisible(!isEmptyWord);
        }
        
        private void StorageUpdatedHandler()
        {
            View.SetCurrentPlayer(_gameMediator.CurrentPlayer.Uid);
            var lastTurn = Data.GameSessionStorage.Data.Turns.Last();
            View.ShowLastTurn(Data.GameSessionStorage.Data.LastTurnPlayerId, lastTurn);
        }
        
        private void WinHandler(WinData winData)
        {
            var winPopupData = new WinPopupData(winData, Data.GameSessionStorage.Data.Players, WinPopupClosedHandler);
            _uiSystem.ShowPopup(PopupId.Win, winPopupData);
        }

        private void WinPopupClosedHandler()
        {
            _uiSystem.ShowScreen(ScreenId.MainMenu);
        }

        private void BackClickHandler()
        {
            _uiSystem.ShowScreen(ScreenId.MainMenu);
        }
        
        private void ApplyClickHandler()
        {
            var validationResult = _wordValidator.Validate(_gameMediator.CurrentWord);
            switch (validationResult)
            {
                case ValidationResultType.Valid:
                    View.ClearResult();
                    View.SetButtonsVisible(false);
                    _gameMediator.ApplyCurrentWord();
                    View.SetCurrentPlayer(_gameMediator.CurrentPlayer.Uid);
                    View.ShowLastTurn(Data.GameSessionStorage.Data.LastTurnPlayerId, Data.GameSessionStorage.Data.Turns.Last());
                    break;
                
                case ValidationResultType.AlreadyUsed:
                    View.ClearResult();
                    View.SetButtonsVisible(false);
                    _gameMediator.ClearCurrentWord();
                    View.ShowInfoField(AlreadyUsedWordMessage);
                    break;
                
                case ValidationResultType.NotFoundInVocabulary:
                    View.ShowInfoField(NotFoundWordMessage);
                    break;
                
                default:
                    View.ClearResult();
                    View.SetButtonsVisible(false);
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

        private void SettingsClickHandler()
        {
            _uiSystem.ShowPopup(PopupId.GameSettingsPanel, new GameSettingsPanelData(Data.GameSessionStorage));
        }
        
        private void SurrenderDataUpdated(IGameSessionStorage sender)
        {
            var surrenderData = Data.GameSessionStorage.Data.SurrenderData;
            if (surrenderData == null)
            {
                Debug.Log("Surrender data is deleted");
                return;
            }
            
            if (surrenderData.InitiatorUid != _player.Uid)
                ProcessOpponentsOfferToSurrender();
        }

        private void ProcessOpponentsOfferToSurrender()
        {
            var opponentData = Data.GameSessionStorage.Data.Players.First(p => p.Uid != _player.Uid);
            var confirmationPopupData = new ConfirmationPopupData(ConfirmationPopupType.TwoButtons,
                string.Empty,
                string.Format(ConfirmationPopupText.MainText.OpponentOffersToSurrender, opponentData.Name),
                ConfirmationPopupText.ConfirmButton.Yes,
                onConfirm,
                ConfirmationPopupText.DeclineButton.No,
                onDecline);
            
            _uiSystem.ShowPopup(PopupId.ConfirmationPopup, confirmationPopupData);

            void onConfirm()
            {
                var winData = new WinData(opponentData.Uid, WinReason.Surrender);
                Data.GameSessionStorage.Data.WinData = winData;
                Data.GameSessionStorage.Data.SurrenderData = null;
                Data.GameSessionStorage.Save();

                _gameSessionsManager.DeleteGameForUserAsync(Data.GameSessionStorage).Run();
                
                _gameMediator.ProcessWin();
            }

            void onDecline()
            {
                Data.GameSessionStorage.Data.SurrenderData = null;
                Data.GameSessionStorage.Save();
            }
        }
    }
}