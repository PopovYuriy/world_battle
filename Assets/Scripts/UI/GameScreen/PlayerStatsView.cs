using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
{
    public sealed class PlayerAreaView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private GameObject _disableState;
        [SerializeField] private GameObject _activeState;
        [SerializeField] private Color _normalNameTextColor;
        [SerializeField] private Color _disabledNameTextColor;

        public void Initialize(string name, Color color)
        {
            _name.text = name;
            _backgroundImage.color = color;
        }

        public void SetActive(bool isActive)
        {
            _disableState.SetActive(!isActive);
            _activeState.SetActive(isActive);
            _name.color = isActive ? _normalNameTextColor : _disabledNameTextColor;
        }
    }
}