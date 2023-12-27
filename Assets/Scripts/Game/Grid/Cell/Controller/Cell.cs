using System;
using Game.Grid.Cell.Enum;
using Game.Grid.Cell.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Grid.Cell.Controller
{
    public sealed class Cell : MonoBehaviour
    {
        [SerializeField] private Image _occupiedState;
        [SerializeField] private Color _occupiedStateTextColor;
        [SerializeField] private Image _defaultState;
        [SerializeField] private Color _defaultStateTextColor;
        [SerializeField] private GameObject _pickedView;
        [SerializeField] private GameObject _disabledCoverView;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _letter;
        [SerializeField] private TextMeshProUGUI _points;

        public CellState State { get; private set; }
        public CellModel Model { get; private set; }
        
        public event Action<Cell> OnClick;

        private void Awake() => _button.onClick.AddListener(ClickHandler);

        private void OnDestroy() => _button.onClick.RemoveListener(ClickHandler);

        public void SetModel(CellModel model)
        {
            if (Model != null)
                Model.OnChanged -= ModelChangedHandler;
            
            Model = model;
            Model.OnChanged += ModelChangedHandler;
            UpdateView(Model);
        }

        public void SetState(CellState state)
        {
            State = state;
            var isDefault = state == CellState.Default;
            _occupiedState.gameObject.SetActive(!isDefault);
            _defaultState.gameObject.SetActive(isDefault);

            var textColor = isDefault ? _defaultStateTextColor : _occupiedStateTextColor;
            _letter.color = textColor;
            _points.color = textColor;
        }

        public void SetColor(Color color)
        {
            _occupiedState.color = color;
        }
        
        public void SetPicked(bool isPicked) => _pickedView.SetActive(isPicked);

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }

        public void SetReachable(bool isReachable)
        {
            _disabledCoverView.SetActive(!isReachable);
        }

        public void UpdatePoints()
        {
            _points.SetText(Model.Points.ToString());
        }

        private void ModelChangedHandler() => UpdateView(Model);

        private void UpdateView(CellModel model)
        {
            _letter.SetText(new[] {model.Letter});
            UpdatePoints();
        }
        
        private void ClickHandler() => OnClick?.Invoke(this);
    }
}