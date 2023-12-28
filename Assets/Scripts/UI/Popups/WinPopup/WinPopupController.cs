using Core.UI.Screens;
using UnityEngine;

namespace UI.Popups.WinPopup
{
    public sealed class WinPopupController : ScreenControllerAbstract<WinPopupView, WinPopupData>
    {
        public override void Initialize()
        {
            View.Initialize();
            View.SetPlayerName(Data.PlayerName);
            View.OnOkClicked += OkClickedHandler;
        }

        private void OkClickedHandler()
        {
            Close();
            Data.OnCloseCallback?.Invoke();
        }

        public override void Dispose()
        {
            View.OnOkClicked -= OkClickedHandler;
        }

        public override void Show()
        {
            View.Show();
        }

        public override void Close()
        {
            Dispose();
            Object.Destroy(View.gameObject);
        }
    }
}