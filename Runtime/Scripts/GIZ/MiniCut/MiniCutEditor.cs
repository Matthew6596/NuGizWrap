#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(MiniCut))]
    public class MiniCutEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!MiniCutSection.Instance.CreateVersionEditorGUI(s => s.version, "MiniCut", out int version)) return;

            serializedObject.Props("unknown1", "duration", "blendInTime", "blendOutTime", "maxTotalDuration", "miniCutParts");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif