#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(SecurityDoorConfig))]
    public class SecurityDoorSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = target as SecurityDoorConfig;
            if (!config.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif