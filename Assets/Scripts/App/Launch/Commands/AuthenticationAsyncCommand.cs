using System.Threading.Tasks;
using App.Data.Player;
using App.Services.Database;
using Core.Commands;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace App.Launch.Commands
{
    public sealed class AuthenticationAsyncCommand : ICommandAsync
    {
        [Inject] private IPlayerMutable _player;
        [Inject] private RealtimeDatabase _database;
        
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
            
            SetUserData(user);
            await _database.TrySaveUser(_player);
        }
        
        private void SetUserData(FirebaseUser firebaseUser)
        {
            var name = firebaseUser.DisplayName.IsNullOrEmpty() ? Application.isEditor ? "Editor" : "Device" : firebaseUser.DisplayName;
            _player.SetUidAndName(firebaseUser.UserId, name);
        }
    }
}