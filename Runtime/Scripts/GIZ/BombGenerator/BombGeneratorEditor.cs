#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(BombGenerator))]
    public class BombGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!BombGeneratorSection.Instance.CreateVersionEditorGUI(s => s.version, "BombGenerator", out int version)) return;

            serializedObject.Prop("unknown1");
            if (version >= 2) serializedObject.Prop("unknown2");

            EditorGUILayout.IntSlider(serializedObject.FindProperty("specialObjectVersion"), 1, 3);
            serializedObject.Prop("specialObjects");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif