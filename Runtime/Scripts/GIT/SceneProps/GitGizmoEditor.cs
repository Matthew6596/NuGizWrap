#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace NuGizWrap.GizFlow
{
    using Helper;
    using Gizmos;

    [CustomEditor(typeof(GitGizmo))]
    public class GitGizmoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GitGizmo gitGizmo = serializedObject.targetObject as GitGizmo;

            //Ensure that a Gizmo is connected
            var gizProp = serializedObject.FindProperty("connectedGizmo");
            Gizmo giz = gizProp.boxedValue as Gizmo;
            if (giz == null)
            {
                if (gitGizmo.TryGetComponent(out giz)) gizProp.boxedValue = giz;
                else
                {
                    EditorGUILayout.HelpBox("GitGizmo needs to be on the same gameObject as the related Gizmo.", MessageType.Error);
                    return;
                }
            }

            EditorGUILayout.LabelField("Connected Gizmo: " + giz.GetType().Name);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif