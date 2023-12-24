using System.Threading.Tasks;
using App.Enums;
using Core.Commands;
using Core.Services.Scene;
using Zenject;

namespace App.Launch.Commands
{
    public sealed class LaunchFinishedCommand : ICommandAsync
    {
        [Inject] private ScenesLoader _scenesLoader;
        
        public async Task Execute()
        {
            await _scenesLoader.LoadScene(SceneId.Main);
        }
    }
}