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

            serializedObject.Props("startKnob", "endKnob");
            if (version >= 4) serializedObject.Props("unknown3", "unknown4");
            if (version >= 3) serializedObject.Prop("alwaysShowStartKnob");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif