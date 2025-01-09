using Zenject;

namespace Core.Modules
{
    public abstract class ModuleInstaller : Installer<ModuleInstaller>
    {
        private DiContainer _localContainer;

        protected DiContainer LocalContainer
        {
            get
            {
                _localContainer ??= Container.CreateSubContainer();
                return _localContainer;
            }
        }
        
        protected DiContainer GlobalContainer => Container;
    }
}