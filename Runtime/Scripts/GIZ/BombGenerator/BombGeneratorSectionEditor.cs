#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(BombGeneratorConfig))]
    public class BombGeneratorSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = target as BombGeneratorConfig;
            if (!config.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif