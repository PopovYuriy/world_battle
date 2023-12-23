using App.Enums;
using Core.UI.Enums;
using Core.UI.Screens;
using UI.GameScreen;
using UI.GamesManagingScreen;
using UI.MainMenuScreen;
using UI.Popups.NewOnlineGamePopup;
using UnityEngine;
using Zenject;

namespace App.Installer
{
    [CreateAssetMenu(fileName = "ScreensProvider", menuName = "Installers/ScreensProvider")]
    public sealed class ScreensProvider : ScriptableObjectInstaller<ScreensProvider>
    {
        [Header("Screens:")]
        [SerializeField] private ScreenView _mainMenuScreenPrefab;
        [SerializeField] private ScreenView _gameScreenPrefab;
        [SerializeField] private ScreenView _gamesManagingScreenPrefab;
        
        [Header("Popups:")]
        [SerializeField] private ScreenView _newOnlineGamePopupPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(_mainMenuScreenPrefab).WithId(ScreenId.MainMenu);
            Container.Bind<IScreenController>().WithId(ScreenId.MainMenu).To<MainMenuScreenController>()
                .AsTransient();
            
            Container.BindInstance(_gameScreenPrefab).WithId(ScreenId.Game);
            Container.Bind<IScreenController>().WithId(ScreenId.Game).To<GameScreenController>()
                .AsTransient();
            
            Container.BindInstance(_gamesManagingScreenPrefab).WithId(ScreenId.GamesManaging);
            Container.Bind<IScreenController>().WithId(ScreenId.GamesManaging).To<GamesManagingScreenController>()
                .AsTransient();

            Container.BindInstance(_newOnlineGamePopupPrefab).WithId(PopupId.NewGame);
            Container.Bind<IScreenController>().WithId(PopupId.NewGame).To<NewGamePopupController>()
                .AsTransient();
        }
    }
}