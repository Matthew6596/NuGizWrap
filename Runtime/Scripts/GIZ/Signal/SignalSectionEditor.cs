#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(SignalConfig))]
    public class SignalSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var config = target as SignalConfig;
            if (!config.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif