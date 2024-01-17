using UnityEditor;
using UnityEditor.UI;

namespace Core.UI.Components.Editor
{
    [CustomEditor(typeof(TextButton))]
    public class TextButtonEditor : ButtonEditor
    {
        SerializedProperty _labelProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _labelProperty = serializedObject.FindProperty("_label");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_labelProperty);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}