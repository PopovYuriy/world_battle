using System;

namespace UI.Popups.ConfirmationPopup
{
    public sealed class ConfirmationPopupData
    {
        public ConfirmationPopupType Type { get; }
        public string HeaderText { get; }
        public string MainText { get; }
        public string ConfirmButtonLabel { get; }
        public string DeclineButtonLabel { get; }
        
        public Action ConfirmCallback { get; }
        public Action DeclineCallback { get; }

        public ConfirmationPopupData(ConfirmationPopupType type, string headerText, string mainText, string confirmButtonLabel,
            Action confirmCallback, string declineButtonLabel = null, Action declineCallback = null)
        {
            Type = type;
            HeaderText = headerText;
            MainText = mainText;
            ConfirmButtonLabel = confirmButtonLabel;
            ConfirmCallback = confirmCallback;

            DeclineButtonLabel = declineButtonLabel;
            DeclineCallback = declineCallback;
        }
    }
}