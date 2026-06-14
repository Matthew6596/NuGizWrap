#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Whipper))]
    public class WhipperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!WhipperSection.Instance.CreateVersionEditorGUI(s => s.version, "Whipper", out int version)) return;

            serializedObject.Props("unknown1", "unknown2");
            if (version >= 2) serializedObject.Prop("unknown3");
            if (version >= 3) serializedObject.Prop("unknown4");
            if (version >= 4) serializedObject.Prop("gizObstacle");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif