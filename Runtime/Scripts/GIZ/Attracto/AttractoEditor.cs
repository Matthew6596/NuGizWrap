#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
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
            if (version == 2) serializedObject.Prop("unknown1");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif