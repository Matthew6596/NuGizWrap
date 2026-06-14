#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(TorpMachine))]
    public class TorpMachineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!TorpMachineSection.Instance.CreateVersionEditorGUI(s => s.version, "Torp Machine", out int version)) return;

            if (version >= 2) serializedObject.Prop("redOutline");
            if (version >= 4) serializedObject.Prop("unknown1");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif