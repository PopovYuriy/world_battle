using System.Collections.Generic;

namespace Game.Data
{
    public interface IGameTurnsProvider
    {
        IReadOnlyList<string> TurnsList { get; }
    }
}