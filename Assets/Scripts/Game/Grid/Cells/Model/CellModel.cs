using System;
using Newtonsoft.Json;

namespace Game.Grid.Cells.Model
{
    [Serializable]
    public sealed class CellModel
    {
        public int Id { get; private set; }
        public char Letter { get; private set; }
        public int Points { get; private set; }
        public string PlayerId { get; private set; }
        public bool IsLocked { get; private set; }

        public event Action OnChanged;

        public CellModel(int id, char letter, int points, string playerId)
        {
            Id = id;
            Letter = letter;
            Points = points;
            PlayerId = playerId;
        }
        
        [JsonConstructor]
        public CellModel(int id, char letter, int points, string playerId, bool isLocked)
        {
            Id = id;
            Letter = letter;
            Points = points;
            PlayerId = playerId;
            IsLocked = isLocked;
        }

        public void SetPoints(int points)
        {
            OnChanged?.Invoke();
            Points = points;
        }

        public void SetLetter(char letter)
        {
            Letter = letter;
            OnChanged?.Invoke();
        }

        public void SetIsLocked(bool isLocked)
        {
            IsLocked = isLocked;
            OnChanged?.Invoke();
        }
        
        public void SetPlayerId(string playerId) => PlayerId = playerId;
    }
}