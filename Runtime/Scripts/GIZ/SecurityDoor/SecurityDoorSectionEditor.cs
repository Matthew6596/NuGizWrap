#if UNITY_EDITOR
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(SecurityDoorSection))]
    public class SecurityDoorSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!SecurityDoorSection.Instance.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif