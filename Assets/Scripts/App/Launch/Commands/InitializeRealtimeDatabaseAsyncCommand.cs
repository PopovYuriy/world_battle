using System.Threading.Tasks;
using App.Services.Database;
using Core.Commands;
using Firebase.Database;
using Zenject;

namespace App.Launch.Commands
{
    public sealed class InitializeRealtimeDatabaseAsyncCommand : ICommandAsync
    {
        [Inject] private RealtimeDatabase _realtimeDatabase;
        
        public async Task Execute()
        {
            var rootReference = FirebaseDatabase.DefaultInstance.RootReference;
            _realtimeDatabase.Initialize(rootReference);
        }
    }
}