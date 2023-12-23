using Game.Data;
using UnityEngine;
using Zenject;

namespace App.Installer
{
    [CreateAssetMenu(fileName = "ConfigsInstaller", menuName = "Installers/ConfigsInstaller")]
    public sealed class ConfigsInstaller : ScriptableObjectInstaller<ConfigsInstaller>
    {
        [SerializeField] private GameFieldColorsConfig _gameFieldColorsConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_gameFieldColorsConfig);
        }
    }
}