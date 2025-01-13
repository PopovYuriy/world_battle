using System.Linq;
using App.Data.Player;
using App.Enums;
using App.Modules.GameSessions.Controller;
using App.Modules.GameSessions.Data;
using Core.UI;
using Core.UI.Screens;
using Game.Abilities;
using Game.Data;
using Game.Field;
using Game.Field.Mediators;
using Game.Services;
using Game.Services.Utils;
using UI.Popups.ConfirmationPopup;
using UI.Popups.GameSettingsPopup;
using UI.Popups.WinPopup;
using UI.Screens.GameScreen.Data;
using UI.Screens.GameScreen.Validator;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace UI.Screens.GameScreen
{
    public sealed class GameScreenController : ScreenControllerAbstract<GameScreenView, GameScreenData>
    {
        private const string NotFoundWordMessage    = "Слово не знайдено у словнику";
        private const string AlreadyUsedWordMessage = "Слово вже використовувалось";

        [Inject] private UISystem              _uiSystem;
        [Inject] private GameFieldColorsConfig _colorsConfig;
        [Inject] private IPlayer               _player;
        [Inject] private AbilityConfigsStorage _abilitiesConfig;

        private IGamePlayController      _gamePlayController;
        private WordValidator            _wordValidator;
        private AvailableLettersProvider _availableLettersProvider;

        public override void Initialize()
        {
            var letters = Data.GameSessionController.Data.Grid.Cells
                              .SelectMany(list => list.Select(c => c.Letter)).ToArray();
            var wordsProvider = new WordsProvider();
            wordsProvider.Initialize(letters);

            View.GridView.Initialize();
            var gameField = new GameField(Data.GameSessionController.Data.Players, View.GridView);
            _gamePlayController = Data.GamePlayController;
            _gamePlayController.OnWordChanged += WordChangedHandler;
            _gamePlayController.OnStorageUpdated += StorageUpdatedHandler;
            _gamePlayController.OnWin += WinHandler;
            _gamePlayController.Initialize(gameField, Data.GameSessionController, _colorsConfig, _player.Uid);
            _gamePlayController.Activate();

            Data.GameSessionController.OnSurrenderDataUpdated += SurrenderDataUpdated;

            _wordValidator = new WordValidator(Data.GameSessionController.Data, wordsProvider);
            _availableLettersProvider = new AvailableLettersProvider(gameField);

            View.OnBack += BackClickHandler;
            View.OnApply += ApplyClickHandler;
            View.OnClear += ClearClickHandler;
            View.OnSettingsClick += SettingsClickHandler;
            View.Initialize();
            View.ClearResult();
            View.SetButtonsVisible(false);
            View.SetPlayers(_gamePlayController.GetOrderedPlayersList());
            View.SetCurrentPlayer(_gamePlayController.CurrentPlayer.Uid);

            foreach (var playerGameData in Data.GameSessionController.Data.Players)
            {
                var abilitiesController = View.GetAbilitiesController(playerGameData.Uid);
                abilitiesController.Initialize(playerGameData, _abilitiesConfig, Data.GameSessionController, _gamePlayController,
                                               _uiSystem);

                if (playerGameData.Uid == _gamePlayController.CurrentPlayer.Uid)
                    abilitiesController.Activate();
                else
                    abilitiesController.Deactivate();
            }

            if (Data.GameSessionController.Data.Turns?.Count > 0)
                View.ShowLastTurn(Data.GameSessionController.Data.LastTurnPlayerId, Data.GameSessionController.Data.Turns.Last());

            View.DevUtils.Initialize(_availableLettersProvider, wordsProvider, _gamePlayController, Data.GameSessionController.Data,
                                     _uiSystem, Data.GameSessionController, StorageUpdatedHandler);
        }

        public override void Dispose()
        {
            _gamePlayController.OnWordChanged -= WordChangedHandler;
            _gamePlayController.OnStorageUpdated -= StorageUpdatedHandler;
            _gamePlayController.OnWin -= WinHandler;
            _gamePlayController.Dispose();

            Data.GameSessionController.OnSurrenderDataUpdated -= SurrenderDataUpdated;


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
            View.UpdateResultWord(_gamePlayController.CurrentWord);
            var isEmptyWord = _gamePlayController.CurrentWord.IsNullOrEmpty();
            View.SetButtonsVisible(!isEmptyWord);
        }

        private void StorageUpdatedHandler()
        {
            View.SetCurrentPlayer(_gamePlayController.CurrentPlayer.Uid);
            var lastTurn = Data.GameSessionController.Data.Turns.Last();
            View.ShowLastTurn(Data.GameSessionController.Data.LastTurnPlayerId, lastTurn);

            foreach (var playerGameData in Data.GameSessionController.Data.Players)
                View.GetAbilitiesController(playerGameData.Uid).UpdatePlayerAbilitiesInfo();

            Data.GameSessionController.Data.AbilityData = null;
        }

        private void WinHandler(WinData winData)
        {
            var winPopupData = new WinPopupData(winData, Data.GameSessionController.Data.Players, WinPopupClosedHandler);
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
            var validationResult = _wordValidator.Validate(_gamePlayController.CurrentWord);
            switch (validationResult)
            {
                case ValidationResultType.Valid:
                    View.ClearResult();
                    View.SetButtonsVisible(false);
                    _gamePlayController.ApplyCurrentWord();
                    foreach (var playerGameData in Data.GameSessionController.Data.Players)
                    {
                        var abilitiesController = View.GetAbilitiesController(playerGameData.Uid);
                        abilitiesController.UpdatePlayerAbilitiesInfo();
                        if (playerGameData.Uid == _gamePlayController.CurrentPlayer.Uid)
                            abilitiesController.Activate();
                        else
                            abilitiesController.Deactivate();
                    }

                    View.SetCurrentPlayer(_gamePlayController.CurrentPlayer.Uid);
                    View.ShowLastTurn(Data.GameSessionController.Data.LastTurnPlayerId, Data.GameSessionController.Data.Turns.Last());
                    break;

                case ValidationResultType.AlreadyUsed:
                    View.ClearResult();
                    View.SetButtonsVisible(false);
                    _gamePlayController.ClearCurrentWord();
                    View.ShowInfoField(AlreadyUsedWordMessage);
                    break;

                case ValidationResultType.NotFoundInVocabulary:
                    View.ShowInfoField(NotFoundWordMessage);
                    break;

                default:
                    View.ClearResult();
                    View.SetButtonsVisible(false);
                    _gamePlayController.ClearCurrentWord();
                    Debug.LogWarning("Invalid validation result");
                    break;
            }
        }

        private void ClearClickHandler()
        {
            View.ClearResult();
            View.SetButtonsVisible(false);
            _gamePlayController.ClearCurrentWord();
        }

        private void SettingsClickHandler()
        {
            _uiSystem.ShowPopup(PopupId.GameSettingsPanel, new GameSettingsPanelData(Data.GameSessionController));
        }

        private void SurrenderDataUpdated(IGameSessionController sender)
        {
            var surrenderData = Data.GameSessionController.Data.SurrenderData;
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
            var opponentData = Data.GameSessionController.Data.Players.First(p => p.Uid != _player.Uid);
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
                Data.GameSessionController.Data.WinData = winData;
                Data.GameSessionController.Data.SurrenderData = null;
                Data.GameSessionController.Save();

                Data.GameSessionController.Delete();

                _gamePlayController.ProcessWin();
            }

            void onDecline()
            {
                Data.GameSessionController.Data.SurrenderData = null;
                Data.GameSessionController.Save();
            }
        }
    }
}