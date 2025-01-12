using System;
using System.Collections.Generic;
using App.Modules.GameSessions.Data;

namespace UI.Popups.WinPopup
{
    public sealed class WinPopupData
    {
        public WinData WinData { get; }
        public IReadOnlyList<PlayerGameData> Players { get; }
        public Action OnCloseCallback { get; }

        public WinPopupData(WinData winData, IReadOnlyList<PlayerGameData> players, Action onCloseCallback)
        {
            WinData = winData;
            Players = players;
            OnCloseCallback = onCloseCallback;
        }
    }
}