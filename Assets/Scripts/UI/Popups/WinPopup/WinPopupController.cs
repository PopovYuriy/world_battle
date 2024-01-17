using App.Data.Player;
using Core.UI.Screens;
using Game.Data;
using UI.Popups.ConfirmationPopup;
using UnityEngine;
using Zenject;

namespace UI.Popups.WinPopup
{
    public sealed class WinPopupController : ScreenControllerAbstract<ConfirmationPopupView, WinPopupData>
    {
        private IPlayer _player;

        [Inject]
        private void Construct(IPlayer player)
        {
            _player = player;
        }
        
        public override void Initialize()
        {
            View.Initialize();
            View.SetType(ConfirmationPopupType.SingleButton);
            View.SetConfirmationButtonLabel(ConfirmationPopupText.ConfirmButton.Ok);
            
            if (Data.WinData.PlayerId == _player.Uid)
            {
                View.SetHeader("Перемога :)");

                var text = Data.WinData.Reason == WinReason.Surrender
                    ? "Опонент здався"
                    : "Ви захватили базу";
                
                View.SetText(text);
            }
            else
            {
                View.SetHeader("Поразка :(");

                var text = Data.WinData.Reason == WinReason.Surrender
                    ? "Ви здалися"
                    : "Опонент захватив базу";
                
                View.SetText(text);
            }
            
            View.OnConfirmClicked += OkClickedHandler;
        }

        public override void Dispose()
        {
            View.Dispose();
            View.OnConfirmClicked -= OkClickedHandler;
        }

        public override void Show()
        {
            
        }

        public override void Close()
        {
            Dispose();
            Object.Destroy(View.gameObject);
        }
        
        private void OkClickedHandler()
        {
            Close();
            Data.OnCloseCallback?.Invoke();
        }
    }
}