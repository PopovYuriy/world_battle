using System.Threading.Tasks;
using App.Launch.Commands;
using App.Launch.Signals;
using App.Modules.GameSessions;
using App.Modules.GameSessions.Commands;
using App.Modules.Matchmaking.Commands;
using CoolishUI;
using Core.Services.Scene;
using Tools.CSharp;
using UnityEngine;
using Zenject;

namespace App.Launch
{
    public sealed class AppLauncher : MonoBehaviour
    {
        [SerializeField] private SimpleDebugConsole _debugConsole;

        [Inject] private InitializeFirebaseAsyncCommand              _initializeFirebaseAsyncCommand;
        [Inject] private InitializeRealtimeDatabaseAsyncCommand      _initializeRealtimeDatabaseAsyncCommand;
        [Inject] private AuthenticationAsyncCommand                  _authenticationAsyncCommand;
        [Inject] private InitializeFirebaseNotificationsAsyncCommand _initializeFirebaseNotificationsAsyncCommand;
        [Inject] private InitializeMatchmakerCommandAsync            _initializeMatchmakerCommandAsync;
        [Inject] private InitializeGameSessionsManagerCommandAsync   _initializeGameSessionsManagerCommandAsync;
        [Inject] private ScenesLoader                                _scenesLoader;
        [Inject] private SignalBus                                   _signalBus;

        private void Awake()
        {
            LaunchAsync().Run();
        }

        private async Task LaunchAsync()
        {
            DontDestroyOnLoad(_debugConsole);

            await _scenesLoader.LoadTransitionSceneAsync();

            await _initializeFirebaseAsyncCommand.Execute();
            await _initializeRealtimeDatabaseAsyncCommand.Execute();
            await _authenticationAsyncCommand.Execute();
            await _initializeFirebaseNotificationsAsyncCommand.Execute();
            await _initializeMatchmakerCommandAsync.Execute();
            await _initializeGameSessionsManagerCommandAsync.Execute();

            _signalBus.Fire<LaunchFinishedSignal>();
        }
    }
}