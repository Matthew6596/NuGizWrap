#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizTurret))]
    public class GizTurretEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizTurretSection.Instance.CreateVersionEditorGUI(s => s.version, "GizTurret", out int version)) return;

            EditorGUILayout.IntSlider(serializedObject.FindProperty("specialObjectVersion"), 1, 3);
            serializedObject.Prop("specialObjects");

            serializedObject.Props("unknown1", "unknown2", "unknown3", "unknown4", "unknown5", "unknown6", "unknown7", "unknown8", "unknown9", "unknown10");

            if (version >= 2) serializedObject.Prop("unknown11");

            serializedObject.Props("unknown12", "unknown13", "unknown14", "unknown15", "unknown16", "unknown17", "unknown18", "minStuds", "maxStuds", "studsSpawn");

            if (version >= 6) serializedObject.Prop("studsSpawnSpeed");
            serializedObject.Prop("unknown19");
            if (version >= 4) serializedObject.Props("unknown20", "unknown21");

            serializedObject.Props("blasterMaterial", "part1", "part2");
            if (version >= 7) serializedObject.Prop("part3");
            serializedObject.Props("blowup", "unknown22");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif