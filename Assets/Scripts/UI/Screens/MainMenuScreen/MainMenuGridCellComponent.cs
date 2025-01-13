using System;
using TMPro;
using UI.Screens.MainMenuScreen.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.MainMenuScreen
{
    public sealed class MainMenuGridCellComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _letter;
        [SerializeField] private Image           _defaultState;
        [SerializeField] private Image           _capturedState;

        [SerializeField] private ColorPair _ownerCollors;
        [SerializeField] private ColorPair _oppositCollors;

        public void SetContent(string letter, GridCellState state, bool isActive)
        {
            _letter.SetText(letter);
            _defaultState.gameObject.SetActive(state == GridCellState.Default);
            _capturedState.gameObject.SetActive(state != GridCellState.Default);

            if (state == GridCellState.Default) 
                return;
            
            var colorPair = state == GridCellState.Owner ? _ownerCollors : _oppositCollors;
            _capturedState.color = isActive ? colorPair.Active : colorPair.NonActive;
        }

        [Serializable]
        private sealed class ColorPair
        {
            [field: SerializeField] public Color Active    { get; private set; }
            [field: SerializeField] public Color NonActive { get; private set; }
        }
    }
}