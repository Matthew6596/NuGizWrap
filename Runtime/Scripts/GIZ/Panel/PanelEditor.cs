#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Panel))]
    public class PanelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!PanelSection.Instance.CreateVersionEditorGUI(s => s.version, "Panel", out int version)) return;

            if (TTUnityProject.Game == TTGame.LB1) EditorGUILayout.LabelField("Type: Joker Switch");
            else serializedObject.Prop("type");
            if (version >= 3) serializedObject.Prop("invisible");
            if (version >= 4) serializedObject.Prop("target");
            if (version >= 5) serializedObject.Prop("targetInvisible");
            if (version >= 6) serializedObject.Props("alternativeFace", "alternativeBody");
            if (version >= 7) serializedObject.Prop("unknown1");
            if (version >= 8) serializedObject.Prop("unknown2");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif