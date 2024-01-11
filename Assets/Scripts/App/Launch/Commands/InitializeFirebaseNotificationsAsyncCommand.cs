using System.Threading.Tasks;
using App.Services.Database;
using App.Services.PushNotifications;
using App.Services.PushNotifications.Providers;
using Core.Commands;
using UnityEngine;
using Zenject;

namespace App.Launch.Commands
{
    public sealed class InitializeFirebaseNotificationsAsyncCommand : ICommandAsync
    {
        private PushNotificationsService _notificationsService;
        private RealtimeDatabase _realtimeDatabase;
        
        [Inject]
        private void Construct(PushNotificationsService notificationsService, RealtimeDatabase realtimeDatabase)
        {
            _notificationsService = notificationsService;
            _realtimeDatabase = realtimeDatabase;
        }

        public async Task Execute()
        {
            if (Application.isEditor)
                return;
            
            var firebasePushNotificationsProvider = new FirebasePushNotificationsProvider();
            _notificationsService.Initialize(firebasePushNotificationsProvider);
            await firebasePushNotificationsProvider.Initialize();
            await _realtimeDatabase.SaveUserNotificationToken(firebasePushNotificationsProvider.Token);
        }
    }
}