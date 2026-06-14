#if UNITY_EDITOR
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(SignalSection))]
    public class SignalSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!SignalSection.Instance.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif