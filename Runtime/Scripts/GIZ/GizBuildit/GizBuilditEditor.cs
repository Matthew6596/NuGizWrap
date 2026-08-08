#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizBuildit))]
    public class GizBuilditEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizBuilditSection.Instance.CreateVersionEditorGUI(s => s.version, "GizBuildit", out int version)) return;

            EditorGUILayout.IntSlider(serializedObject.FindProperty("specialObjectVersion"), 1, 3);
            serializedObject.Prop("specialObjects");

            serializedObject.Props("jumpIntensity");
            if (version <= 6) serializedObject.Prop("unknown1");
            serializedObject.Props("minStuds", "maxStuds", "unknown2", "unknown3");

            if (version >= 10) serializedObject.Prop("unknown10");
            if (version >= 6) serializedObject.Prop("unknown4");
            if (version == 7) EditorGUILayout.HelpBox("Connecting blowup via nametable ID is not supported.", MessageType.None);
            if (version >= 8) serializedObject.Prop("blowup");
            if (version >= 7) serializedObject.Prop("studsSpawn");
            if (version >= 9) serializedObject.Prop("studsSpawnSpeed");
            if (version >= 4) serializedObject.Prop("unknown7");
            if (version >= 5) serializedObject.Props("unknown8", "unknown9");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif