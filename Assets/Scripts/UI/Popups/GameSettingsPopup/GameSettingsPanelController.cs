using System.Linq;
using App.Data.Player;
using App.Enums;
using App.Modules.GameSessions;
using App.Modules.GameSessions.Controller;
using App.Modules.GameSessions.Data;
using Core.UI;
using Core.UI.Screens;
using UI.Popups.ConfirmationPopup;
using UnityEngine;
using Zenject;

namespace UI.Popups.GameSettingsPopup
{
    public sealed class GameSettingsPanelController : ScreenControllerAbstract<GameSettingsPanelView, GameSettingsPanelData>
    {
        [Inject] private IPlayer _player;
        [Inject] private UISystem _uiSystem;
        [Inject] private IGameSessionsManager _gameSessionsManager;

        private bool _isLocalGame;
        
        public override void Initialize()
        {
            View.Initialize();
            View.OnGiveUpClicked += GiveUpClickHandler;
            View.OnDeclareDefeatClicked += DeclareDefeatClickHandler;
            View.OnCloseClicked += CloseClickHandler;
            
            var isOwnersFirstTurn = Data.GameSessionController.Data.Players.First().Uid == _player.Uid;
            View.SetWordsList(Data.GameSessionController.Data.Turns, isOwnersFirstTurn);
            
            Data.GameSessionController.OnTurn += GameSessionControllerOnTurnHandler;
            
            var hasSurrenderData = Data.GameSessionController.Data.SurrenderData != null;
            View.SetGiveUpButtonInteractable(true);

            var declareDefeatButtonInteractable = !hasSurrenderData
                                                  || Data.GameSessionController.Data.SurrenderData.InitiatorUid !=
                                                  _player.Uid;
            View.SetDeclareDefeatButtonInteractable(declareDefeatButtonInteractable);

            _isLocalGame = _gameSessionsManager.IsLocalGame(Data.GameSessionController.Data.Uid);
            View.SetDeclareDefeatButtonVisible(!_isLocalGame);

            if (_isLocalGame)
                View.SetGiveUpButtonLabel("Завершити");
        }

        public override void Dispose()
        {
            View.OnGiveUpClicked -= GiveUpClickHandler;
            View.OnDeclareDefeatClicked -= DeclareDefeatClickHandler;
            View.OnCloseClicked -= CloseClickHandler;

            Data.GameSessionController.OnTurn -= GameSessionControllerOnTurnHandler;
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
        
        private void GameSessionControllerOnTurnHandler(IGameSessionController sender)
        {
            var isOwnersTurn = Data.GameSessionController.Data.LastTurnPlayerId == _player.Uid;
            View.AddWord(Data.GameSessionController.Data.Turns.Last(), isOwnersTurn);
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
                    Data.GameSessionController.Delete();
                    _uiSystem.ShowScreen(ScreenId.GamesManaging);
                }
                else
                {
                    var opponentUid = Data.GameSessionController.Data.Players.First(p => p.Uid != _player.Uid).Uid;
                    Data.GameSessionController.Data.WinData = new WinData(opponentUid, WinReason.Surrender);
                    Data.GameSessionController.Save();
                }
                
                Data.GameSessionController.Delete();
                
                Close();
            }
        }

        private void DeclareDefeatClickHandler()
        {
            Data.GameSessionController.Data.SurrenderData = new SurrenderData(_player.Uid);
            Data.GameSessionController.Save();
            Close();
        }

        private void CloseClickHandler()
        {
            View.PlayHideAnimation(Close);
        }
    }
}