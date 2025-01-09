namespace Core.Modules
{
    public interface IModuleInitializer
    {
        void Install();
        void Initialize();
    }
}