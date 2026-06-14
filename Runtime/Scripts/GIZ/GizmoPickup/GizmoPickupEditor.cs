#if UNITY_EDITOR
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizmoPickup))]
    public class GizmoPickupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizmoPickupSection.Instance.CreateVersionEditorGUI(s => s.version, "GizmoPickup", out int version)) return;

            serializedObject.Prop("type");
            if (version >= 2) serializedObject.Prop("spawnType");
            if (version >= 4) serializedObject.Prop("spawnGroup");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif