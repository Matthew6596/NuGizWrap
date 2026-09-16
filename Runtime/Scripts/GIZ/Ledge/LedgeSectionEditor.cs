#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(LedgeConfig))]
    public class LedgeSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = target as LedgeConfig;
            if (!config.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif