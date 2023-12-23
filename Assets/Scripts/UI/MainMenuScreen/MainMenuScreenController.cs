using Core.UI;
using Core.UI.Enums;
using Core.UI.Screens;
using UnityEngine;
using Zenject;

namespace UI.MainMenuScreen
{
    public sealed class MainMenuScreenController : ScreenControllerAbstract<MainMenuScreenView>
    {
        [Inject] private UISystem _uiSystem;
        
        public override void Initialize()
        {
            View.Initialize();
            View.OnStartClicked += StartClickHandler;
        }

        public override void Dispose()
        {
            View.OnStartClicked -= StartClickHandler;
        }

        public override void Show()
        {
            View.Show();
        }

        public override void Close()
        {
            View.Hide();
            Object.Destroy(View.gameObject);
            Dispose();
        }

        private void StartClickHandler()
        {
            _uiSystem.ShowScreen(ScreenId.GamesManaging);
        }
    }
}