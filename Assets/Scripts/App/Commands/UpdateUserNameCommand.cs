using System.Threading.Tasks;
using App.Data.Player;
using App.Services.Database;
using App.Signals;
using Core.Commands;
using Firebase.Auth;
using Zenject;

namespace App.Commands
{
    public sealed class UpdateUserNameCommand : ICommandAsync
    {
        private UpdateUserNameSignal _signal;
        private IPlayerMutable _player;
        private RealtimeDatabase _database;

        [Inject]
        private void Construct(UpdateUserNameSignal signal, IPlayerMutable player, RealtimeDatabase database)
        {
            _signal = signal;
            _player = player;
            _database = database;
        }

        public async Task Execute()
        {
            var user = FirebaseAuth.DefaultInstance.CurrentUser;
            var profile = new UserProfile {DisplayName = _signal.Name};
            await user.UpdateUserProfileAsync(profile);

            _player.SetName(user.DisplayName);
            await _database.TrySaveUser(_player);
        }
    }
}