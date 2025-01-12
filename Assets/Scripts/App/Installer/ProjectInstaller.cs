using App.Commands;
using App.Data.Device;
using App.Data.Player;
using App.Modules.GameSessions;
using App.Modules.GameSessions.Installer;
using App.Modules.Matchmaking.Installer;
using App.Services.Database;
using App.Services.PushNotifications;
using App.Signals;
using Core.Commands;
using Core.Modules;
using Core.Services.Scene;
using Zenject;

namespace App.Installer
{
    public class ProjectInstaller : MonoInstaller
    {
        private ModulesResolver _modulesResolver;
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.Bind<IDeviceData>().To<DefaultDeviceData>().AsSingle();
            Container.Bind(typeof(IPlayer), typeof(IPlayerMutable)).To<Player>().AsSingle();
            Container.Bind<ScenesLoader>().AsSingle();
            Container.Bind<RealtimeDatabase>().AsSingle();
            Container.Bind<PushNotificationsService>().AsSingle();
            
            Container.DeclareSignalAndBindToAsyncCommand<UpdateUserNameSignal, UpdateUserNameCommand>();
            
            _modulesResolver = new ModulesResolver(Container)
                .AddModuleInstaller<MatchmakerModuleInstaller>()
                .AddModuleWithInstaller<GameSessionsModuleInstaller, GameSessionsModule>()
                .Resolve(ModulesResolveCompleteHandler);
        }

        private void ModulesResolveCompleteHandler()
        {
            _modulesResolver.Dispose();
            _modulesResolver = null;
            
            
        }
    }
}