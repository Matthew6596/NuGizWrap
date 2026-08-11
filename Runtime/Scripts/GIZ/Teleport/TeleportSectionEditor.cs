#if UNITY_EDITOR
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(TeleportSection))]
    public class TeleportSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!TeleportSection.Instance.CheckSectionCompatibilityAndVersion(serializedObject, editable:false)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif