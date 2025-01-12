using System;
using System.Collections.Generic;
using System.Linq;
using App.Modules.GameSessions.Controller;
using App.Modules.GameSessions.Data;
using Game.Data;
using UnityEngine;

namespace Game.Field.Mediators
{
    public abstract class GamePlayControllerAbstract : IGamePlayController
    {
        public PlayerGameData CurrentPlayer => SessionController.Data.LastTurnPlayerId == null 
            ? SessionController.Data.Players[0]
            : SessionController.Data.Players.First(p => p.Uid != SessionController.Data.LastTurnPlayerId);

        public string CurrentWord { get; private set; }

        protected GameField GameField { get; private set; }
        protected IGameSessionController SessionController { get; private set; }
        protected GameFieldColorsConfig ColorConfig { get; private set; }
        protected string OwnerPlayerId { get; private set; }

        public event Action OnWordChanged;
        public event Action OnStorageUpdated;
        public event Action<WinData> OnWin;

        public void Initialize(GameField gameField, IGameSessionController sessionController, GameFieldColorsConfig colorConfig, 
            string ownerPlayerId)
        {
            GameField = gameField;
            SessionController = sessionController;
            ColorConfig = colorConfig;
            OwnerPlayerId = ownerPlayerId;

            ProcessPostInitializing();
            
            if (SessionController.Data.WinData != null)
                ProcessWin();
        }

        public void Dispose()
        {
            Deactivate();
        }

        public void Activate()
        {
            GameField.Activate();
            GameField.OnPickedLettersChanged += PickedLettersChangedHandler;
            SessionController.OnTurn += ControllerOnTurnHandler;
            ProcessPostActivating();
            ApplyModifications();
        }

        public void Deactivate()
        {
            GameField.Deactivate();
            GameField.OnPickedLettersChanged -= PickedLettersChangedHandler;
            SessionController.OnTurn -= ControllerOnTurnHandler;
        }

        public void ClearCurrentWord()
        {
            CurrentWord = string.Empty;
            GameField.ResetPickedCells();
        }

        public void ApplyCurrentWord()
        {
            GameField.ApplyWordForPlayer(CurrentPlayer.Uid);
            var earnedPoints = GameField.PickedCells.Sum(c => c.Model.Points);
            CurrentPlayer.Points += earnedPoints;

            var playerUid = CurrentPlayer.Uid;
            SessionController.Data.LastTurnPlayerId = playerUid;
            
            SessionController.Data.Turns ??= new List<string>();
            SessionController.Data.Turns.Add(CurrentWord);

            var isWin = CheckPlayerWin(playerUid);
            if (isWin)
                SessionController.Data.WinData = new WinData(playerUid, WinReason.BaseCaptured);
            
            SessionController.Save();
            
            ClearCurrentWord();
            
            if (isWin)
                ProcessWin();
            else
                ProcessFinishTurn();
            
            ApplyModifications();
        }

        private void ApplyModifications()
        {
            if (SessionController.Data.ModificationsData == null)
                return;

            var completedLockedCellData = new List<LockedCellData>();
            foreach (var lockedCellData in SessionController.Data.ModificationsData.LockedCells)
            {
                var cell = GameField.GetCellById(lockedCellData.CellId);
                if (SessionController.Data.Turns.Count <= lockedCellData.FinalTurnNumber) 
                    continue;
                
                cell.Model.SetIsLocked(false);
                completedLockedCellData.Add(lockedCellData);
            }

            foreach (var lockedCellData in completedLockedCellData)
                SessionController.Data.ModificationsData.LockedCells.Remove(lockedCellData);
        }

        public abstract IReadOnlyList<PlayerGameData> GetOrderedPlayersList();

        public void DeleteGame()
        {
            SessionController.Delete();
        }

        protected virtual void ProcessPostInitializing() {}
        protected virtual void ProcessPostActivating() {}
        protected virtual void ProcessWinImpl(WinData winData) { }

        protected abstract void ProcessFinishTurn();
        protected abstract void StorageUpdatedImpl();

        public void ProcessWin()
        {
            var winData = SessionController.Data.WinData;
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
        
        private void ControllerOnTurnHandler(IGameSessionController sender)
        {
            StorageUpdatedImpl();
            
            OnStorageUpdated?.Invoke();
            ApplyModifications();

            if (sender.Data.WinData != null)
                ProcessWin();
        }

        private PlayerGameData GetOpposedPlayer(string playerUid) => SessionController.Data.Players
            .First(p => p.Uid != playerUid);
        
        private bool CheckOpposedBaseCaptured(string uid)
        {
            return GameField.GetOpposedBaseCellModels()
                .Any(cell => cell.PlayerId == uid);
        }
    }
}