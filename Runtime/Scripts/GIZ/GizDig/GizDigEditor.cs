#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizDig))]
    public class GizDigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizDigSection.Instance.CreateVersionEditorGUI(s => s.version, "GizDig", out int version)) return;

            if (version >= 17) serializedObject.Prop("unknown1");
            serializedObject.Props("unknown2", "interactionOptions");

            EditorGUILayout.IntSlider(serializedObject.FindProperty("specialObjectVersion"), 1, 3);
            serializedObject.Prop("specialObjects");

            serializedObject.Props("unknown3", "unknown4", "unknown5", "minStuds", "maxStuds", "studsSpawn", "studsSpawnSpeed", "unknown6", "unknown7");

            if (version >= 18) serializedObject.Prop("unknown8");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif