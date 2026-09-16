#if UNITY_EDITOR
using NuGizWrap.Helper;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    [CustomEditor(typeof(GizDigConfig))]
    public class GizDigSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = target as GizDigConfig;
            if (!config.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif