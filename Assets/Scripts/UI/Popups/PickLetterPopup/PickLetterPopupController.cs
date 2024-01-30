using Core.UI.Screens;
using UnityEngine;

namespace UI.Popups.PickLetterPopup
{
    public sealed class PickLetterPopupController : ScreenControllerAbstract<PickLetterPopupView, PickLetterPopupData>
    {
        private LetterItem _pickedItem;
        
        public override void Initialize()
        {
            View.Initialize();
            View.SetConfirmButtonVisible(false);
            View.SetLetterToChange(Data.LetterToChange);

            View.OnCloseClicked += CloseClickHandler;
            View.OnLetterPicked += LetterPickedHandler;
            View.OnConfirmClicked += ConfirmClicked;
        }

        public override void Dispose()
        {
            View.OnCloseClicked -= CloseClickHandler;
            View.OnLetterPicked -= LetterPickedHandler;
            View.OnConfirmClicked -= ConfirmClicked;
        }

        public override void Show()
        {
            View.Show();
        }

        public override void Close()
        {
            Data.OnDeclineCallback?.Invoke();
            Dispose();
            Object.Destroy(View.gameObject);
        }

        private void LetterPickedHandler(LetterItem letterItem)
        {
            if (_pickedItem == null)
                View.SetConfirmButtonVisible(true);
            else 
                _pickedItem.SetPickedState(false);
            
            _pickedItem = letterItem;
            _pickedItem.SetPickedState(true);
        }

        private void ConfirmClicked()
        {
            Data.OnPickLetterCallback?.Invoke(_pickedItem.Letter);
            Close();
        }

        private void CloseClickHandler()
        {
            Close();
        }
    }
}