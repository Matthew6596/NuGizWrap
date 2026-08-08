#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using UnityEngine;

    [CustomEditor(typeof(ShadowEditor))]
    public class ShadowEditorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!ShadowEditorSection.Instance.CreateVersionEditorGUI(s => s.version, "ShadowEditor", out int version)) return;

            EditorGUILayout.HelpBox("The forward vector on this transform will be used for the Shadow Direction.", MessageType.None);

            //Getting properties
            var presetProp = serializedObject.FindProperty("preset");

            var opacityProp = serializedObject.FindProperty("opacity");
            var unk2Prop = serializedObject.FindProperty("unknown2");
            var unk3Prop = serializedObject.FindProperty("unknown3");
            var unk4Prop = serializedObject.FindProperty("unknown4");
            var unk5Prop = serializedObject.FindProperty("unknown5");
            var renderDistProp = serializedObject.FindProperty("renderDistance");
            var blurProp = serializedObject.FindProperty("blur");
            var unk10Prop = serializedObject.FindProperty("unknown10");
            var unk11Prop = serializedObject.FindProperty("unknown11");
            var qualityProp = serializedObject.FindProperty("quality");
            var unk14Prop = serializedObject.FindProperty("unknown14");

            //Property fields
            if (version < 8 || presetProp.intValue == 0)
            {
                if (version >= 8) EditorGUILayout.PropertyField(presetProp);

                opacityProp.floatValue = EditorGUILayout.Slider(opacityProp.displayName, opacityProp.floatValue, 0, 1);
                if (version >= 2)
                {
                    EditorGUILayout.PropertyField(unk2Prop);
                    EditorGUILayout.PropertyField(unk3Prop);
                }
                if (version >= 3)
                {
                    EditorGUILayout.PropertyField(unk4Prop);
                    EditorGUILayout.PropertyField(unk5Prop);
                }
                if (version >= 4) EditorGUILayout.PropertyField(renderDistProp);
                if (version >= 6)
                {
                    EditorGUILayout.PropertyField(blurProp);
                    EditorGUILayout.PropertyField(unk10Prop);
                    EditorGUILayout.PropertyField(unk11Prop);
                }
                if (version >= 7)
                {
                    EditorGUILayout.PropertyField(qualityProp, new GUIContent(qualityProp.displayName, "The closer to 0, the better the quality. Numbers with a higher absolute value will result in more blocky/pixelated shadows."));
                }
                if (version > 9) EditorGUILayout.PropertyField(unk14Prop); // >9 instead of >=9 since ==9 gets overridden anyways
            }
            else //Readonly from preset
            {
                EditorGUILayout.PropertyField(presetProp);

                ReadonlyProps(opacityProp, unk2Prop, unk3Prop, unk4Prop, unk5Prop, renderDistProp, blurProp, unk10Prop, unk11Prop, qualityProp);
                if (version >= 9) ReadonlyProp(unk14Prop); // >9 instead of >=9 since ==9 gets overridden anyways
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ReadonlyProp(SerializedProperty prop)
        {
            EditorGUILayout.LabelField($"{prop.displayName,-26}\t{prop.boxedValue}");
        }

        private void ReadonlyProps(params SerializedProperty[] props)
        {
            foreach (var prop in props) ReadonlyProp(prop);
        }
    }
}
#endif