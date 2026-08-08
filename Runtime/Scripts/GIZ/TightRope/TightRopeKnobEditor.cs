#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(TightRope))]
    public class TightRopeKnobEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!TightRopeSection.Instance.CreateVersionEditorGUI(s => s.version, "TightRope", out int version)) return;

            EditorGUILayout.HelpBox("Position on this transform will affect the tightrope knob's position", MessageType.None);
            if (version >= 2)
            {
                EditorGUILayout.HelpBox("X and Y Rotation on this transform will affect the tightrope knob's rotation", MessageType.None);
                serializedObject.Prop("pinFacingSideways");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif