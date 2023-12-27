using UnityEngine;

namespace Core.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        private void Awake()
        {
            DetermineAnchors();
        }

        private void DetermineAnchors()
        {
            var rectTransform = GetComponent<RectTransform>();
            var anchorMin = Screen.safeArea.position;
            var anchorMax = Screen.safeArea.position + Screen.safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMax.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.y /= Screen.height;
            
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}