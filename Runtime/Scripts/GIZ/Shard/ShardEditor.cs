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

            if (version >= 2) serializedObject.Props("unknown1", "unknown2");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif