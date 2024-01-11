using System;

namespace App.Services.PushNotifications
{
    public interface INotificationsProvider : IDisposable
    {
        event Action<MessageData> OnMessageReceived;
    }
}