using App.Enums;
using Core.UI.Screens;
using UI.GameScreen;
using UI.LoadingScreen;
using UI.MainMenuScreen;
using UI.Popups.ConfirmationPopup;
using UI.Popups.EnterNamePopup;
using UI.Popups.GameSettingsPopup;
using UI.Popups.NewOnlineGamePopup;
using UI.Popups.PickLetterPopup;
using UI.Popups.WinPopup;
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
        [SerializeField] private ScreenView _loadingScreenPrefab;
        
        [Header("Popups:")]
        [SerializeField] private ScreenView _newOnlineGamePopupPrefab;
        [SerializeField] private ScreenView _winPopupPrefab;
        [SerializeField] private ScreenView _updateNamePopupPrefab;
        [SerializeField] private ScreenView _gameSettingsPanelPrefab;
        [SerializeField] private ScreenView _confirmationPopup;
        [SerializeField] private ScreenView _pickLetterPopup;

        public override void InstallBindings()
        {
            BindScreens();
            BindPopups();
        }

        private void BindScreens()
        {
            Container.BindInstance(_mainMenuScreenPrefab).WithId(ScreenId.MainMenu);
            Container.Bind<IScreenController>().WithId(ScreenId.MainMenu).To<MainMenuScreenController>()
                .AsTransient();
            
            Container.BindInstance(_gameScreenPrefab).WithId(ScreenId.Game);
            Container.Bind<IScreenController>().WithId(ScreenId.Game).To<GameScreenController>()
                .AsTransient();
            
            Container.BindInstance(_loadingScreenPrefab).WithId(ScreenId.Loading);
            Container.Bind<IScreenController>().WithId(ScreenId.Loading).To<LoadingScreenController>()
                .AsTransient();
        }

        private void BindPopups()
        {
            Container.BindInstance(_newOnlineGamePopupPrefab).WithId(PopupId.NewGame);
            Container.Bind<IScreenController>().WithId(PopupId.NewGame).To<NewGamePopupController>()
                .AsTransient();
            
            Container.BindInstance(_winPopupPrefab).WithId(PopupId.Win);
            Container.Bind<IScreenController>().WithId(PopupId.Win).To<WinPopupController>()
                .AsTransient();
            
            Container.BindInstance(_updateNamePopupPrefab).WithId(PopupId.UpdateName);
            Container.Bind<IScreenController>().WithId(PopupId.UpdateName).To<UpdateNamePopupController>()
                .AsTransient();
            
            Container.BindInstance(_gameSettingsPanelPrefab).WithId(PopupId.GameSettingsPanel);
            Container.Bind<IScreenController>().WithId(PopupId.GameSettingsPanel).To<GameSettingsPanelController>()
                .AsTransient();
            
            Container.BindInstance(_confirmationPopup).WithId(PopupId.ConfirmationPopup);
            Container.Bind<IScreenController>().WithId(PopupId.ConfirmationPopup).To<ConfirmationPopupController>()
                .AsTransient();
            
            Container.BindInstance(_pickLetterPopup).WithId(PopupId.PickLetter);
            Container.Bind<IScreenController>().WithId(PopupId.PickLetter).To<PickLetterPopupController>()
                .AsTransient();
        }
    }
}