using System.Collections.Generic;
using Game.Field;

namespace Game.Services.Utils
{
    public sealed class AvailableLettersProvider : ILettersProvider
    {
        private readonly GameField _gameField;
        
        public AvailableLettersProvider(GameField gameField)
        {
            _gameField = gameField;
        }

        public IReadOnlyList<char> GetLettersForPlayer(string uid)
        {
            return _gameField.GetAvailableLettersForPlayer(uid);
        }
    }
}