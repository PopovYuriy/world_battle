using System;
using System.Threading.Tasks;
using App.Modules.Matchmaking.API;
using App.Modules.Matchmaking.Data;
using App.Modules.Matchmaking.Enums;
using Tools.CSharp;
using UnityEngine;

namespace App.Modules.Matchmaking.Controllers
{
    public sealed class JoinGameController
    {
        public event Action<PendingGameData> OnSuccess;
        public event Action<PendingGameJoinError> OnFailed;
        
        private readonly IMatchmakerAPI _api;
        private readonly int            _expirationTime;

        public JoinGameController(IMatchmakerAPI api, int expirationTime)
        {
            _api = api;
            _expirationTime = expirationTime;
        }

        public void JoinGame(string keyword, string userId)
        {
            JoinGameAsync(keyword, userId).Run();
        }

        private async Task JoinGameAsync(string keyword, string userId)
        {
            Debug.Log("Matchmaker :: Game joining...");
            
            var result = await _api.TryJoinToGameAsync(keyword, userId, _expirationTime);

            if (result.Error != PendingGameJoinError.None)
            {
                Debug.Log($"Matchmaker :: Game joining failed : {result.Error}");
                OnFailed?.Invoke(result.Error);
                return;
            }
            
            Debug.Log("Matchmaker :: Game joining successful");
            OnSuccess?.Invoke(result.Data);
        }
    }
}