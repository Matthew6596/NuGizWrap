#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Lever))]
    public class LeverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!LeverSection.Instance.CreateVersionEditorGUI(s => s.version, "Lever", out int version)) return;

            serializedObject.Prop("handleColor");
            if (version >= 2) serializedObject.Prop("multiplePulls");
            if (version >= 3) serializedObject.Prop("pullTime");
            if (version >= 4) serializedObject.Prop("invisible");
            if (version >= 5) serializedObject.Prop("target");
            if (version >= 6) serializedObject.Prop("targetInvisible");
            if (version >= 7) serializedObject.Prop("unknown1");
            if (version >= 8) serializedObject.Prop("unknown2");
            if (version >= 9) serializedObject.Props("unknown3", "unknown4");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif