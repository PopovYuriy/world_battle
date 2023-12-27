using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Services;
using Game.Services.Storage;

namespace Game.Field.Mediators
{
    public abstract class GameMediatorAbstract : IGameMediator
    {
        private const int OpposedBaseRowIndex = 0;
        private readonly int[] _baseRowIndexes = {4, 0};
        
        private WordsProvider _wordsProvider;

        public PlayerGameData CurrentPlayer => SessionStorage.Data.LastTurnPlayerId == null 
            ? SessionStorage.Data.Players[0]
            : SessionStorage.Data.Players.First(p => p.Uid != SessionStorage.Data.LastTurnPlayerId);

        public string CurrentWord { get; private set; }

        protected GameField GameField { get; private set; }
        protected IGameSessionStorage SessionStorage { get; private set; }
        protected GameFieldColorsConfig ColorConfig { get; private set; }
        protected string OwnerPlayerId { get; private set; }

        public event Action<char> OnLetterPicked;
        public event Action OnStorageUpdated;

        public void Initialize(GameField gameField, WordsProvider wordsProvider, IGameSessionStorage sessionStorage, 
            GameFieldColorsConfig colorConfig, string ownerPlayerId)
        {
            GameField = gameField;
            _wordsProvider = wordsProvider;
            SessionStorage = sessionStorage;
            ColorConfig = colorConfig;
            OwnerPlayerId = ownerPlayerId;

            GameField.OnLetterPick += LetterPickHandler;
            SessionStorage.Updated += StorageUpdatedHandler;

            ProcessPostInitializing();
        }

        public void Dispose()
        {
            SessionStorage.Updated -= StorageUpdatedHandler;
        }
        
        public void ClearCurrentWord()
        {
            CurrentWord = string.Empty;
            GameField.ResetPickedCells();
        }

        public void ApplyCurrentWord()
        {
            GameField.ApplyWordForPlayer(CurrentPlayer.Uid);

            var player = CurrentPlayer;
            SessionStorage.Data.LastTurnPlayerId = player.Uid;
            
            SessionStorage.Data.Turns ??= new List<string>();
            SessionStorage.Data.Turns.Add(CurrentWord);
            SessionStorage.Save();
            
            ClearCurrentWord();
            
            if (CheckPlayerWin(player))
                ProcessWin();
            else
                ProcessFinishTurn();
        }

        public abstract IReadOnlyList<PlayerGameData> GetOrderedPlayersList();

        protected virtual void ProcessPostInitializing() {}
        
        protected abstract void ProcessWin();
        protected abstract void ProcessFinishTurn();
        protected abstract void StorageUpdatedImpl();
        
        private PlayerGameData GetOpposedPlayer(PlayerGameData player) => SessionStorage.Data.Players
            .First(p => p != player);

        
        private void LetterPickHandler(char letter)
        {
            CurrentWord += letter;
            OnLetterPicked?.Invoke(letter);
        }
        
        private bool CheckPlayerWin(PlayerGameData player)
        {
            var opposedPlayer = GetOpposedPlayer(player);
            return CheckOpposedBaseCaptured(player.Uid) || !CheckAvailableWords(opposedPlayer.Uid);
        }
        
        private void StorageUpdatedHandler(IGameSessionStorage sender)
        {
            StorageUpdatedImpl();
            OnStorageUpdated?.Invoke();
        }

        private bool CheckAvailableWords(string userId)
        {
            var letters = SessionStorage.Data.Grid.GetAvailableCellsForUser(userId).Select(cell => cell.Letter);
            var words = _wordsProvider.GetAvailableWords(letters.ToList(), SessionStorage.Data.Turns);
            return words.Any();
        }
        
        private bool CheckOpposedBaseCaptured(string uid)
        {
            return SessionStorage.Data.Grid.Cells[OpposedBaseRowIndex]
                .Any(cell => cell.PlayerId == uid);
        }
    }
}