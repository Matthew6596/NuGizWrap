#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Grapple))]
    public class GrappleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GrappleSection.Instance.CreateVersionEditorGUI(s => s.version, "Grapple", out int version)) return;

            EditorGUILayout.HelpBox("Y rotation on this transform will affect the Grapple base's Y rotation.", MessageType.None);
            if (version >= 5) EditorGUILayout.HelpBox("X rotation on this transform will affect the Grapple base's X rotation.", MessageType.None);

            var ropeProp = serializedObject.FindProperty("swingingRope");
            if (version >= 3) serializedObject.Prop("unknown3");
            if (version >= 4)
            {
                EditorGUILayout.PropertyField(ropeProp);
                if (ropeProp.boolValue) serializedObject.Prop("length");
            }
            if (version >= 6) serializedObject.Prop("noFreeMovement");
            if (version >= 7) serializedObject.Prop("specialObject");
            if (version >= 8) serializedObject.Prop("visible");
            if (version >= 9)
            {
                if (ropeProp.boolValue) serializedObject.Prop("ropeType");
                else serializedObject.Prop("grappleType");
            }
            if (version >= 10) serializedObject.Prop("blowup");
            if (version >= 11 && ropeProp.boolValue) 
            {
                var shadeProp = serializedObject.FindProperty("ropeBrightness");
                shadeProp.floatValue = EditorGUILayout.Slider("Rope Brightness",shadeProp.floatValue,0,1);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif