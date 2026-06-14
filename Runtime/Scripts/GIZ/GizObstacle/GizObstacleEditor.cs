#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizObstacle))]
    public class GizObstacleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizObstacleSection.Instance.CreateVersionEditorGUI(s => s.version, "GizObstacle", out int version)) return;

            if (version >= 2) serializedObject.Prop("triggerTransform");
            serializedObject.Props("unknown1");
            if (version >= 3) serializedObject.Props("unknown3", "unknown4");
            serializedObject.Prop("unknown5");
            if (version >= 12) serializedObject.Prop("unknown6");
            if (version == 6) serializedObject.Props("unknown7", "unknown8");
            serializedObject.Props("unknown9", "unknown10");

            if (version >= 15) serializedObject.Props("unknown17", "unknown18");
            if (version >= 17) serializedObject.Prop("unknown19");
            if (version >= 18) serializedObject.Prop("unknown20");

            if (version >= 7) serializedObject.Prop("unknown11");

            EditorGUILayout.IntSlider(serializedObject.FindProperty("specialObjectVersion"), 1, 3);
            serializedObject.Prop("specialObjects");

            if (version >= 4) serializedObject.Prop("unknown12");
            if (version >= 5) serializedObject.Prop("unknown13");
            if (version >= 8) serializedObject.Prop("unknown14");
            if (version == 9) serializedObject.Prop("unknown15");
            if (version >= 10) serializedObject.Prop("unknown16");
            if (version >= 9) serializedObject.Props("minStuds", "maxStuds", "studsSpawn");
            if (version >= 11) serializedObject.Prop("studsSpawnSpeed");
            if (version >= 13) serializedObject.Prop("unknownSfx1");
            if (version >= 14) serializedObject.Prop("unknownSfx2");
            if (version >= 16) serializedObject.Prop("unknownSfx3");

            if (version >= 19) serializedObject.Props("unknown21", "unknown22");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif