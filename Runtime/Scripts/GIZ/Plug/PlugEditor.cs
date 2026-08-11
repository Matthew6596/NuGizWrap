#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Plug))]
    public class PlugEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!PlugSection.Instance.CreateVersionEditorGUI(s => s.version, "Plug", out int version)) return;

            var visProp = serializedObject.FindProperty("blowupObjectVisible");
            if (version < 4 || visProp.boolValue)
            {
                if(version >= 2) EditorGUILayout.HelpBox("X, Y, and Z rotation on this transform will affect the blowup object's X, Y, and Z rotation.", MessageType.None);
                else EditorGUILayout.HelpBox("X and Y rotation on this transform will affect the blowup object's X and Y rotation.", MessageType.None);
            }

            serializedObject.Prop("validBlowups");
            if (version >= 5) serializedObject.Prop("unknown2");
            if (version >= 4) EditorGUILayout.PropertyField(visProp);
            if (version >= 6) serializedObject.Prop("unknown5");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif