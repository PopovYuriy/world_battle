using System.Collections.Generic;
using System.Linq;
using Game.Services;

namespace UI.GameScreen.Validator
{
    public sealed class WordValidator
    {
        private readonly IReadOnlyList<string> _turns;
        private readonly WordsProvider _wordsProvider;

        public WordValidator(IReadOnlyList<string> turns, WordsProvider wordsProvider)
        {
            _turns = turns;
            _wordsProvider = wordsProvider;
        }

        public ValidationResultType Validate(string word)
        {
            if (_turns.Contains(word))
                return ValidationResultType.AlreadyUsed;

            if (!_wordsProvider.IsValidWord(word))
                return ValidationResultType.NotFoundInVocabulary;

            return ValidationResultType.Valid;
        }
    }
}