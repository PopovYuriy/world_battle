namespace Core.UI.Screens
{
    public interface IScreenController
    {
        void SetView(ScreenView view);
        void SetData(object data);
        void Initialize();
        void Dispose();
        void Show();
        void Close();
    }
}