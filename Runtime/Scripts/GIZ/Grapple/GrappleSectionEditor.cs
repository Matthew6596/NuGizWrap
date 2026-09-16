#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GrappleConfig))]
    public class GrappleSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = target as GrappleConfig;
            if (!config.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif