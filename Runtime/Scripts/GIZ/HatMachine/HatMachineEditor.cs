#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(HatMachine))]
    public class HatMachineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!HatMachineSection.Instance.CreateVersionEditorGUI(s => s.version, "HatMachine", out int version)) return;

            serializedObject.Prop("type");
            if (version >= 3) serializedObject.Prop("handleColor");
            if (version >= 4) serializedObject.Prop("target");
            if (version >= 5) serializedObject.Prop("targetInvisible");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif