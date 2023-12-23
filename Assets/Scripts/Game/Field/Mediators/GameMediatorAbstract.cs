using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Services;
using Game.Services.Storage;
using ModestTree;
using UnityEngine;

namespace Game.Field.Mediators
{
    public abstract class GameMediatorAbstract : IGameMediator
    {
        private readonly int[] _baseRowIndexes = {4, 0};
        
        private WordsProvider _wordsProvider;

        protected PlayerGameData CurrentPlayer => SessionStorage.Data.LastTurnPlayerId == null 
            ? SessionStorage.Data.Players[0]
            : SessionStorage.Data.Players.First(p => p.Uid != SessionStorage.Data.LastTurnPlayerId);

        protected GameField GameField { get; private set; }
        protected IGameSessionStorage SessionStorage { get; private set; }
        protected GameFieldColorsConfig ColorConfig { get; private set; }

        public void Initialize(GameField gameField, IGameSessionStorage sessionStorage, GameFieldColorsConfig colorConfig, string ownerPlayerId)
        {
            GameField = gameField;
            SessionStorage = sessionStorage;
            ColorConfig = colorConfig;

            var letters = sessionStorage.Data.Grid.Cells
                .SelectMany(list => list.Select(c => c.Letter)).ToArray();
            _wordsProvider = new WordsProvider();
            _wordsProvider.Initialize(letters);

            GameField.Initialize(SessionStorage.Data.Players, ownerPlayerId);
            GameField.OnApplyClicked += WordApplyClickHandler;

            SessionStorage.Updated += StorageUpdatedHandler;

            ProcessPostInitializing();
        }

        public void Dispose()
        {
            GameField.OnApplyClicked -= WordApplyClickHandler;
            SessionStorage.Updated -= StorageUpdatedHandler;
        }
        
        protected virtual void ProcessPostInitializing() {}
        
        protected abstract void ProcessWin();
        protected abstract void ProcessFinishTurn();
        protected abstract void StorageUpdatedImpl();
        
        protected PlayerGameData GetOpposedPlayer(PlayerGameData player) => SessionStorage.Data.Players
            .First(p => p != player);

        private void WordApplyClickHandler(string word)
        {
            if (SessionStorage.Data.Turns != null && SessionStorage.Data.Turns.Contains(word))
            {
                Debug.Log($"Word '{word}' is already used");
                GameField.ClearTurn();
                return;
            }

            if (!_wordsProvider.IsValidWord(word))
            {
                Debug.Log($"Word '{word}' is invalid");
                GameField.ClearTurn();
                return;
            }
            
            GameField.ApplyWord(CurrentPlayer.Uid);
            GameField.ClearTurn();

            var player = CurrentPlayer;
            SessionStorage.Data.LastTurnPlayerId = player.Uid;
            
            SessionStorage.Data.Turns ??= new List<string>();
            SessionStorage.Data.Turns.Add(word);
            SessionStorage.Save();
            
            if (CheckWin(player))
                ProcessWin();
            else
                ProcessFinishTurn();
        }
        
        private bool CheckWin(PlayerGameData player)
        {
            var opposedPlayer = GetOpposedPlayer(player);
            return CheckBaseCaptured(player, opposedPlayer) ||
                   !CheckVariants(opposedPlayer.Uid);
        }
        
        private void StorageUpdatedHandler(IGameSessionStorage sender)
        {
            StorageUpdatedImpl();
        }

        private bool CheckVariants(string userId)
        {
            var letters = SessionStorage.Data.Grid.GetAvailableCellsForUser(userId).Select(cell => cell.Letter);
            var words = _wordsProvider.GetAvailableWords(letters.ToList(), SessionStorage.Data.Turns);
            return words.Any();
        }
        
        private bool CheckBaseCaptured(PlayerGameData player, PlayerGameData opposedPlayer)
        {
            var playerIndex = SessionStorage.Data.Players.IndexOf(player);
            return SessionStorage.Data.Grid.Cells[_baseRowIndexes[playerIndex]]
                .Any(cell => cell.PlayerId == opposedPlayer.Uid);
        }
    }
}