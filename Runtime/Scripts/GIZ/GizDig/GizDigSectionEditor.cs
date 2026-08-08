#if UNITY_EDITOR
using NuGizWrap.Helper;
using UnityEditor;

namespace NuGizWrap.Gizmos
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