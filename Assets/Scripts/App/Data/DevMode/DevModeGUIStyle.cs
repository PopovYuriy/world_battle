using UnityEngine;

namespace App.Data.DevMode
{
    [CreateAssetMenu(fileName = "DevModeGUIStyle", menuName = "Devmode/GUIStyle", order = 0)]
    public sealed class DevModeGUIStyle : ScriptableObject
    {
        [field: SerializeField] public GUIStyle DefaultStyle { get; private set; }
        [field: SerializeField] public Vector2 ButtonDefaultSize { get; private set; }
        [field: SerializeField] public RectOffset DefaultGroupMargin { get; private set; }
        [field: SerializeField] public Vector2 GroupButtonsDefaultSize { get; private set; }
        [field: SerializeField] public GUIStyle GroupButtonsDefaultStyle { get; private set; }
        [field: SerializeField] public int DefaultGroupButtonsSpacing { get; private set; }
    }
}