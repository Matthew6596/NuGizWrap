#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Spinner))]
    public class SpinnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!SpinnerSection.Instance.CreateVersionEditorGUI(s => s.version, "Spinner", out int version)) return;

            serializedObject.Prop("specialObject");
            if (version >= 2) 
            {
                var flapProp = serializedObject.FindProperty("flapCount");
                EditorGUILayout.PropertyField(flapProp);

                //if (flapProp.intValue > 0)
                {
                    if (version >= 3)
                    {
                        var unk1Prop = serializedObject.FindProperty("interactionOptions");
                        EditorGUILayout.PropertyField(unk1Prop);

                        serializedObject.Prop("outputStickTime");
                        if (version >= 4) serializedObject.Prop("animSpeed");
                        if (unk1Prop.intValue != 0 && version >= 6) serializedObject.Prop("unknown4");
                    }
                }
            }

            if (version >= 6)
            {
                EditorGUILayout.IntSlider(serializedObject.FindProperty("specialObjectVersion"), 1, 3);
                serializedObject.Prop("animObjects");
            }

            if (version >= 7) serializedObject.Prop("outputStates");
            if (version >= 8) serializedObject.Prop("unknown6");
            if (version >= 9) serializedObject.Prop("unknown7");

            if (version >= 10) serializedObject.Prop("unknownSpecialObject");
            if (version >= 12) serializedObject.Prop("unknown9");
            if (version >= 13) serializedObject.Prop("unknown10");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif