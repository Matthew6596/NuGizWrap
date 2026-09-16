#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizFlockConfig))]
    public class GizFlockSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = target as GizFlockConfig;
            if (!config.CheckSectionCompatibilityAndVersion(serializedObject, editable: false)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif