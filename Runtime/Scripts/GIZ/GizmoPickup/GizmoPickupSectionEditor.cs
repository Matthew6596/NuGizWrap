#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizmoPickupSection))]
    public class GizmoPickupSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizmoPickupSection.Instance.CheckSectionCompatibilityAndVersion(serializedObject)) return;
            int version = GizmoPickupSection.Instance.version;

            if (version >= 3) serializedObject.Prop("unknown1");
            if (version >= 5)
            {
                serializedObject.Prop("drawDistance");
                serializedObject.Prop("scale");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif