using Zenject;

namespace Core.Modules
{
    public sealed class ModuleInitializer<TModule> : IModuleInitializer where TModule : IModule
    {
        private readonly DiContainer _container;

        public ModuleInitializer(DiContainer container)
        {
            _container = container;
        }

        public void Install()
        {
        }

        public void Initialize()
        {
            _container.Instantiate<TModule>().Initialize();
        }
    }
    
    public sealed class ModuleInitializerWithInstaller<TInstaller, TModule> : IModuleInitializer where TInstaller : ModuleInstaller where TModule : IModule
    {
        private readonly DiContainer _container;

        public ModuleInitializerWithInstaller(DiContainer container)
        {
            _container = container;
        }

        public void Install()
        {
            _container.Instantiate<TInstaller>().InstallBindings();
        }

        public void Initialize()
        {
            _container.Instantiate<TModule>();
        }
    }
    
    public sealed class ModuleInitializerWithInstallerOnly<TInstaller> : IModuleInitializer where TInstaller : ModuleInstaller
    {
        private readonly DiContainer _container;

        public ModuleInitializerWithInstallerOnly(DiContainer container)
        {
            _container = container;
        }

        public void Install()
        {
            _container.Instantiate<TInstaller>().InstallBindings();
        }

        public void Initialize()
        {
        }
    }
}