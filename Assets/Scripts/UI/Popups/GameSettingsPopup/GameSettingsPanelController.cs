using System.Linq;
using App.Data.Player;
using App.Enums;
using App.Services;
using Core.UI;
using Core.UI.Screens;
using Game.Data;
using Game.Services.Storage;
using Tools.CSharp;
using UI.Popups.ConfirmationPopup;
using UnityEngine;
using Zenject;

namespace UI.Popups.GameSettingsPopup
{
    public sealed class GameSettingsPanelController : ScreenControllerAbstract<GameSettingsPanelView, GameSettingsPanelData>
    {
        private IPlayer _player;
        private UISystem _uiSystem;
        private GameSessionsManager _gameSessionsManager;

        private bool _isLocalGame;

        [Inject]
        private void Construct(IPlayer player, UISystem uiSystem, GameSessionsManager gameSessionsManager)
        {
            _player = player;
            _uiSystem = uiSystem;
            _gameSessionsManager = gameSessionsManager;
        }
        
        public override void Initialize()
        {
            View.Initialize();
            View.OnGiveUpClicked += GiveUpClickHandler;
            View.OnDeclareDefeatClicked += DeclareDefeatClickHandler;
            View.OnCloseClicked += CloseClickHandler;
            
            var isOwnersFirstTurn = Data.GameSessionStorage.Data.Players.First().Uid == _player.Uid;
            View.SetWordsList(Data.GameSessionStorage.Data.Turns, isOwnersFirstTurn);
            
            Data.GameSessionStorage.OnTurn += GameSessionStorageOnTurnHandler;
            
            var hasSurrenderData = Data.GameSessionStorage.Data.SurrenderData != null;
            View.SetGiveUpButtonInteractable(true);

            var declareDefeatButtonInteractable = !hasSurrenderData
                                                  || Data.GameSessionStorage.Data.SurrenderData.InitiatorUid !=
                                                  _player.Uid;
            View.SetDeclareDefeatButtonInteractable(declareDefeatButtonInteractable);

            _isLocalGame = _gameSessionsManager.IsLocalGame(Data.GameSessionStorage.Data.Uid);
            View.SetDeclareDefeatButtonVisible(!_isLocalGame);

            if (_isLocalGame)
                View.SetGiveUpButtonLabel("Завершити");
        }

        public override void Dispose()
        {
            View.OnGiveUpClicked -= GiveUpClickHandler;
            View.OnDeclareDefeatClicked -= DeclareDefeatClickHandler;
            View.OnCloseClicked -= CloseClickHandler;

            Data.GameSessionStorage.OnTurn -= GameSessionStorageOnTurnHandler;
        }

        public override void Show()
        {
            View.PlayShowAnimation();
        }

        public override void Close()
        {
            Dispose();
            Object.Destroy(View.gameObject);
        }
        
        private void GameSessionStorageOnTurnHandler(IGameSessionStorage sender)
        {
            var isOwnersTurn = Data.GameSessionStorage.Data.LastTurnPlayerId == _player.Uid;
            View.AddWord(Data.GameSessionStorage.Data.Turns.Last(), isOwnersTurn);
        }

        private void GiveUpClickHandler()
        {
            var confirmationPopupData = new ConfirmationPopupData(ConfirmationPopupType.TwoButtons,
                string.Empty,
                _isLocalGame ? ConfirmationPopupText.MainText.LocalGameOverConfirmation : ConfirmationPopupText.MainText.SelfSurrenderConfirmation,
                ConfirmationPopupText.ConfirmButton.Yes,
                onConfirm,
                ConfirmationPopupText.DeclineButton.No);
            
            _uiSystem.ShowPopup(PopupId.ConfirmationPopup, confirmationPopupData);

            void onConfirm()
            {
                if (_isLocalGame)
                {
                    Data.GameSessionStorage.Delete();
                    _uiSystem.ShowScreen(ScreenId.GamesManaging);
                }
                else
                {
                    var opponentUid = Data.GameSessionStorage.Data.Players.First(p => p.Uid != _player.Uid).Uid;
                    Data.GameSessionStorage.Data.WinData = new WinData(opponentUid, WinReason.Surrender);
                    Data.GameSessionStorage.Save();
                }
                
                _gameSessionsManager.DeleteGameForUserAsync(Data.GameSessionStorage).Run();
                
                Close();
            }
        }

        private void DeclareDefeatClickHandler()
        {
            Data.GameSessionStorage.Data.SurrenderData = new SurrenderData(_player.Uid);
            Data.GameSessionStorage.Save();
            Close();
        }

        private void CloseClickHandler()
        {
            View.PlayHideAnimation(Close);
        }
    }
}