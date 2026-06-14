#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Techno))]
    public class TechnoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!TechnoSection.Instance.CreateVersionEditorGUI(s => s.version, "Techno", out int version)) return;

            if (version >= 8) serializedObject.Prop("controlType");
            if (version >= 2) serializedObject.Props("unknown1", "controlledEntity");
            if (version >= 3) serializedObject.Prop("unknown2");
            if (version >= 4) serializedObject.Prop("cameraEmphasisAmount");
            if (version >= 5) serializedObject.Prop("unknown3");
            if (version >= 7) serializedObject.Prop("unknown4");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif