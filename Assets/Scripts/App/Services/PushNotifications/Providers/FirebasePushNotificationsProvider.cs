using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Messaging;
using UnityEngine;
using Utils.Extensions;

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace App.Services.PushNotifications.Providers
{
    public sealed class FirebasePushNotificationsProvider : INotificationsProvider
    {
        public string Token { get; private set; }

        public event Action<MessageData> OnMessageReceived;

        public async Task Initialize()
        {
            #if UNITY_IOS
            await InitializeIOSNotifications();
            #endif

            // Subscribe to messaging events
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += MessageReceivedHandler;

            // Get the token
            await RequestFCMToken();
        }

        public void Dispose()
        {
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= MessageReceivedHandler;
        }
        
        #if UNITY_IOS
        private async Task InitializeIOSNotifications()
        {
            // Request authorization
            using var authRequest = new AuthorizationRequest(AuthorizationOption.Alert |
                                                             AuthorizationOption.Badge |
                                                             AuthorizationOption.Sound, true);
            
            await new WaitUntil(() => authRequest is { IsFinished: true });
            
            if (authRequest.Granted)
                Debug.Log("iOS notification permission granted");
            else if (authRequest.Error.IsNullOrEmpty())
                Debug.LogError($"Authorization error: {authRequest.Error}");
            else
                Debug.Log("iOS notification permission declined");
        }
        #endif
        
        private async Task RequestFCMToken()
        {
            try
            {
                Token = await FirebaseMessaging.GetTokenAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get FCM token: {ex.Message}");
            }
        }
        
        private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            Debug.Log($"Received Registration Token: {token.Token}");
            // Send this token to your server
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