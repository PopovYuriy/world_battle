using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "AbilityConfigsStorage", menuName = "Configs/AbilityConfigsStorage", order = 0)]
    public sealed class AbilityConfigsStorage : ScriptableObject
    {
        [SerializeField] private AbilityConfig[] _configs;

        public AbilityConfig GetConfig(AbilityType type) => _configs.First(c => c.Type == type);

        public Dictionary<AbilityType, int> GetDefaultCosts() => _configs.ToDictionary(c => c.Type, c => c.InitialCost);
    }

    [Serializable]
    public sealed class AbilityConfig
    {
        [field: SerializeField] public AbilityType Type { get; private set; }
        [field: SerializeField] public int InitialCost { get; private set; }
        [field: SerializeField] public int CostMultiplier { get; private set; }
    }
}