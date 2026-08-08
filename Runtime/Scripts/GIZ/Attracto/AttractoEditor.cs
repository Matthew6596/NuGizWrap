#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Attracto))]
    public class AttractoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!AttractoSection.Instance.CreateVersionEditorGUI(s => s.version, "Attracto", out int version)) return;

            serializedObject.Prop("pieceCount");
            if (version == 2) EditorGUILayout.HelpBox("The property made available while version is 2 is unused in game, so it is not editable here.", MessageType.None);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif