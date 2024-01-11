using System;
using System.Collections.Generic;
using App.Signals;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace App.Services.PushNotifications
{
    public sealed class PushNotificationsService : IDisposable
    {
        private INotificationsProvider _notificationsProvider;
        private SignalBus _signalBus;

        private MessageData _unhandledMessage;

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize(INotificationsProvider notificationsProvider)
        {
            _notificationsProvider = notificationsProvider;
            _notificationsProvider.OnMessageReceived += MessageReceivedHandler;
            Debug.Log($"{nameof(PushNotificationsService)} initialized.");
        }

        public void Dispose()
        {
            _notificationsProvider.OnMessageReceived -= MessageReceivedHandler;
            _notificationsProvider.Dispose();
            Debug.Log($"{nameof(PushNotificationsService)} disposed.");
        }
        
        public MessageData GetUnhandledMessage()
        {
            var messageData = _unhandledMessage;
            _unhandledMessage = null;
            return messageData;
        }

        private void MessageReceivedHandler(MessageData messageData)
        {
            _unhandledMessage = messageData;
            return;
            switch (messageData.Type)
            {
                case MessageType.OpenGame:
                    ProcessOpenGame(messageData.Data);
                    break;
                
                default:
                    Debug.LogWarning($"Unhandled message type: {messageData.Type}");
                    break;
            }
        }

        private void ProcessOpenGame(IDictionary<string, string> data)
        {
            if (!data.TryGetValue(MessageDataFields.GameId, out var gameId))
            {
                Debug.LogWarning("Cannot open game. Message data has no game id");
                return;
            }

            if (gameId.IsNullOrEmpty())
            {
                Debug.LogWarning("Cannot open game. Game id is null or empty");
                return;
            }
            
            Debug.Log($"Open game by game id from push notifications: {gameId}");
            
            _signalBus.Fire(new StartExistGameSignal(gameId));
        }
    }
}