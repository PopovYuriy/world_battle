using App.Commands;
using App.Data.Device;
using App.Data.Player;
using App.Services;
using App.Services.Database;
using App.Services.PushNotifications;
using App.Signals;
using Core.Commands;
using Core.Services.Scene;
using Zenject;

namespace App.Installer
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.Bind<IDeviceData>().To<DefaultDeviceData>().AsSingle();
            Container.Bind(typeof(IPlayer), typeof(IPlayerMutable)).To<Player>().AsSingle();
            Container.Bind<ScenesLoader>().AsSingle();
            Container.Bind<RealtimeDatabase>().AsSingle();
            Container.Bind<GameSessionsManager>().AsSingle();
            Container.Bind<PushNotificationsService>().AsSingle();
            
            Container.DeclareSignalAndBindToAsyncCommand<UpdateUserNameSignal, UpdateUserNameCommand>();
        }
    }
}