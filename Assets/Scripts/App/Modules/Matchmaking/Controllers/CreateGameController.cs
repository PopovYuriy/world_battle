using System;
using System.Linq;
using System.Threading.Tasks;
using App.Modules.Matchmaking.API;
using App.Modules.Matchmaking.Data;
using App.Modules.Matchmaking.Enums;
using Tools.CSharp;
using UnityEngine;
using Utils.Extensions;

namespace App.Modules.Matchmaking.Controllers
{
    public sealed class CreateGameController
    {
        private readonly string[] Keywords = { "пес", "кіт", "ніс" };
        
        public event Action<PendingGameCreateError> OnError;
        public event Action<PendingGameData>        OnCreated;

        private readonly IMatchmakerAPI _api;

        public CreateGameController(IMatchmakerAPI api)
        {
            _api = api;
        }

        public void CreateGame(string playerUid)
        {
            ProcessGameCreating(playerUid).Run();
        }

        private async Task ProcessGameCreating(string playerUid)
        {
            Debug.Log("Matchmaker :: Game creating...");

            var usedKeywords = await _api.GetKeywordsInUseAsync();

            var availableKeywords = usedKeywords == null ? Keywords : Keywords.Except(usedKeywords, StringComparer.OrdinalIgnoreCase);
            var keyword = availableKeywords.Random();

            var result = await _api.CreatePendingGameAsync(keyword, playerUid);

            if (result.Error != PendingGameCreateError.None)
            {
                Debug.Log($"Matchmaker :: Game creating failed : {result.Error}");
                
                OnError?.Invoke(result.Error);
                return;
            }

            Debug.Log("Matchmaker :: Game creating successful");
            
            OnCreated?.Invoke(result.Data);
        }
    }
}