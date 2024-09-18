using System.Threading.Tasks;
using App.Data.Player;
using App.Services.Database;
using App.Signals;
using Core.Commands;
using Firebase.Auth;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace App.Launch.Commands
{
    public sealed class AuthenticationAsyncCommand : ICommandAsync
    {
        private const string DefaultPlayerNamePrefix = "Гравець-"; 
        
        private IPlayerMutable _player;
        private RealtimeDatabase _database;

        [Inject]
        private void Construct(IPlayerMutable player, RealtimeDatabase database)
        {
            _player = player;
            _database = database;
        }

        public async Task Execute()
        {
            var auth = FirebaseAuth.DefaultInstance;
            var user = auth.CurrentUser;
            if (user == null)
            {
                var authResult = await auth.SignInAnonymouslyAsync();
                user = authResult.User;
                Debug.Log($"Created new user with id: {user.UserId}");
            }

            _player.SetUid(user.UserId);
            
            if (user.DisplayName.IsNullOrEmpty())
            {
                var name = CreateNewPlayerName(user.UserId);
                _player.SetName(name);
                await _database.TrySaveUser(_player);
            }
            else
            {
                _player.SetName(user.DisplayName);
            }
        }

        private string CreateNewPlayerName(string uid) => Application.isEditor 
            ? "Editor" : DefaultPlayerNamePrefix + uid[..5];
    }
}