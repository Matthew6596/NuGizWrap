#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Tube))]
    public class TubeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!TubeSection.Instance.CreateVersionEditorGUI(s => s.version, "Tube", out int version)) return;

            serializedObject.Props("height", "radius");
            if (version >= 2) serializedObject.Prop("magnetic");
            if (version >= 3) serializedObject.Prop("specialObject");
            if (version >= 4) serializedObject.Prop("glideOnly");
            if (version >= 5)
            {
                serializedObject.Prop("horizontal");
                serializedObject.FindProperty("canBeHorizontal").boolValue = true;
            }
            else serializedObject.FindProperty("canBeHorizontal").boolValue = false;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif