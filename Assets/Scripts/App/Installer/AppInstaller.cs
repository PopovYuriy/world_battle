using App.Commands;
using App.Signals;
using Core.Commands;
using Core.UI;
using Zenject;

namespace App.Installer
{
    public sealed class AppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<UISystem>().FromComponentInHierarchy().AsSingle();

            Container.DeclareSignalAndBindToAsyncCommand<CreateOnlineGameSignal, CreateOnlineGameCommand>();
            Container.DeclareSignalAndBindToAsyncCommand<CreateLocalGameSignal, CreateLocalGameCommand>();
            Container.DeclareSignalAndBindToAsyncCommand<StartExistGameSignal, StartExistGameCommand>();
        }
    }
}