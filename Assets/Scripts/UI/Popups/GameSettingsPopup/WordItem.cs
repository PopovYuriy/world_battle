using TMPro;
using UnityEngine;

namespace UI.Popups.GameSettingsPopup
{
    public sealed class WordItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _backgroundtransform;
        [SerializeField] private TextMeshProUGUI _wordField;

        public void SetWord(string word)
        {
            _wordField.SetText(word);
            FitSize();
        }

        private void FitSize()
        {
            var preferredSize = _wordField.GetPreferredValues(_wordField.text);
            var rect = _backgroundtransform.rect;
            if (preferredSize.x >= rect.width)
                return;

            var anchoredX = (_wordField.horizontalAlignment == HorizontalAlignmentOptions.Right
                ? rect.width - preferredSize.x
                : preferredSize.x - rect.width) / 2;
            
            _backgroundtransform.anchoredPosition = new Vector2(anchoredX, _backgroundtransform.anchoredPosition.y);
            _backgroundtransform.sizeDelta = new Vector2(preferredSize.x - rect.width, _backgroundtransform.sizeDelta.y);
        }
    }
}