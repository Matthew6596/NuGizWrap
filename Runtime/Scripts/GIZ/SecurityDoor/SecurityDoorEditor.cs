#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(SecurityDoor))]
    public class SecurityDoorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!SecurityDoorSection.Instance.CreateVersionEditorGUI(s => s.version, "SecurityDoor", out int version)) return;

            if (version >= 2) serializedObject.Prop("type");
            if (version >= 3) serializedObject.Prop("unknown1");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif