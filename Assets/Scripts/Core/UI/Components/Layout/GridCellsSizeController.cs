using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Components.Layout
{
    public sealed class GridCellsSizeController : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _gridLayout;
        [SerializeField] private RectTransform _gridContainer;
        
        private void Start() => AdjustGridLayout();

        private void AdjustGridLayout()
        {
            var parentTransform = (RectTransform) _gridContainer.parent;
            float containerWidth = parentTransform.rect.width;
            float totalPaddingWidth = (_gridLayout.constraintCount - 1) * _gridLayout.spacing.x 
                                      + _gridLayout.padding.left + _gridLayout.padding.right;
            float adjustedItemWidth = (containerWidth - totalPaddingWidth) / _gridLayout.constraintCount;

            int numRows = Mathf.CeilToInt((float)_gridContainer.childCount / _gridLayout.constraintCount);
            float containerHeight = numRows * (adjustedItemWidth + _gridLayout.spacing.y);

            _gridContainer.sizeDelta = new Vector2(containerWidth, containerHeight);

            _gridLayout.cellSize = new Vector2(adjustedItemWidth, adjustedItemWidth);
        }

        private void OnValidate() => AdjustGridLayout();
    }
}