using App.Enums;
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
            _currentScreen?.Close();
            _currentScreen = ShowWithId(screenId, data);
        }

        public void ShowPopup(PopupId popupId, object data = null)
        {
            ShowWithId(popupId, data);
        }

        private IScreenController ShowWithId(object id, object data)
        {
            var elementViewPrefab = _diContainer.ResolveId<ScreenView>(id);
            var elementViewGO = _diContainer.InstantiatePrefab(elementViewPrefab, _screenContainer);
            var elementView = elementViewGO.GetComponent<ScreenView>();
            var controller = _diContainer.TryResolveId<IScreenController>(id);
            controller.SetView(elementView);
            controller.SetData(data);
            controller.Initialize();
            controller.Show();
            return controller;
        }
    }
}