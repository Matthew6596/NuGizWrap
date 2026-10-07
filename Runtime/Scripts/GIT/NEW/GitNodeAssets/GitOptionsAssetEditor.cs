#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace NuGizWrap.GizFlow
{
    using Helper;

    [CustomEditor(typeof(GitOptionsAsset))]
    public class GitOptionsAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var options = target as GitOptionsAsset;
            var node = options.node;

            EditorExt.Header("Git Window Customization:");

            if (node != null)
            {
                node.backgroundColor = EditorGUILayout.ColorField("Background Color", node.backgroundColor);
                node.defaultFlowboxColor = EditorGUILayout.ColorField("Default FlowBox Color", node.defaultFlowboxColor);
                node.collapseBoxColor = EditorGUILayout.ColorField("Default Collapse Box Color", node.collapseBoxColor);
                node.textColor = EditorGUILayout.ColorField("Default Text Color", node.textColor);
                node.defaultEdgeColor = EditorGUILayout.ColorField("Default Edge Color", node.defaultEdgeColor);
            }

            options.node = node;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif