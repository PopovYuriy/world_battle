using App.Enums;
using Core.UI.Enums;
using Core.UI.Screens;
using UnityEngine;
using Zenject;

namespace Core.UI
{
    public sealed class UISystem : MonoBehaviour
    {
        [SerializeField] private Transform _screenContainer;

        [Inject] private DiContainer _diContainer;

        private IScreenController _currentScreen;

        public void ShowScreen(ScreenId screenId, object data = null)
        {
            var controller = ShowWithId(screenId, data);
            _currentScreen?.Close();
            _currentScreen = controller;
        }

        public void ShowPopup(PopupId popupId, object data = null)
        {
            ShowWithId(popupId, data);
        }

        private IScreenController ShowWithId(object id, object data)
        {
            var elementViewPrefab = _diContainer.ResolveId<ScreenView>(id);
            var elementView = Instantiate(elementViewPrefab, _screenContainer);

            var controller = _diContainer.TryResolveId<IScreenController>(id);
            controller.SetView(elementView);
            controller.SetData(data);
            controller.Initialize();
            controller.Show();
            return controller;
        }
    }
}