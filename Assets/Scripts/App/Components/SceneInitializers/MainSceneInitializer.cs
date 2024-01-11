using System.Collections.Generic;
using System.Threading.Tasks;
using App.Data.Player;
using App.Enums;
using App.Services;
using App.Services.PushNotifications;
using App.Signals;
using Core.Services.Scene;
using Core.UI;
using Cysharp.Threading.Tasks;
using UI.Popups.EnterNamePopup;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace App.Components.SceneInitializers
{
    public sealed class MainSceneInitializer : MonoBehaviour, ISceneInitializer
    {
        private UISystem _uiSystem;
        private IPlayer _player;
        private PushNotificationsService _pushNotificationsService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(UISystem uiSystem, IPlayer player, PushNotificationsService pushNotificationsService,
            SignalBus signalBus)
        {
            _uiSystem = uiSystem;
            _player = player;
            _pushNotificationsService = pushNotificationsService;
            _signalBus = signalBus;
        }

        public async Task InitializeAsync()
        {
            if (!PrefsProvider.FirstTimeNameChanged)
            {
                await UniTask.Yield();
                _uiSystem.ShowScreen(ScreenId.MainMenu);
                
                PrefsProvider.FirstTimeNameChanged = true;
                _uiSystem.ShowPopup(PopupId.UpdateName, new UpdateNamePopupData(_player.Name));
                return;
            }
            var unhandledPushMessage = _pushNotificationsService.GetUnhandledMessage();
            if (unhandledPushMessage is {IsOpened: true})
            {
                switch (unhandledPushMessage.Type)
                {
                    case MessageType.OpenGame:
                        ProcessOpenGame(unhandledPushMessage.Data);
                        break;
            
                    default:
                        Debug.LogWarning($"Unhandled message type: {unhandledPushMessage.Type}");
                        break;
                }
            }
            else
            {
                _uiSystem.ShowScreen(ScreenId.MainMenu);
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