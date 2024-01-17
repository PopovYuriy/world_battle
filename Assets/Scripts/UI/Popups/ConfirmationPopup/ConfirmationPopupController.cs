using Core.UI.Screens;
using UnityEngine;

namespace UI.Popups.ConfirmationPopup
{
    public sealed class ConfirmationPopupController : ScreenControllerAbstract<ConfirmationPopupView, ConfirmationPopupData>
    {
        public override void Initialize()
        {
            View.Initialize();
            View.SetType(Data.Type);
            View.SetHeader(Data.HeaderText);
            View.SetText(Data.MainText);
            View.SetConfirmationButtonLabel(Data.ConfirmButtonLabel);
            View.SetDeclineButtonLabel(Data.DeclineButtonLabel);
            View.OnConfirmClicked += ConfirmButtonClickHandler;
            View.OnDeclineClicked += DeclineButtonClickHandler;
        }

        public override void Dispose()
        {
            View.OnConfirmClicked -= ConfirmButtonClickHandler;
            View.OnDeclineClicked -= DeclineButtonClickHandler;
            View.Dispose();
        }

        public override void Show()
        {
            View.gameObject.SetActive(true);
        }

        public override void Close()
        {
            Dispose();
            Object.Destroy(View.gameObject);
        }

        private void ConfirmButtonClickHandler()
        {
            Data.ConfirmCallback?.Invoke();
            Close();
        }

        private void DeclineButtonClickHandler()
        {
            Data.DeclineCallback?.Invoke();
            Close();
        }
    }
}