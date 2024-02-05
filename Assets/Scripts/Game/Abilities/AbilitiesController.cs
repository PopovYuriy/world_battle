using System.Linq;
using Core.UI;
using Game.Abilities.Runners;
using Game.Data;
using Game.Field.Mediators;
using Game.Services.Storage;
using TMPro;
using UnityEngine;

namespace Game.Abilities
{
    public sealed class AbilitiesController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerPoints;
        [SerializeField] private AbilityView[] _abilityViews;
        [SerializeField] private AbilityRunnerAbstract[] _runners;
        
        private AbilityConfigsStorage _abilitiesConfig;
        private IGameSessionStorage _gameSessionStorage;
        private IGamePlayController _gamePlayController;
        private UISystem _uiSystem;

        private PlayerGameData _playerData;
        private AbilityRunnerAbstract _currentAbilityRunnerAbstract;
        private AbilityView _currentAbilityView;

        public void Initialize(PlayerGameData playerGameData, AbilityConfigsStorage abilitiesConfig, 
            IGameSessionStorage gameSessionStorage, IGamePlayController gamePlayController, UISystem uiSystem)
        {
            _playerData = playerGameData;
            _abilitiesConfig = abilitiesConfig;
            _gameSessionStorage = gameSessionStorage;
            _gamePlayController = gamePlayController;
            _uiSystem = uiSystem;
            
            foreach (var abilityView in _abilityViews)
            {
                abilityView.Initialize();
                abilityView.OnActivated += AbilityActivated;
            }

            foreach (var abilityRunner in _runners)
                abilityRunner.Initialize(_uiSystem, _gameSessionStorage.Data);
            
            UpdatePlayerAbilitiesInfo();
        }

        private void OnDestroy()
        {
            FinalizeCurrentRunner();
            
            foreach (var abilityView in _abilityViews)
                abilityView.OnActivated -= AbilityActivated;
        }

        public void Activate()
        {
            UpdateViewsInteractable();
        }

        public void Deactivate()
        {
            foreach (var abilityView in _abilityViews)
                abilityView.SetInteractable(false);
        }
        
        public void UpdatePlayerAbilitiesInfo()
        {
            _playerPoints.SetText(_playerData.Points.ToString());
            UpdateViewsInteractable();
            UpdateAbilitiesCost();
        }

        private void UpdateViewsInteractable()
        {
            foreach (var abilityView in _abilityViews)
            {
                var isCurrentUser = _playerData.Uid == _gamePlayController.CurrentPlayer.Uid;
                var isAlreadyUsed = _gameSessionStorage.Data.AbilityData?.InitiatorUid == _playerData.Uid;
                var isPointsEnough = _playerData.Points >= abilityView.Cost;
                var isInteractable = isCurrentUser && !isAlreadyUsed && isPointsEnough && _playerData.IsControllable;
                abilityView.SetInteractable(isInteractable);
            }
        }

        private void UpdateAbilitiesCost()
        {
            foreach (var abilityView in _abilityViews)
            {
                if (!_playerData.AbilitiesCosts.TryGetValue(abilityView.AbilityType, out var cost))
                    _playerData.AbilitiesCosts = _abilitiesConfig.GetDefaultCosts();
                abilityView.SetCost(_playerData.AbilitiesCosts[abilityView.AbilityType]);
            }
        }

        private void AbilityActivated(AbilityView abilityView)
        {
            _currentAbilityView = abilityView;
            
            _gamePlayController.Deactivate();
            
            _currentAbilityRunnerAbstract = _runners.First(runner => runner.AbilityType == abilityView.AbilityType);
            _currentAbilityRunnerAbstract.OnApplied += AbilityAppliedHandler;
            _currentAbilityRunnerAbstract.OnDeclined += AbilityDeclinedHandler;
            _currentAbilityRunnerAbstract.Run(_playerData.Uid);
        }

        private void AbilityAppliedHandler()
        {
            _playerData.Points -= _playerData.AbilitiesCosts[_currentAbilityRunnerAbstract.AbilityType];
            _playerData.AbilitiesCosts[_currentAbilityRunnerAbstract.AbilityType] *= _abilitiesConfig
                .GetConfig(_currentAbilityRunnerAbstract.AbilityType).CostMultiplier;

            _gameSessionStorage.Save();
            
            _gamePlayController.Activate();
            _currentAbilityView.SetCost(_playerData.AbilitiesCosts[_currentAbilityRunnerAbstract.AbilityType]);

            FinalizeCurrentRunner();
            UpdatePlayerAbilitiesInfo();
        }

        private void AbilityDeclinedHandler()
        {
            UpdateViewsInteractable();
            _gamePlayController.Activate();

            FinalizeCurrentRunner();
        }

        private void FinalizeCurrentRunner()
        {
            if (_currentAbilityRunnerAbstract == null) 
                return;
            
            _currentAbilityRunnerAbstract.Finalize();
            _currentAbilityRunnerAbstract.OnApplied -= AbilityAppliedHandler;
            _currentAbilityRunnerAbstract.OnDeclined -= AbilityDeclinedHandler;
            _currentAbilityRunnerAbstract = null;
        }
    }
}