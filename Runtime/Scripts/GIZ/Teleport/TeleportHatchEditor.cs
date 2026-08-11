#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(TeleportHatch))]
    public class TeleportHatchEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!TeleportSection.Instance.CreateVersionEditorGUI(s => s.version, "Teleport", out int version)) return;

            EditorGUILayout.HelpBox("XYZ position and Y rotation on this transform will affect the hatch's position and rotation.", MessageType.None);
            serializedObject.Props("flapSpecialObject", "flapYOffset");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif