#if UNITY_EDITOR
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(PushBlocksSection))]
    public class PushBlocksSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!PushBlocksSection.Instance.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif