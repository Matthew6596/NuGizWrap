#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;

namespace NuGizWrap
{
    using Helper;
    [CustomEditor(typeof(TTUnityProject))]
    public class TTUnityProjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorExt.Header("Mod Data");
            serializedObject.Prop("modMeta");
            serializedObject.Prop("globalThings");

            EditorExt.Header("Preferences");
            serializedObject.Prop("prefs");

            EditorExt.Header("External");
            var managerLinkedProp = serializedObject.FindProperty("modManagerLinked");
            var vanillaPathProp = serializedObject.FindProperty("vanillaGamePath");
            var managerSettingsProp = serializedObject.FindProperty("modManagerSettings");

            if (managerLinkedProp.boolValue)
            {
                EditorGUILayout.PropertyField(managerSettingsProp);

                var modManagerSettings = (TTUnityProject.ModManagerSettings)managerSettingsProp.boxedValue;

                if (modManagerSettings.gamePaths == null) modManagerSettings.RefreshGamePaths();
                if (modManagerSettings.gamePaths != null)
                {
                    foreach (var pair in modManagerSettings.gamePaths)
                    {
                        EditorGUILayout.LabelField($"{pair.Key} Game Path: {pair.Value}");
                    }
                }

                if (!File.Exists(modManagerSettings.exe) || !Directory.Exists(modManagerSettings.dataPath))
                {
                    managerLinkedProp.boolValue = false;
                    return;
                }

                vanillaPathProp.stringValue = modManagerSettings.GetGamePath(TTGame.TCS);

                if (GUILayout.Button("Open Mod Manager"))
                {
                    Process.Start(modManagerSettings.exe);
                }
            }
            else
            {
                if (GUILayout.Button("Link Mod Manager"))
                {
                    var modManagerSettings = (TTUnityProject.ModManagerSettings)managerSettingsProp.boxedValue;

                    if (!File.Exists(modManagerSettings.exe))
                    {
                        string modManagerExe = EditorUtility.OpenFilePanel("Select Mod Manager .exe file", "", "exe");
                        if (File.Exists(modManagerExe)) modManagerSettings.exe = modManagerExe;
                    }

                    if (!Directory.Exists(modManagerSettings.dataPath))
                    {
                        string modManagerData = EditorUtility.OpenFolderPanel("Select Mod Manager Data Path", "", "");
                        if (Directory.Exists(modManagerData)) modManagerSettings.dataPath = modManagerData;
                    }

                    managerLinkedProp.boolValue = File.Exists(modManagerSettings.exe) && Directory.Exists(modManagerSettings.dataPath);
                    managerSettingsProp.boxedValue = modManagerSettings;
                }

                EditorGUILayout.PropertyField(vanillaPathProp);
            }

            EditorExt.Header("Export Options");
            serializedObject.Prop("removalPaths");

            EditorExt.Header("Project Paths");
            serializedObject.Props("audioPath", "scenesPath", "materialsPath", "textAssetsPath");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif