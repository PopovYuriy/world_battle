namespace Core.UI.Screens
{
    public abstract class ScreenControllerAbstract<TScreenView> : IScreenController 
        where TScreenView : ScreenView
    {
        protected TScreenView View { get; private set; }

        public void SetView(ScreenView view) => View = view as TScreenView;

        public void SetData(object data) {}

        public abstract void Initialize();
        
        public abstract void Dispose();

        public abstract void Show();

        public abstract void Close();
    }
    
    public abstract class ScreenControllerAbstract<TScreenView, TData> : IScreenController 
        where TScreenView : ScreenView
        where TData : class
    {
        protected TScreenView View { get; private set; }
        protected TData Data { get; private set; }

        public void SetView(ScreenView view) => View = view as TScreenView;
        public void SetData(object data) => Data = data as TData;

        public abstract void Initialize();
        
        public abstract void Dispose();

        public abstract void Show();

        public abstract void Close();
    }
}