using System;

namespace UI.Popups.PickLetterPopup
{
    public sealed class PickLetterPopupData
    {
        public char LetterToChange { get; }
        public Action<char> OnPickLetterCallback { get; }
        public Action OnDeclineCallback { get; }

        public PickLetterPopupData(char letterToChange, Action<char> onPickLetterCallback, Action onDeclineCallback)
        {
            LetterToChange = letterToChange;
            OnPickLetterCallback = onPickLetterCallback;
            OnDeclineCallback = onDeclineCallback;
        }
    }
}