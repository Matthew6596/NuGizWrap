#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(ZipUp))]
    public class ZipUpEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!ZipUpSection.Instance.CreateVersionEditorGUI(s => s.version, "ZipUp", out int version)) return;

            EditorGUILayout.HelpBox("Position on the Start, Hook, and End transforms will affect their positions.", MessageType.None);
            EditorGUILayout.HelpBox("X and Y rotation on the Hook transform will affect the hook's X and Y rotation.", MessageType.None);
            serializedObject.Props("start", "hook", "end");
            if (version < 5) serializedObject.Props("swing", "activeForPlayer"); //does nothing when version >= 5
            serializedObject.Prop("twoWay");
            if (version >= 2) serializedObject.Prop("hookVisible");
            if (version >= 3)
            {
                //shenanigans when version >= 5
                var inactiveProp = serializedObject.FindProperty("inactive");
                inactiveProp.boolValue = EditorGUILayout.Toggle(version >= 5 ? "Active" : "Inactive", inactiveProp.boolValue);
            }
            if (version == 4) serializedObject.Prop("targetsVisible"); //does nothing when version >= 5
            if (version >= 5) serializedObject.Prop("unknown5");
            if (version >= 6) serializedObject.Props("startPlatformStyle", "endPlatformStyle");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif