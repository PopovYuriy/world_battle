using System;
using System.Collections.Generic;
using Game.Data;
using Game.Services.Storage;

namespace Game.Field.Mediators
{
    public interface IGameMediator : IDisposable
    {
        event Action<char> OnLetterPicked;
        event Action OnStorageUpdated;
        event Action<string> OnWin;

        PlayerGameData CurrentPlayer { get; }
        string CurrentWord { get; }

        void Initialize(GameField gameField, IGameSessionStorage sessionStorage, GameFieldColorsConfig colorConfig, string ownerPlayerId);
        void ClearCurrentWord();
        void ApplyCurrentWord();
        IReadOnlyList<PlayerGameData> GetOrderedPlayersList();
    }
}