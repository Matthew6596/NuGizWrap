#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(MiniCutConfig))]
    public class MiniCutSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = target as MiniCutConfig;
            if (!config.CheckSectionCompatibilityAndVersion(serializedObject, editable:false)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif