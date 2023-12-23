using App.Commands;
using App.Signals;
using Core.Commands;
using Core.UI;
using Game.Field;
using UnityEngine;
using Zenject;

namespace App.Installer
{
    public sealed class AppInstaller : MonoInstaller
    {
        [SerializeField] private GameField _gameFieldPrefab;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_gameFieldPrefab).AsSingle();

            Container.Bind<UISystem>().FromComponentInHierarchy().AsSingle();

            Container.DeclareSignalAndBindToCommand<CreateLocalGameSignal, CreateLocalGameCommand>();
            Container.DeclareSignalAndBindToAsyncCommand<CreateOnlineGameSignal, CreateOnlineGameCommand>();
            Container.DeclareSignalAndBindToCommand<StartExistGameSignal, StartExistGameCommand>();
        }
    }
}