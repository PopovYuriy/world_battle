using System.Collections.Generic;

namespace Game.Services.Utils
{
    public interface ILettersProvider
    {
        IReadOnlyList<char> GetLettersForPlayer(string uid);
    }
}