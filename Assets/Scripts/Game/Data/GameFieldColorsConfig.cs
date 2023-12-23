using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "GameFieldColorsConfig", menuName = "GameFieldColorsConfig", order = 0)]
    public sealed class GameFieldColorsConfig : ScriptableObject
    {
        [field: SerializeField] public Color OwnerColor { get; private set; }
        [field: SerializeField] public Color OpponentColor { get; private set; }
        [field: SerializeField] public Color DefaultColor { get; private set; }
    }
}