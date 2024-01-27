using Game.Abilities;
using Game.Data;
using UnityEngine;
using Zenject;

namespace App.Installer
{
    [CreateAssetMenu(fileName = "ConfigsInstaller", menuName = "Installers/ConfigsInstaller")]
    public sealed class ConfigsInstaller : ScriptableObjectInstaller<ConfigsInstaller>
    {
        [SerializeField] private GameFieldColorsConfig _gameFieldColorsConfig;
        [SerializeField] private AbilityConfigsStorage _abilitiesConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_gameFieldColorsConfig);
            Container.BindInstance(_abilitiesConfig);
        }
    }
}