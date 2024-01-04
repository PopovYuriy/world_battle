namespace UI.Popups.EnterNamePopup
{
    public sealed class UpdateNamePopupData
    {
        public string CurrentName { get; }

        public UpdateNamePopupData(string currentName)
        {
            CurrentName = currentName;
        }
    }
}