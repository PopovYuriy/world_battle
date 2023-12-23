using Core.UI;
using Core.UI.Enums;
using Core.UI.Screens;
using Game.Data;
using Game.Field.Mediators;
using UI.GameScreen.Data;
using UnityEngine;
using Zenject;

namespace UI.GameScreen
{
    public sealed class GameScreenController : ScreenControllerAbstract<GameScreenView, GameScreenData>
    {
        [Inject] private UISystem _uiSystem;
        [Inject] private GameFieldColorsConfig _colorsConfig;
        
        private IGameMediator _gameMediator;
        
        public override void Initialize()
        {
            var gameField = Object.Instantiate(Data.GameFieldPrefab, View.GameFieldPlaceholder);
            _gameMediator = Data.GameMediator;
            _gameMediator.Initialize(gameField, Data.GameSessionStorage, _colorsConfig, Data.OwnerId);

            View.OnBackClicked += BackButtonClickHandler;
            View.Initialize();
        }

        public override void Dispose()
        {
            View.OnBackClicked -= BackButtonClickHandler;
        }

        public override void Show()
        {
            View.Show();
        }

        public override void Close()
        {
            Dispose();
            View.Hide();
            Object.Destroy(View.gameObject);
        }

        private void BackButtonClickHandler()
        {
            _uiSystem.ShowScreen(ScreenId.MainMenu);
        }
    }
}