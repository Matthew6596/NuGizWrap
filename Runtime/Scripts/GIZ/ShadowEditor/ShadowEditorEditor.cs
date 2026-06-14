#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(ShadowEditor))]
    public class ShadowEditorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!ShadowEditorSection.Instance.CreateVersionEditorGUI(s => s.version, "ShadowEditor", out int version)) return;

            serializedObject.Props("unknown1");
            if (version >= 2) serializedObject.Props("unknown2", "unknown3");
            if (version >= 3) serializedObject.Props("unknown4", "unknown5");
            if (version >= 4) serializedObject.Prop("unknown6");
            if (version >= 5) serializedObject.Props("unknown7", "unknown8");
            if (version >= 6) serializedObject.Props("unknown9", "unknown10", "unknown11");
            if (version >= 7) serializedObject.Prop("unknown12");
            if (version >= 8) serializedObject.Prop("unknown13");
            if (version > 9) serializedObject.Prop("unknown14"); // >9 instead of >=9 since ==9 gets overridden anyways

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif