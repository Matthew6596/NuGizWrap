#if UNITY_EDITOR
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizFlockSection))]
    public class GizFlockSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizFlockSection.Instance.CheckSectionCompatibilityAndVersion(serializedObject, editable: false)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif