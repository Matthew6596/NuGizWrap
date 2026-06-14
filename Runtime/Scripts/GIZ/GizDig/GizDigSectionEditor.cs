#if UNITY_EDITOR
using TTModdingKit.Helper;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    [CustomEditor(typeof(GizDigSection))]
    public class GizDigSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizDigSection.Instance.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif