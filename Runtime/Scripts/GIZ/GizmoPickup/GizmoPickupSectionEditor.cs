#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizmoPickupConfig))]
    public class GizmoPickupSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = this.target as GizmoPickupConfig;

            if (!config.CheckSectionCompatibilityAndVersion(serializedObject)) return;
            int version = config.version;

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