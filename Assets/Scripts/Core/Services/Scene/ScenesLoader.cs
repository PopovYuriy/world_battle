using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Enums;
using Cysharp.Threading.Tasks;
using Tools.CSharp;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Services.Scene
{
    public sealed class ScenesLoader
    {
        private const int DelayAfterTransitionLoaded = 100;
        
        public async Task LoadScene(SceneId sceneId, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var sceneKey = sceneId.ToString();
            await SceneManager.LoadSceneAsync(sceneKey, mode).ToUniTask();
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

        public async Task UnloadScene(SceneId sceneId)
        {
            var sceneKey = sceneId.ToString();
            var scene = SceneManager.GetSceneByName(sceneKey);
            if (scene.isLoaded)
                await SceneManager.UnloadSceneAsync(scene);
        }

        public async Task LoadTransitionSceneWithCancellationToken(CancellationTokenSource cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await LoadTransitionSceneAsync();
            
            WaitForTransitionClose(cancellationToken).Run();
        }

        public async Task LoadTransitionSceneAsync()
        {
            await SceneManager.LoadSceneAsync(SceneId.Transition.ToString(), LoadSceneMode.Additive);
            await UniTask.Delay(DelayAfterTransitionLoaded);
        }

        public async Task UnloadTransitionScene()
        {
            var scene = SceneManager.GetSceneByName(SceneId.Transition.ToString());
            if (!scene.isLoaded)
                return;
            
            await UnloadScene(SceneId.Transition);
        }

        private async Task WaitForTransitionClose(CancellationTokenSource cancellationToken)
        {
            
            await UniTask.WaitUntilCanceled(cancellationToken.Token, PlayerLoopTiming.Update, true);
            await UnloadTransitionScene();
        }
    }
}