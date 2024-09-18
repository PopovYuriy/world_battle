using System;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Messaging;
using UnityEngine;

namespace App.Services.PushNotifications.Providers
{
    public sealed class FirebasePushNotificationsProvider : INotificationsProvider
    {
        public string Token { get; private set; }

        public event Action<MessageData> OnMessageReceived;

        public async Task Initialize()
        {
            Token = await FirebaseMessaging.GetTokenAsync();
            
            FirebaseMessaging.MessageReceived += MessageReceivedHandler;
        }

        public void Dispose()
        {
            FirebaseMessaging.MessageReceived -= MessageReceivedHandler;
        }

        private void MessageReceivedHandler(object sender, MessageReceivedEventArgs e)
        {
            Debug.Log("MessageReceivedHandler");
            
            if (!e.Message.Data.TryGetValue(MessageDataFields.Type, out var messageTypeString))
            {
                Debug.LogWarning("Received message has no type");
                return;
            }

            if (!Enum.TryParse(typeof(MessageType), messageTypeString, out var messageTypeObj))
            {
                Debug.LogWarning($"Received unknown message type {messageTypeString}");
                return;
            }

            var messageType = (MessageType) messageTypeObj;
            var data = e.Message.Data.ToDictionary(kv => kv.Key,
                kv => kv.Value);
            var messageData = new MessageData(messageType, data, e.Message.NotificationOpened);
            
            Debug.Log($"Message received :: {messageData}");
            
            OnMessageReceived?.Invoke(messageData);
        }
    }
}