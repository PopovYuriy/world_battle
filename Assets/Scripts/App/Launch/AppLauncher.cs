using App.Launch.Commands;
using App.Launch.Signals;
using App.Services;
using UnityEngine;
using Zenject;

namespace App.Launch
{
    public sealed class AppLauncher : MonoBehaviour
    {
        [Inject] private InitializeFirebaseAsyncCommand _initializeFirebaseAsyncCommand;
        [Inject] private InitializeRealtimeDatabaseAsyncCommand _initializeRealtimeDatabaseAsyncCommand;
        [Inject] private AuthenticationAsyncCommand _authenticationAsyncCommand;

        [Inject] private GameSessionsManager _gameSessionsManager;
        
        [Inject] private SignalBus _signalBus;

        private async void Awake()
        {
            await _initializeFirebaseAsyncCommand.Execute();
            await _initializeRealtimeDatabaseAsyncCommand.Execute();
            await _authenticationAsyncCommand.Execute();

            await _gameSessionsManager.InitializeAsync();
            
            _signalBus.Fire<LaunchFinishedSignal>();
        }
    }
}