using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.PickLetterPopup
{
    [RequireComponent(typeof(Button))]
    public sealed class LetterItem : MonoBehaviour
    {
        [field: SerializeField] public char Letter { get; private set; }
        
        [SerializeField] private TextMeshProUGUI _letterField;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _pickedState;

        public event Action<LetterItem> OnClicked;
        
        private void OnValidate()
        {
            _letterField ??= GetComponentInChildren<TextMeshProUGUI>();
            _button ??= GetComponent<Button>();
            _letterField.SetText(Letter.ToString());
        }

        private void Start()
        {
            _button.onClick.AddListener(ClickHandler);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(ClickHandler);
        }

        public void SetPickedState(bool isPicked)
        {
            _pickedState.SetActive(isPicked);
        }

        private void ClickHandler()
        {
            OnClicked?.Invoke(this);
        }
    }
}