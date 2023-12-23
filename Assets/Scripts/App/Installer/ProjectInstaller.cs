using App.Data.Device;
using App.Data.Player;
using App.Services;
using App.Services.Database;
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
        }
    }
}