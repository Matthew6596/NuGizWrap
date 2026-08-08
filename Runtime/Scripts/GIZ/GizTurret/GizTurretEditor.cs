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

            serializedObject.Props("unknown2", "unknown3", "unknown4", "unknown5", "unknown6", "unknown7", "unknown8", "unknown9", "unknown10");

            if (version >= 2) serializedObject.Prop("unknown11");

            serializedObject.Props("unknown12", "unknown13", "shootRange", "unknown15", "fireRate", "xRotationSpeed", "yRotationSpeed", "studsValue", "studsSpawn");

            if (version >= 6) serializedObject.Prop("studsSpawnSpeed");
            serializedObject.Prop("unknown19");
            if (version >= 4) serializedObject.Props("unknown20", "unknown21");

            serializedObject.Props("boltType", "unknownSfx1", "unknownSfx2");
            if (version >= 7) serializedObject.Prop("unknownSfx3");
            serializedObject.Props("blowup", "unknown22");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif