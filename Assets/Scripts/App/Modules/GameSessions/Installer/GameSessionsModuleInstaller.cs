using App.Modules.GameSessions.API;
using Core.Modules;
using Core.Modules.Extensions;
using Zenject;

namespace App.Modules.GameSessions.Installer
{
    public sealed class GameSessionsModuleInstaller : ModuleInstaller
    {
        [Inject] private SignalBus _signalBus;

        public override void InstallBindings()
        {
            InstallLocalBindings();
            InstallGlobalBindings();
            DeclareSignals();
        }

        private void InstallLocalBindings()
        {
            LocalContainer.Bind<IGameSessionsAPI>().To<RDBGameSessionsAPI>().AsSingle();

            LocalContainer.Bind(typeof(IGameSessionsManager), typeof(IGameSessionsManagerInitializable)).To<GameSessionsManager>().AsSingle();
        }

        private void InstallGlobalBindings()
        {
            GlobalContainer.Bind(typeof(IGameSessionsManager), typeof(IGameSessionsManagerInitializable)).FromLocalContainer(LocalContainer);
            GlobalContainer.Bind<IGameSessionsAPI>().FromLocalContainer(LocalContainer);
        }

        private void DeclareSignals()
        {
            _signalBus.DeclareSignal<GameSessionsSignal.GameCreatedSignal>();
            _signalBus.DeclareSignal<GameSessionsSignal.GameDeletedSignal>();
        }
    }
}