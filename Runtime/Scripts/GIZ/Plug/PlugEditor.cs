#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Plug))]
    public class PlugEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!PlugSection.Instance.CreateVersionEditorGUI(s => s.version, "Plug", out int version)) return;

            serializedObject.Prop("unknown1");
            if (version >= 5) serializedObject.Prop("unknown2");
            if (version >= 2) serializedObject.Prop("unknown3");
            if (version >= 4) serializedObject.Prop("unknown4");
            if (version >= 6) serializedObject.Prop("unknown5");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif