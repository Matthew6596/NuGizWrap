#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(TeleportConfig))]
    public class TeleportSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = target as TeleportConfig;
            if (!config.CheckSectionCompatibilityAndVersion(serializedObject, editable:false)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif