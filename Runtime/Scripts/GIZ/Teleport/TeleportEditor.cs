#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Teleport))]
    public class TeleportEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizmoConfig.Instance.teleport.CreateVersionEditorGUI(s => s.version, "Teleport", out int version)) return;

            serializedObject.Props("hatchBaseSpecialObject", "hatch1", "hatch2", "unknown4", "unknown5", "unknown6", "unknown7", "unknown10", "unknown11", "unknown14");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif