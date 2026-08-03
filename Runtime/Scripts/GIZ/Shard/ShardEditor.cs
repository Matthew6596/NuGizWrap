#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Shard))]
    public class ShardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!ShardSection.Instance.CreateVersionEditorGUI(s => s.version, "Shard", out int version)) return;

            EditorGUILayout.HelpBox("The shard's Y rotation will be randomized as it is loaded in game.", MessageType.None);
            if (version >= 2)
            {
                EditorGUILayout.HelpBox("X and Z rotation on this transform will affect the shard's X and Z rotation.", MessageType.None);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif