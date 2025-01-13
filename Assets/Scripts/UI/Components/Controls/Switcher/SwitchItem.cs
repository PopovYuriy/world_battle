using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components.Controls.Switcher
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class SwitchItem : MonoBehaviour
    {
        [SerializeField] private RectTransform   _transform;
        [SerializeField] private Button          _button;
        [SerializeField] private TextMeshProUGUI _text;

        private Tween _colorTween;

        public RectTransform Transform => _transform;

        public event Action<SwitchItem> OnClick;

        private void OnEnable()
        {
            _transform = GetComponent<RectTransform>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(ClickHandler);
        }

        private void OnDestroy()
        {
            _colorTween?.Kill();
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        public void SetColor(Color color)
        {
            _colorTween?.Kill();

            _text.color = color;
        }

        public void SetColor(Color color, float duration)
        {
            _colorTween?.Kill();

            _colorTween = _text.DOColor(color, duration)
                               .OnComplete(() => _colorTween = null);
        }

        private void ClickHandler()
        {
            OnClick?.Invoke(this);
        }
    }
}