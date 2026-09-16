#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(BlowupConfig))]
    public class BlowupSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = target as BlowupConfig;
            if (!config.CheckSectionCompatibilityAndVersion(serializedObject)) return;
            int version = config.version;

            if (version >= 39) serializedObject.Prop("unknown");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif