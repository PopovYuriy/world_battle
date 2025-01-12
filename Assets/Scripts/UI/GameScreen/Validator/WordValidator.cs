using System.Linq;
using Game.Data;
using Game.Services;

namespace UI.GameScreen.Validator
{
    public sealed class WordValidator
    {
        private readonly IGameTurnsProvider _turnsProvider;
        private readonly WordsProvider _wordsProvider;

        public WordValidator(IGameTurnsProvider turnsProvider, WordsProvider wordsProvider)
        {
            _turnsProvider = turnsProvider;
            _wordsProvider = wordsProvider;
        }

        public ValidationResultType Validate(string word)
        {
            if (_turnsProvider.TurnsList != null && _turnsProvider.TurnsList.Contains(word))
                return ValidationResultType.AlreadyUsed;

            if (!_wordsProvider.IsValidWord(word))
                return ValidationResultType.NotFoundInVocabulary;

            return ValidationResultType.Valid;
        }
    }
}