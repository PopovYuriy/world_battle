using System;

namespace Game.Grid.Cells.Model
{
    [Serializable]
    public sealed class CellModel
    {
        public int Id { get; private set; }
        public char Letter { get; private set; }
        public int Points { get; private set; }
        public string PlayerId { get; private set; }

        public event Action OnChanged;

        public CellModel(int id, char letter, int points, string playerId)
        {
            Id = id;
            Letter = letter;
            Points = points;
            PlayerId = playerId;
        }

        public void SetPoints(int points)
        {
            OnChanged?.Invoke();
            Points = points;
        }
        
        public void SetPlayerId(string playerId) => PlayerId = playerId;
    }
}