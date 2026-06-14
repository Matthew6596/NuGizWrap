#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Signal))]
    public class SignalEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!SignalSection.Instance.CreateVersionEditorGUI(s => s.version, "Signal", out int version)) return;

            serializedObject.Prop("character");
            if (version >= 2) serializedObject.Prop("suit");
            if (version >= 4) serializedObject.Props("unknown2", "unknown3");
            if (version >= 5) serializedObject.Prop("unknown4");
            if (version >= 7) serializedObject.Prop("unknown5");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif