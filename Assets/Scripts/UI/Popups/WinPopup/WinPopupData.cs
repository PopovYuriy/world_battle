using System;

namespace UI.Popups.WinPopup
{
    public sealed class WinPopupData
    {
        public string PlayerName { get; }
        public Action OnCloseCallback { get; }

        public WinPopupData(string playerName, Action onCloseCallback)
        {
            PlayerName = playerName;
            OnCloseCallback = onCloseCallback;
        }
    }
}