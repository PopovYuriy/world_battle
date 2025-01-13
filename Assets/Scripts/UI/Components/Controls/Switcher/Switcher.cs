using System;
using DG.Tweening;
using ModestTree;
using UnityEngine;

namespace UI.Components.Controls.Switcher
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public sealed class Switcher : MonoBehaviour
    {
        [SerializeField] private Color         _activeTextColor;
        [SerializeField] private Color         _inactiveTextColor;
        [SerializeField] private float         _switchDuration;
        [SerializeField] private Ease          _easeType;
        [SerializeField] private RectTransform _marker;
        [SerializeField] private RectTransform _itemsContainer;
        [SerializeField] private SwitchItem[]  _items;

        private SwitchItem _currentSwitch;
        private int        _activeItemIndex;
        private Tween      _markerTween;

        public event Action<int> OnSwitchStarted;
        public event Action<int> OnSwitchComplete;

        private void OnEnable()
        {
            _items = GetComponentsInChildren<SwitchItem>();

            if (_items.Length == 0)
            {
                _marker.offsetMin = Vector2.zero;
                _marker.offsetMax = Vector2.zero;
            }
            else
            {
                _activeItemIndex = Mathf.Clamp(_activeItemIndex, 0, _items.Length - 1);
                var activeItem = _items[_activeItemIndex];

                ActivateItem(activeItem);
            }
        }

        private void Awake()
        {
            foreach (var item in _items)
                item.OnClick += ItemClickHandler;
        }

        private void OnDestroy()
        {
            foreach (var item in _items)
                item.OnClick -= ItemClickHandler;
        }

        public void SetActiveItemIndex(int index)
        {
            _activeItemIndex = Mathf.Clamp(index, 0, _items.Length - 1);
            var activeItem = _items[_activeItemIndex];

            ActivateItem(activeItem);
        }

        private void ItemClickHandler(SwitchItem item)
        {
            if (_currentSwitch == item)
                return;

            ActivateItemWithAnimation(item);
        }

        private void ActivateItem(SwitchItem item)
        {
            foreach (var groupItem in _items)
                groupItem.SetColor(groupItem == item ? _activeTextColor : _inactiveTextColor, _switchDuration);

            DetermineMarkerOffset(item.Transform, out var offsetMin, out var offsetMax);

            _marker.offsetMin = offsetMin;
            _marker.offsetMax = offsetMax;
        }

        private void ActivateItemWithAnimation(SwitchItem item)
        {
            _currentSwitch = item;
            _activeItemIndex = _items.IndexOf(item);

            OnSwitchStarted?.Invoke(_activeItemIndex);

            foreach (var groupItem in _items)
                groupItem.SetColor(groupItem == item ? _activeTextColor : _inactiveTextColor, _switchDuration);

            _markerTween?.Kill();

            DetermineMarkerOffset(item.Transform, out var offsetMin, out var offsetMax);

            _markerTween = DOTween.Sequence()
                                  .Join(DOTween.To(() => _marker.offsetMin, v => _marker.offsetMin = v, offsetMin, _switchDuration))
                                  .Join(DOTween.To(() => _marker.offsetMax, v => _marker.offsetMax = v, offsetMax, _switchDuration))
                                  .SetEase(_easeType)
                                  .OnComplete(() =>
                                  {
                                      _markerTween = null;
                                      OnSwitchComplete?.Invoke(_activeItemIndex);
                                  });
        }

        private void DetermineMarkerOffset(RectTransform itemTransform, out Vector2 min, out Vector2 max)
        {
            var initialOffsetMin = _marker.offsetMin;
            var initialOffsetMax = _marker.offsetMax;

            var newXMinOffset = itemTransform.localPosition.x + itemTransform.rect.xMin - _itemsContainer.rect.xMin;
            var newXMaxOffset = itemTransform.localPosition.x + itemTransform.rect.xMax - _itemsContainer.rect.xMax;

            min = new Vector2(newXMinOffset, initialOffsetMin.y);
            max = new Vector2(newXMaxOffset, initialOffsetMax.y);
        }
    }
}