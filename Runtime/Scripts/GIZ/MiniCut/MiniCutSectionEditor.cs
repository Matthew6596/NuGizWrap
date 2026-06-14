#if UNITY_EDITOR
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(MiniCutSection))]
    public class MiniCutSectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!MiniCutSection.Instance.CheckSectionCompatibilityAndVersion(serializedObject, editable:false)) return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif