using App.Modules.Matchmaking.API;
using Core.Modules;
using Core.Modules.Extensions;
using Zenject;

namespace App.Modules.Matchmaking.Installer
{
    public sealed class MatchmakerModuleInstaller : ModuleInstaller
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
            LocalContainer.Bind(typeof(IMatchmakerAPI)).To<RDBMatchmakerAPI>().AsSingle();
            
            LocalContainer.Bind(typeof(IMatchmaker), typeof(IMatchmakerInitializable)).To<Matchmaker>().AsSingle();
        }

        private void InstallGlobalBindings()
        {
            GlobalContainer.Bind(typeof(IMatchmaker), typeof(IMatchmakerInitializable)).FromLocalContainer(LocalContainer);
        }

        private void DeclareSignals()
        {
            _signalBus.DeclareSignal<MatchmakerSignal.PendingGameCreated>();
            _signalBus.DeclareSignal<MatchmakerSignal.PendingGameJoined>();
            _signalBus.DeclareSignal<MatchmakerSignal.PendingGameDeleted>();
            _signalBus.DeclareSignal<MatchmakerSignal.PendingGameExpired>();
            _signalBus.DeclareSignal<MatchmakerSignal.PendingGameSessionCreated>();
        }
    }
}