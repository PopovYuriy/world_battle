using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Services.Scene
{
    public sealed class ScenesLoader
    {
        public async Task LoadScene(string sceneKey)
        {
            await SceneManager.LoadSceneAsync(sceneKey).ToUniTask();
            var scene = SceneManager.GetSceneByName(sceneKey);
            var sceneInitializerGO = scene.GetRootGameObjects()
                .FirstOrDefault(go => go.GetComponent<ISceneInitializer>() != null);

            if (sceneInitializerGO == null)
            {
                Debug.LogWarning($"Scene with key {sceneKey} has no component {nameof(ISceneInitializer)}");
                return;
            }

            var sceneInitializer = sceneInitializerGO.GetComponent<ISceneInitializer>();
            await UniTask.Yield();
            await sceneInitializer.InitializeAsync();
        }
    }
}