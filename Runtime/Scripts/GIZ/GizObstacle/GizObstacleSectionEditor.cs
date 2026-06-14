#if UNITY_EDITOR
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizObstacleSection))]
    public class GizObstacleSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizObstacleSection.Instance.CheckSectionCompatibilityAndVersion(serializedObject)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif