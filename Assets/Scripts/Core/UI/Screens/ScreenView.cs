using UnityEngine;

namespace Core.UI.Screens
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ScreenView : MonoBehaviour
    {
        protected CanvasGroup CanvasGroup;

        private void Awake() => CanvasGroup = GetComponent<CanvasGroup>();

        private void OnDestroy() => Dispose();

        public virtual void Show()
        {
            CanvasGroup.alpha = 1;
        }
        
        public virtual void Hide()
        {
            CanvasGroup.alpha = 0;
        }

        public abstract void Initialize();
        public abstract void Dispose();
    }
}