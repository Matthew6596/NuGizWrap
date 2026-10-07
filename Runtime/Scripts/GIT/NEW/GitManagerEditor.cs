#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace NuGizWrap.GizFlow
{
    using Helper;

    [CustomEditor(typeof(GitManager))]
    public class GitManagerEditor : Editor
    {
        bool debugFold = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if(GUILayout.Button("Edit Gizmo Flow in GitWindow")) GitWindow.ShowWindow((GitManager)target);

            if (debugFold = EditorGUILayout.BeginFoldoutHeaderGroup(debugFold, new GUIContent("Debug Buttons")))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Count boxes")) Debug.Log($"Counted {serializedObject.FindProperty("boxes").arraySize} boxes");
                if (GUILayout.Button("Count conns")) Debug.Log($"Counted {serializedObject.FindProperty("connections").arraySize} connections");
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.Props("options", "visualizeInScene", "variables");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif