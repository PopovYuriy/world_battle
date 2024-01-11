using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Messaging;
using UnityEngine;

namespace App.Services.PushNotifications.Providers
{
    public sealed class FirebasePushNotificationsProvider : INotificationsProvider
    {
        private bool _tokenReceived;
        
        public string Token { get; private set; }

        public event Action<MessageData> OnMessageReceived;

        public async Task Initialize()
        {
            FirebaseMessaging.TokenReceived += TokenReceivedHandler;
            FirebaseMessaging.MessageReceived += MessageReceivedHandler;

            await UniTask.WaitUntil(() => _tokenReceived);
        }

        public void Dispose()
        {
            FirebaseMessaging.MessageReceived -= MessageReceivedHandler;
        }
        
        private void TokenReceivedHandler(object sender, TokenReceivedEventArgs e)
        {
            FirebaseMessaging.TokenReceived -= TokenReceivedHandler;
            Debug.Log($"Token received :: {e.Token}");
            Token = e.Token;
            _tokenReceived = true;
        }

        private void MessageReceivedHandler(object sender, MessageReceivedEventArgs e)
        {
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