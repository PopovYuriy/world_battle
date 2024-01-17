using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Components
{
    public class TextButton : Button
    {
        [SerializeField] private TextMeshProUGUI _label;

        public void SetLabelText(string label)
        {
            _label.SetText(label);
        }
    }
}