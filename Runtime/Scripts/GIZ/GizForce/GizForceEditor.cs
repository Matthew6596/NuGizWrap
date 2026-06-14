#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizForce))]
    public class GizForceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizForceSection.Instance.CreateVersionEditorGUI(s => s.version, "GizForce", out int version)) return;

            if (version == 1) serializedObject.Prop("unknown1");
            serializedObject.Prop("returnTime");
            if (version >= 8) serializedObject.Prop("shakeTime");
            serializedObject.Prop("range");
            if (version == 1) serializedObject.Props("unknown2", "unknown3");
            serializedObject.Props("interactionOptions", "togglable");
            if (version >= 11) serializedObject.Prop("unknown4");
            serializedObject.Prop("unknown5");
            if (version == 1) serializedObject.Prop("unknown6");

            EditorGUILayout.IntSlider(serializedObject.FindProperty("specialObjectVersion"), 1, 3);
            serializedObject.Prop("specialObjects");

            serializedObject.Props("forceSpeed", "returnSpeed");
            if (version >= 6) serializedObject.Prop("autoForce");
            if (version >= 7) serializedObject.Prop("effectScale");
            if (version >= 3) serializedObject.Prop("unknown7");
            if (version == 4) serializedObject.Prop("unknown8");
            if (version >= 5) serializedObject.Prop("blowup");

            if (version >= 4) serializedObject.Props("minStuds", "maxStuds", "studsSpawn");
            if (version >= 10) serializedObject.Prop("studsSpawnSpeed");

            if (version >= 15) serializedObject.Props("processSound", "completeSound", "returnSound");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif