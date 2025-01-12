using System.Threading.Tasks;
using Core.Commands;
using Zenject;

namespace App.Modules.Matchmaking.Commands
{
    public sealed class InitializeMatchmakerCommandAsync : ICommandAsync
    {
        [Inject] private IMatchmakerInitializable MatchmakerInitializable { get; set; }
        
        public async Task Execute()
        {
            await MatchmakerInitializable.InitializeAsync();
        }
    }
}