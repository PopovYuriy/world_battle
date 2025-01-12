using System.Threading.Tasks;
using App.Data.Player;
using App.Modules.GameSessions.API;
using Core.Commands;
using Zenject;

namespace App.Modules.GameSessions.Commands
{
    public sealed class InitializeGameSessionsManagerCommandAsync : ICommandAsync
    {
        [Inject] private IGameSessionsManagerInitializable GameSessionsManager { get; set; }
        [Inject] private IGameSessionsAPI                  API                 { get; set; }
        [Inject] private IPlayer                           Player              { get; set; }

        public InitializeGameSessionsManagerCommandAsync()
        {
        }

        public async Task Execute()
        {
            var controllers = await API.LoadExistGamesAsync(Player.Uid);
            GameSessionsManager.Initialize(controllers);
        }
    }
}