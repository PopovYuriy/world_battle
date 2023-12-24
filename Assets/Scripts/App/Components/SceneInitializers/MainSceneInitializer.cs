using System.Threading.Tasks;
using App.Enums;
using Core.Services.Scene;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace App.Components.SceneInitializers
{
    public sealed class MainSceneInitializer : MonoBehaviour, ISceneInitializer
    {
        [Inject] private UISystem _uiSystem;
        
        public async Task InitializeAsync()
        {
            _uiSystem.ShowScreen(ScreenId.MainMenu);
            await UniTask.Yield();
        }
    }
}