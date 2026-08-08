#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(BlowupSection))]
    public class BlowupSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!BlowupSection.Instance.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            if (BlowupSection.Instance.version >= 39) serializedObject.Prop("unknown");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif