using System;
using Core.UI;
using Game.Data;
using UnityEngine;

namespace Game.Abilities.Runners
{
    public abstract class AbilityRunnerAbstract : MonoBehaviour
    {
        [field: SerializeField] public AbilityType AbilityType { get; private set; }
        
        public event Action<AbilityData> OnApplied;
        public event Action OnDeclined;

        public abstract void Initialize(UISystem uiSystem);
        public abstract void Run(string initiatorUid);
        public abstract void Finalize();

        protected void ProcessApply(AbilityData data)
        {
            OnApplied?.Invoke(data);
        }

        protected void ProcessDecline()
        {
            OnDeclined?.Invoke();
        }
    }
}