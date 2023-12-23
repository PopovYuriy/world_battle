using System;
using System.Collections.Generic;
using Game.Data;
using Game.Services;
using Game.Services.Storage;

namespace Game.Field.Mediators
{
    public interface IGameMediator : IDisposable
    {
        event Action<char> OnLetterPicked;
        
        PlayerGameData CurrentPlayer { get; }
        string CurrentWord { get; }

        void Initialize(GameField gameField, WordsProvider wordsProvider, IGameSessionStorage sessionStorage, GameFieldColorsConfig colorConfig, string ownerPlayerId);
        void ClearCurrentWord();
        void ApplyCurrentWord();
        IReadOnlyList<PlayerGameData> GetOrderedPlayersList();
    }
}