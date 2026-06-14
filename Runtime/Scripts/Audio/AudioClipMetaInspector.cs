#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TTModdingKit.Audio
{
    [CustomEditor(typeof(AudioImporter))]
    public class AudioClipMetaInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var importer = (AudioImporter)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mod Meta Data", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            GUIContent label = new("File Path", "The path of the audio file in the mod, minus Audio. Example: _SoundFX\\PICKUP\\MK-Appear");
            string newValue = EditorGUILayout.TextField(label, importer.userData);
            if (EditorGUI.EndChangeCheck())
            {
                importer.userData = newValue;
                AssetDatabase.WriteImportSettingsIfDirty(importer.assetPath);
            }
        }
    }
}
#endif