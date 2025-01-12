using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data.Player;
using App.Services.Database.Utils;
using Firebase.Database;
using Newtonsoft.Json;
using Zenject;

namespace App.Services.Database
{
    public sealed class RealtimeDatabase
    {
        [Inject] private IPlayer _player;
        
        private DatabaseReference _dataBaseRoot;
        
        public void Initialize(DatabaseReference rootReference)
        {
            _dataBaseRoot = rootReference;
        }

        public async Task TrySaveUser(IPlayer player)
        {
            var usersRoot = GetUsersRoot();
            var userDataSnapshot = await usersRoot
                .OrderByChild(Player.UidKey)
                .EqualTo(player.Uid)
                .GetValueAsync();

            if (userDataSnapshot.Exists)
            {
                var playerDataSnapshot = userDataSnapshot.Children.First();
                var jsonData = playerDataSnapshot.GetRawJsonValue();
                var existPlayerData = JsonConvert.DeserializeObject<Player>(jsonData);
                if (existPlayerData.Name == player.Name)
                    return;
                
                await playerDataSnapshot.Reference.UpdateChildrenAsync(new Dictionary<string, object>
                {
                    {Player.NameKey, player.Name}
                });
            }
            else
            {
                var userPush = usersRoot.Push();
                await userPush.SetRawJsonValueAsync(JsonConvert.SerializeObject(player));
            }
        }

        public async Task SaveUserNotificationToken(string token)
        {
            var usersRoot = GetUsersRoot();
            var userDataSnapshot = await usersRoot
                                         .OrderByChild(Player.UidKey)
                                         .EqualTo(_player.Uid)
                                         .GetValueAsync();

            if (!userDataSnapshot.Exists)
                throw new Exception("There is no saved player. You must to authenticate user before saving a token");

            var playerDataSnapshot = userDataSnapshot.Children.First();

            await playerDataSnapshot.Reference.UpdateChildrenAsync(new Dictionary<string, object>
            {
                { Player.TokenKey, token }
            });
        }

        private DatabaseReference GetUsersRoot() => _dataBaseRoot.Child(DatabasePathProvider.Users);
    }
}