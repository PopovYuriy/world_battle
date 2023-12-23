using System;
using Game.Data;
using Game.Services.Storage;

namespace Game.Field.Mediators
{
    public interface IGameMediator : IDisposable
    {
        void Initialize(GameField gameField, IGameSessionStorage sessionStorage, GameFieldColorsConfig colorConfig, string ownerPlayerId);
    }
}