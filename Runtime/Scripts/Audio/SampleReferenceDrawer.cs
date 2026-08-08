#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace NuGizWrap.Audio
{
    [CustomPropertyDrawer(typeof(SampleReference))]
    [InitializeOnLoad]
    public class SampleReferenceDrawer : PropertyDrawer
    {
        static string[] sampleNames = null;

        static SampleReferenceDrawer()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode) => sampleNames = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            sampleNames ??= AudioExporter.GetAllSampleNames();

            EditorGUI.BeginProperty(position, label, property);

            var refProp = property.FindPropertyRelative("referenceInProject");
            var sampleProp = property.FindPropertyRelative("sample");

            // Draw label, returns remaining rect
            position = EditorGUI.PrefixLabel(position, label);

            // Small toggle for referenceInProject
            float toggleW = 16f;
            Rect toggleRect = new(position.x, position.y, toggleW, position.height);
            Rect fieldRect = new(position.x + toggleW + 2f, position.y, position.width - toggleW - 2f, position.height);

            refProp.boolValue = EditorGUI.Toggle(toggleRect, new GUIContent("", "Check this to reference a sample in the project. Uncheck to enter sample name manually."), refProp.boolValue);

            if (refProp.boolValue)
            {
                if (sampleNames.Length == 0)
                {
                    EditorGUI.HelpBox(fieldRect, "No samples in PROJECT scene.", MessageType.Warning);
                }
                else
                {
                    int ind = Array.IndexOf(sampleNames, sampleProp.stringValue);
                    if (ind == -1) ind = 0;
                    ind = EditorGUI.Popup(fieldRect, ind, sampleNames);
                    sampleProp.stringValue = sampleNames[ind];
                }
            }
            else
            {
                sampleProp.stringValue = EditorGUI.TextField(fieldRect, sampleProp.stringValue);
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif