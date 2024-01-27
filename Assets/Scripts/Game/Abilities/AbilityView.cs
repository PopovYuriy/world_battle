using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Abilities
{
    public sealed class AbilityView : MonoBehaviour
    {
        [field: SerializeField] public AbilityType AbilityType { get; private set; }
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _costField;
        [SerializeField] private GameObject _disableCover;

        public int Cost { get; private set; }

        public event Action<AbilityView> OnActivated;

        public void Initialize()
        {
            _button.onClick.AddListener(ClickHandler);
            _disableCover.SetActive(false);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(ClickHandler);
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
            _disableCover.SetActive(!isInteractable);
        }

        public void SetCost(int cost)
        {
            Cost = cost;
            _costField.SetText(cost.ToString());
        }

        private void ClickHandler()
        {
            OnActivated?.Invoke(this);
        }
    }
}