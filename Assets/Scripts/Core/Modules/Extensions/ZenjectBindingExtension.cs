using Zenject;

namespace Core.Modules.Extensions
{
    public static class ZenjectBindingExtension
    {
        public static void FromLocalContainer(this ConcreteIdBinderNonGeneric binder, DiContainer localContainer)
        {
            binder.FromSubContainerResolve().ByInstance(localContainer).AsSingle();
        }
        
        public static void FromLocalContainer<TContract>(this ConcreteIdBinderGeneric<TContract> binder, DiContainer localContainer)
        {
            binder.FromSubContainerResolve().ByInstance(localContainer).AsSingle();
        }
    }
}