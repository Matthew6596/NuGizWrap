#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Teleport))]
    public class TeleportEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!TeleportSection.Instance.CreateVersionEditorGUI(s => s.version, "Teleport", out int version)) return;

            serializedObject.Props("unknown1", "unknown2", "unknown3", "unknown4", "unknown5", "unknown6", "unknown7", "unknown8", "unknown9", "unknown10", "unknown11", "unknown12", "unknown13", "unknown14", "unknown15", "unknown16");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif