using System;
using System.Threading;
using System.Threading.Tasks;
using App.Data.Player;
using App.Modules.GameSessions;
using App.Modules.Matchmaking.API;
using App.Modules.Matchmaking.Controllers;
using App.Modules.Matchmaking.Data;
using App.Modules.Matchmaking.Enums;
using Core.API.Common;
using Cysharp.Threading.Tasks;
using Tools.CSharp;
using UnityEngine;
using Zenject;

namespace App.Modules.Matchmaking
{
    public sealed class Matchmaker : IMatchmaker, IMatchmakerInitializable, IDisposable
    {
        private const int PendingGameTimeMS = 180 * 1000;

        [Inject] private IPlayer              _player;
        [Inject] private IMatchmakerAPI       _api;
        [Inject] private IGameSessionsManager _gameSessionsManager;
        [Inject] private SignalBus            _signalBus;

        private CreateGameController _createGameController;
        private JoinGameController   _joinGameController;

        private PendingGameData         _currentPendingGame;
        private CancellationTokenSource _cancellationTokenSource;

        private IDataObserver<bool> _userConnectedObserver;

        public async Task InitializeAsync()
        {
            Debug.Log("Matchmaker :: initializing...");

            var currentPendingGame = await _api.GetExistPendingGameAsync(_player.Uid);

            Debug.Log($"Matchmaker :: pending game exist : {currentPendingGame != null}");

            if (currentPendingGame == null)
                return;

            if (await _api.CheckPendingGameIsExpiredAsync(currentPendingGame, PendingGameTimeMS))
            {
                Debug.Log("Matchmaker :: game expired");
                await DeletePendingGameAsync();
                return;
            }

            _currentPendingGame = currentPendingGame;

            _userConnectedObserver = await CreateSessionCreatedObserverAsync();

            if (_userConnectedObserver == null)
                await DeletePendingGameAsync();
            else
                _signalBus.Fire(new MatchmakerSignal.PendingGameCreated(new PendingGameCreatedData(_currentPendingGame)));
        }

        public void Dispose()
        {
            DisposeUserConnectedObserver();
            DisposeCancellationToken();
            DisposeCreateGameController();
            DisposeJoinGameController();
        }

        #region Create

        public void CreateGame()
        {
            if (_currentPendingGame != null || _createGameController != null)
            {
                _signalBus.Fire(new MatchmakerSignal.PendingGameCreated(new PendingGameCreatedData(PendingGameCreateError.AlreadyExists)));
                return;
            }

            _createGameController = new CreateGameController(_api);
            
            SubscribeCreateGameController();

            _createGameController.CreateGame(_player.Uid);
        }

        private void SubscribeCreateGameController()
        {
            _createGameController.OnCreated += GameCreatedHandler;
            _createGameController.OnError += GameCreateErrorHandler;
        }

        private void DisposeCreateGameController()
        {
            if (_createGameController == null)
                return;
            
            _createGameController.OnCreated -= GameCreatedHandler;
            _createGameController.OnError -= GameCreateErrorHandler;
            
            _createGameController = null;
        }

        private void GameCreatedHandler(PendingGameData data)
        {
            _currentPendingGame = data;

            DisposeCreateGameController();
            
            GameCreatedHandlerAsync().Run();
        }

        private async Task GameCreatedHandlerAsync()
        {
            var observer = await CreateSessionCreatedObserverAsync();
            if (observer == null)
            {
                await DeletePendingGameAsync();
                return;
            }

            _userConnectedObserver = observer;
            _cancellationTokenSource = new CancellationTokenSource();
            
            _signalBus.Fire(new MatchmakerSignal.PendingGameCreated(new PendingGameCreatedData(_currentPendingGame)));
            
            await RunTimer(_cancellationTokenSource, PendingGameTimeMS);
        }

        private void GameCreateErrorHandler(PendingGameCreateError error)
        {
            DisposeCreateGameController();
            _signalBus.Fire(new MatchmakerSignal.PendingGameCreated(new PendingGameCreatedData(error)));
        }

        private async Task RunTimer(CancellationTokenSource cancellationToken, int ms)
        {
            await UniTask.WaitForSeconds(ms / 1000f);

            if (cancellationToken.IsCancellationRequested)
                return;

            PendingGameExpired();
        }

        private void PendingGameExpired()
        {
            Debug.Log("Matchmaker :: Created game expired");

            DisposeUserConnectedObserver();
            DeletePendingGameAsync().Run();

            _signalBus.Fire<MatchmakerSignal.PendingGameExpired>();
        }
        
        private async Task<IDataObserver<bool>> CreateSessionCreatedObserverAsync()
        {
            var sessionCreatedObserver = await _api.CreateSessionCreatedObserverAsync(_player.Uid);

            if (sessionCreatedObserver == null)
            {
                Debug.Log("Matchmaker :: User connected observer failed");
                return null;
            }

            sessionCreatedObserver.OnChangeOccured += SessionCreatedHandler;
            sessionCreatedObserver.Observe();
            return sessionCreatedObserver;
        }
        
        private void SessionCreatedHandler(bool _)
        {
            SessionCreatedHandlerAsync().Run();
        }

        private async Task SessionCreatedHandlerAsync()
        {
            DisposeUserConnectedObserver();
            
            var result = await _gameSessionsManager.TryLoadAndAddGameAsync(_currentPendingGame.Id, _player.Uid);

            if (!result)
            {
                _signalBus.Fire(new MatchmakerSignal.PendingGameSessionCreated(null));
                return;
            }
            
            var gameId = _currentPendingGame.Id;
            await DeletePendingGameAsync();
            
            _signalBus.Fire(new MatchmakerSignal.PendingGameSessionCreated(gameId));
        }
        
        private void DisposeUserConnectedObserver()
        {
            if (_userConnectedObserver == null)
                return;

            _userConnectedObserver.Stop();
            _userConnectedObserver.Dispose();
            _userConnectedObserver.OnChangeOccured -= SessionCreatedHandler;
            _userConnectedObserver = null;
        }
        
        private async Task DeletePendingGameAsync()
        {
            _cancellationTokenSource?.Cancel();
            DisposeCancellationToken();

            await _api.DeletePendingGameAsync();
            _signalBus.Fire(new MatchmakerSignal.PendingGameDeleted(_currentPendingGame));
            _currentPendingGame = null;

            Debug.Log("Matchmaker :: pending game deleted");
        }
        
        private void DisposeCancellationToken()
        {
            if (_cancellationTokenSource == null)
                return;

            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        #endregion

        #region Join
        
        public void JoinGame(string keyword)
        {
            if (_joinGameController != null)
            {
                _signalBus.Fire(new MatchmakerSignal.PendingGameJoined(new PendingGameJoinedData(PendingGameJoinError.IsAlreadyJoining)));
                return;
            }
            
            _joinGameController = new JoinGameController(_api, PendingGameTimeMS);
            _joinGameController.OnSuccess += JoinGameSuccessHandler;
            _joinGameController.OnFailed += JoinGameFailedHandler;
            _joinGameController.JoinGame(keyword, _player.Uid);
        }

        private void DisposeJoinGameController()
        {
            if (_joinGameController == null)
                return;
            
            _joinGameController.OnSuccess -= JoinGameSuccessHandler;
            _joinGameController.OnFailed -= JoinGameFailedHandler;
            _joinGameController = null;
        }

        private void JoinGameSuccessHandler(PendingGameData data)
        {
            ProceedJoinGameAsync(data).Run();
        }

        private async Task ProceedJoinGameAsync(PendingGameData data)
        {
            var gameSessionController = await _gameSessionsManager.CreateOnlineGame(data.Id, data.HostUid);
            if (gameSessionController == null)
            {
                DisposeJoinGameController();
                _signalBus.Fire(new MatchmakerSignal.PendingGameJoined(new PendingGameJoinedData(PendingGameJoinError.CannotCreateSession)));
                return;
            }
            
            var result = await _api.MarkPendingGameSessionCreatedAsync(data.Id);
            if (result.Error != PendingGameJoinError.None)
            {
                _signalBus.Fire(new MatchmakerSignal.PendingGameJoined(new PendingGameJoinedData(result.Error)));
                return;
            }
            
            _gameSessionsManager.AddGame(gameSessionController);
            
            DisposeJoinGameController();
            _signalBus.Fire(new MatchmakerSignal.PendingGameJoined(new PendingGameJoinedData(data)));
        }
        
        private void JoinGameFailedHandler(PendingGameJoinError error)
        {
            DisposeJoinGameController();
            _signalBus.Fire(new MatchmakerSignal.PendingGameJoined(new PendingGameJoinedData(error)));
        }
        
        #endregion
    }
}