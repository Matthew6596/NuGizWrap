#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Ledge))]
    public class LedgeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!LedgeSection.Instance.CreateVersionEditorGUI(s => s.version, "Ledge", out int version)) return;

            serializedObject.Prop("unknown1");
            if (version >= 2) serializedObject.Props("unknown2", "unknown3");
            if (version >= 3) serializedObject.Prop("type");
            if (version >= 4)
            {
                var unk4Prop = serializedObject.FindProperty("unknown4");
                EditorGUILayout.PropertyField(unk4Prop);

                if (unk4Prop.stringValue.Length > 0) serializedObject.Props("unknown4Pos", "unknown4Ang");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif