#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace NuGizWrap.Analysis
{
    using Helper;

    [CustomEditor(typeof(AnalysisScript))]
    public class AnalysisScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Script Running: {TTAnalysis.Running}");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("alertFinish"));
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Run Script")) TTAnalysis.RunAnalysisScript();
            if (GUILayout.Button("Force End")) TTAnalysis.ForceStop();


            //horizontal line
            EditorGUILayout.BeginFadeGroup(0.1f);
            EditorGUILayout.HelpBox("",MessageType.None);
            EditorGUILayout.EndFadeGroup();

            serializedObject.Props("subScripts", "lines");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif