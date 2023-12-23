using App.Signals;
using Core.UI.Screens;
using UnityEngine;
using Zenject;

namespace UI.Popups.NewOnlineGamePopup
{
    public sealed class NewGamePopupController : ScreenControllerAbstract<NewGamePopupView>
    {
        [Inject] private SignalBus _signalBus;

        private CreateOnlineGameSignal _createOnlineGameSignal;
        
        public override void Initialize()
        {
            View.Initialize();
            View.SetButtonsInteractable(false);
            
            View.OnEditEnd += EditEndHandler;
            View.OnCloseClick += Close;
            View.OnCreateOnlineClick += CreateOnlineGameClickHandler;
            View.OnFindOnlineClick += FindOnlineGameClickHandler;
            View.OnCreateLocalClick += CreateLocalGameClickHandler;
        }

        public override void Dispose()
        {
            View.OnEditEnd -= EditEndHandler;
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
        
        private void EditEndHandler()
        {
            var isWordValid = ValidateSecretWord(View.InputText);
            View.SetButtonsInteractable(isWordValid);

            if (!isWordValid)
                View.ShowInfoText(null);
        }
        
        private void CreateOnlineGameClickHandler()
        {
            View.ShowPendingState();
            _createOnlineGameSignal = new CreateOnlineGameSignal(View.InputText, CreateOnlineGameSignalType.Create);
            _createOnlineGameSignal.OnGameStarted += GameStartedHandler;
            _signalBus.Fire(_createOnlineGameSignal);
        }

        private void FindOnlineGameClickHandler()
        {
            View.ShowPendingState();
            _createOnlineGameSignal = new CreateOnlineGameSignal(View.InputText, CreateOnlineGameSignalType.Find);
            _createOnlineGameSignal.OnGameStarted += GameStartedHandler;
            _signalBus.Fire(_createOnlineGameSignal);
        }

        private void CreateLocalGameClickHandler()
        {
            _signalBus.Fire<CreateLocalGameSignal>();
            Close();
        }

        private void GameStartedHandler()
        {
            _createOnlineGameSignal.OnGameStarted -= GameStartedHandler;
            Close();
        }
        
        private bool ValidateSecretWord(string word) => word.Length is > 5 and < 15;
    }
}