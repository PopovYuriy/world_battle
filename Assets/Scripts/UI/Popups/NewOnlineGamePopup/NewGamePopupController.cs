using App.Modules.Matchmaking;
using App.Modules.Matchmaking.Enums;
using App.Signals;
using Core.UI.Screens;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace UI.Popups.NewOnlineGamePopup
{
    public sealed class NewGamePopupController : ScreenControllerAbstract<NewGamePopupView>
    {
        [Inject] private SignalBus   _signalBus;
        [Inject] private IMatchmaker _matchmaker;

        public override void Initialize()
        {
            View.Initialize();

            View.OnCloseClick += Close;
            View.OnCreateOnlineClick += CreateOnlineGameClickHandler;
            View.OnFindOnlineClick += FindOnlineGameClickHandler;
            View.OnCreateLocalClick += CreateLocalGameClickHandler;
        }

        public override void Dispose()
        {
            _signalBus.TryUnsubscribe<MatchmakerSignal.PendingGameCreated>(PendingGameCreateHandler);
            _signalBus.TryUnsubscribe<MatchmakerSignal.PendingGameExpired>(GameExpiredHandler);
            
            View.OnCloseClick -= Close;
            View.OnCreateOnlineClick -= CreateOnlineGameClickHandler;
            View.OnFindOnlineClick -= FindOnlineGameClickHandler;
            View.OnCreateLocalClick -= CreateLocalGameClickHandler;
        }

        public override void Show()
        {
            View.Show();
        }

        public override void Close()
        {
            View.Hide();
            Dispose();
            Object.Destroy(View.gameObject);
        }

        private void CreateOnlineGameClickHandler()
        {
            View.ShowPendingState();
            
            _signalBus.Subscribe<MatchmakerSignal.PendingGameCreated>(PendingGameCreateHandler);
            
            _matchmaker.CreateGame();
        }

        private void PendingGameCreateHandler(MatchmakerSignal.PendingGameCreated signal)
        {
            _signalBus.Unsubscribe<MatchmakerSignal.PendingGameCreated>(PendingGameCreateHandler);

            if (signal.Arg.Error != PendingGameCreateError.None)
            {
                //ToDo :: show error message
                View.HidePendingState();
                return;
            }
            
            _signalBus.Subscribe<MatchmakerSignal.PendingGameSessionCreated>(GameSessionCreatedHandler);
            _signalBus.Subscribe<MatchmakerSignal.PendingGameExpired>(GameExpiredHandler);
        }
        
        private void GameSessionCreatedHandler(MatchmakerSignal.PendingGameSessionCreated signal)
        {
            _signalBus.Unsubscribe<MatchmakerSignal.PendingGameSessionCreated>(GameSessionCreatedHandler);
            _signalBus.Unsubscribe<MatchmakerSignal.PendingGameExpired>(GameExpiredHandler);

            if (signal.Arg.IsNullOrEmpty())
            {
                //ToDo :: show error message
                View.HidePendingState();
                return;
            }
            
            _signalBus.Fire(new StartExistGameSignal(signal.Arg));
        }
        
        private void GameExpiredHandler()
        {
            _signalBus.Unsubscribe<MatchmakerSignal.PendingGameSessionCreated>(GameSessionCreatedHandler);
            _signalBus.Unsubscribe<MatchmakerSignal.PendingGameExpired>(GameExpiredHandler);
            
            //ToDo :: show game is expired
            View.HidePendingState();
        }

        private void FindOnlineGameClickHandler()
        {
            View.ShowPendingState();
            _signalBus.Subscribe<MatchmakerSignal.PendingGameJoined>(GameJoinedHandler);
            _matchmaker.JoinGame(View.InputText);
        }

        private void GameJoinedHandler(MatchmakerSignal.PendingGameJoined signal)
        {
            _signalBus.Unsubscribe<MatchmakerSignal.PendingGameJoined>(GameJoinedHandler);
            
            if (signal.Arg.Error != PendingGameJoinError.None)
            {
                //ToDo :: show error message
                View.HidePendingState();
                return;
            }
            
            _signalBus.Fire(new StartExistGameSignal(signal.Arg.Data.Id));
        }

        private void CreateLocalGameClickHandler()
        {
            _signalBus.Fire<CreateLocalGameSignal>();
            Close();
        }
    }
}