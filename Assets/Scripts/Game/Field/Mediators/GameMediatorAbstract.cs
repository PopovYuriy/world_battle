using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Services.Storage;
using UnityEngine;

namespace Game.Field.Mediators
{
    public abstract class GameMediatorAbstract : IGameMediator
    {
        public PlayerGameData CurrentPlayer => SessionStorage.Data.LastTurnPlayerId == null 
            ? SessionStorage.Data.Players[0]
            : SessionStorage.Data.Players.First(p => p.Uid != SessionStorage.Data.LastTurnPlayerId);

        public string CurrentWord { get; private set; }

        protected GameField GameField { get; private set; }
        protected IGameSessionStorage SessionStorage { get; private set; }
        protected GameFieldColorsConfig ColorConfig { get; private set; }
        protected string OwnerPlayerId { get; private set; }

        public event Action OnWordChanged;
        public event Action OnStorageUpdated;
        public event Action<WinData> OnWin;

        public void Initialize(GameField gameField, IGameSessionStorage sessionStorage, GameFieldColorsConfig colorConfig, 
            string ownerPlayerId)
        {
            GameField = gameField;
            SessionStorage = sessionStorage;
            ColorConfig = colorConfig;
            OwnerPlayerId = ownerPlayerId;

            GameField.OnPickedLettersChanged += PickedLettersChangedHandler;
            SessionStorage.Updated += StorageUpdatedHandler;

            ProcessPostInitializing();
            
            if (SessionStorage.Data.WinData != null)
                ProcessWin();
        }

        public void Dispose()
        {
            GameField.OnPickedLettersChanged -= PickedLettersChangedHandler;
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

            var playerUid = CurrentPlayer.Uid;
            SessionStorage.Data.LastTurnPlayerId = playerUid;
            
            SessionStorage.Data.Turns ??= new List<string>();
            SessionStorage.Data.Turns.Add(CurrentWord);

            var isWin = CheckPlayerWin(playerUid);
            if (isWin)
                SessionStorage.Data.WinData = new WinData(playerUid, WinReason.BaseCaptured);
            
            SessionStorage.Save();
            
            ClearCurrentWord();
            
            if (isWin)
                ProcessWin();
            else
                ProcessFinishTurn();
        }

        public abstract IReadOnlyList<PlayerGameData> GetOrderedPlayersList();

        public void DeleteGame()
        {
            SessionStorage.Delete();
        }

        protected virtual void ProcessPostInitializing() {}
        protected virtual void ProcessWinImpl(WinData winData) { }

        protected abstract void ProcessFinishTurn();
        protected abstract void StorageUpdatedImpl();

        public void ProcessWin()
        {
            var winData = SessionStorage.Data.WinData;
            GameField.TurnOffCellsInteractable();
            ProcessWinImpl(winData);
            OnWin?.Invoke(winData);
        }

        private void PickedLettersChangedHandler(string pickedWord)
        {
            CurrentWord = pickedWord;
            OnWordChanged?.Invoke();
        }
        
        private bool CheckPlayerWin(string playerUid)
        {
            var opposedPlayer = GetOpposedPlayer(playerUid);
            var baseCaptured = CheckOpposedBaseCaptured(playerUid);
            if (baseCaptured)
                Debug.Log($"База грвця {opposedPlayer.Name} захвачена");
            
            return baseCaptured;
        }
        
        private void StorageUpdatedHandler(IGameSessionStorage sender)
        {
            StorageUpdatedImpl();
            
            OnStorageUpdated?.Invoke();

            if (sender.Data.WinData != null)
                ProcessWin();
        }

        private PlayerGameData GetOpposedPlayer(string playerUid) => SessionStorage.Data.Players
            .First(p => p.Uid != playerUid);
        
        private bool CheckOpposedBaseCaptured(string uid)
        {
            return GameField.GetOpposedBaseCellModels()
                .Any(cell => cell.PlayerId == uid);
        }
    }
}