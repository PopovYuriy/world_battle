using System.Threading.Tasks;
using App.Data.Player;
using App.Enums;
using App.Services;
using Core.Services.Scene;
using Core.UI;
using Cysharp.Threading.Tasks;
using UI.Popups.EnterNamePopup;
using UnityEngine;
using Zenject;

namespace App.Components.SceneInitializers
{
    public sealed class MainSceneInitializer : MonoBehaviour, ISceneInitializer
    {
        private UISystem _uiSystem;
        private IPlayer _player;

        [Inject]
        private void Construct(UISystem uiSystem, IPlayer player)
        {
            _uiSystem = uiSystem;
            _player = player;
        }

        public async Task InitializeAsync()
        {
            _uiSystem.ShowScreen(ScreenId.MainMenu);
            await UniTask.Yield();

            if (!PrefsProvider.FirstTimeNameChanged)
            {
                PrefsProvider.FirstTimeNameChanged = true;
                _uiSystem.ShowPopup(PopupId.UpdateName, new UpdateNamePopupData(_player.Name));
            }
        }
    }
}