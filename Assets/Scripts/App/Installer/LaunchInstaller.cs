using App.Launch.Commands;
using App.Launch.Signals;
using Core.Commands;
using Zenject;

namespace App.Installer
{
    public class LaunchInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<InitializeFirebaseAsyncCommand>().AsSingle();
            Container.Bind<AuthenticationAsyncCommand>().AsSingle();
            Container.Bind<InitializeRealtimeDatabaseAsyncCommand>().AsSingle();
            Container.Bind<InitializeFirebaseNotificationsAsyncCommand>().AsSingle();
            
            Container.DeclareSignalAndBindToAsyncCommand<LaunchFinishedSignal, LaunchFinishedCommand>();
        }
    }
}