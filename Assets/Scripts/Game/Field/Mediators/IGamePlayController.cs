using System;
using System.Collections.Generic;
using App.Modules.GameSessions.Controller;
using App.Modules.GameSessions.Data;
using Game.Data;

namespace Game.Field.Mediators
{
    public interface IGamePlayController : IDisposable
    {
        event Action OnWordChanged;
        event Action OnStorageUpdated;
        event Action<WinData> OnWin;

        PlayerGameData CurrentPlayer { get; }
        string CurrentWord { get; }

        void Initialize(GameField gameField, IGameSessionController sessionController, GameFieldColorsConfig colorConfig, string ownerPlayerId);
        void Activate();
        void Deactivate();
        void ClearCurrentWord();
        void ApplyCurrentWord();
        void DeleteGame();
        void ProcessWin();
        IReadOnlyList<PlayerGameData> GetOrderedPlayersList();
    }
}