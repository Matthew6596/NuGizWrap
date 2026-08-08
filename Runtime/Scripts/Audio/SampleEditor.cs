#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace NuGizWrap.Audio
{
    using Helper;

    [CustomEditor(typeof(Sample))]
    public class SampleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //ignoreing disable, comment, rumble, seq, and fcat
            serializedObject.Prop("clip");
            serializedObject.Prop("pitch");
            serializedObject.Prop("priority");
            serializedObject.Prop("isGlobal");
            OptionalProp("pan", "panOpt");
            serializedObject.Prop("loop");
            OptionalProp("pitchRandomness", "pitchRandomOpt");
            OptionalProp("volume", "volumeOpt");
            OptionalProp("volumeRandomness", "volumeRandOpt");
            OptionalProp("near", "nearOpt");
            OptionalProp("far", "farOpt");

            serializedObject.ApplyModifiedProperties();
        }

        private void OptionalProp(string prop, string opt)
        {
            EditorGUILayout.BeginHorizontal();
            var optProp = serializedObject.FindProperty(opt);

            if (optProp.boolValue) serializedObject.Prop(prop);
            EditorGUILayout.PropertyField(optProp,new GUIContent(optProp.boolValue ? "":ObjectNames.NicifyVariableName(prop)));

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif