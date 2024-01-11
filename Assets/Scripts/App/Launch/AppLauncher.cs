using App.Launch.Commands;
using App.Launch.Signals;
using App.Services;
using CoolishUI;
using Core.Services.Scene;
using UnityEngine;
using Zenject;

namespace App.Launch
{
    public sealed class AppLauncher : MonoBehaviour
    {
        [SerializeField] private SimpleDebugConsole _debugConsole;
        
        private InitializeFirebaseAsyncCommand _initializeFirebaseAsyncCommand;
        private InitializeRealtimeDatabaseAsyncCommand _initializeRealtimeDatabaseAsyncCommand;
        private AuthenticationAsyncCommand _authenticationAsyncCommand;
        private InitializeFirebaseNotificationsAsyncCommand _initializeFirebaseNotificationsAsyncCommand;

        private GameSessionsManager _gameSessionsManager;
        private ScenesLoader _scenesLoader;
        
        private SignalBus _signalBus;

        [Inject]
        private void Construct(InitializeFirebaseAsyncCommand initializeFirebaseAsyncCommand, 
            InitializeRealtimeDatabaseAsyncCommand initializeRealtimeDatabaseAsyncCommand, 
            AuthenticationAsyncCommand authenticationAsyncCommand, 
            InitializeFirebaseNotificationsAsyncCommand initializeFirebaseNotificationsAsyncCommand,
            GameSessionsManager gameSessionsManager, 
            ScenesLoader scenesLoader, 
            SignalBus signalBus)
        {
            _initializeFirebaseAsyncCommand = initializeFirebaseAsyncCommand;
            _initializeRealtimeDatabaseAsyncCommand = initializeRealtimeDatabaseAsyncCommand;
            _authenticationAsyncCommand = authenticationAsyncCommand;
            _initializeFirebaseNotificationsAsyncCommand = initializeFirebaseNotificationsAsyncCommand;
            _gameSessionsManager = gameSessionsManager;
            _scenesLoader = scenesLoader;
            _signalBus = signalBus;
        }

        private async void Awake()
        {
            DontDestroyOnLoad(_debugConsole);
            
            await _scenesLoader.LoadTransitionSceneAsync();
            
            await _initializeFirebaseAsyncCommand.Execute();
            await _initializeRealtimeDatabaseAsyncCommand.Execute();
            await _authenticationAsyncCommand.Execute();
            await _initializeFirebaseNotificationsAsyncCommand.Execute();

            await _gameSessionsManager.InitializeAsync();

            _signalBus.Fire<LaunchFinishedSignal>();
        }
    }
}