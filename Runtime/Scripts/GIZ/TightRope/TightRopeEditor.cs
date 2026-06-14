#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(TightRope))]
    public class TightRopeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!TightRopeSection.Instance.CreateVersionEditorGUI(s => s.version, "TightRope", out int version)) return;

            serializedObject.Props("unknown1", "unknown2");
            if (version >= 4) serializedObject.Props("unknown3", "unknown4");
            if (version >= 2) serializedObject.Props("unknown5", "unknown6", "unknown7", "unknown8", "unknown9", "unknown10");
            if (version >= 3) serializedObject.Prop("unknown11");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif