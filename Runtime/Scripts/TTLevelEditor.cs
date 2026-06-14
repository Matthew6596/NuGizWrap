#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using System;

namespace TTModdingKit
{
    using Gizmos;

    public static class TTLevelEditor
    {
        public static GameObject[] RootObjects { get; private set; }

        private static bool _errored;
        public static bool Errored { get{
                if (_errored) //reading Errored automatically resets it for convenience
                {
                    _errored = false;
                    return true;
                }
                return false;
            } private set { _errored = value; } 
        }

        static void ExportEntireMod()
        {

        }

        public static void ExportMod()
        {

        }


        [MenuItem("TT Modding/Export/Level", priority = 22)]
        static void ExportModLevel() 
        {
            string path = EditorUtility.SaveFolderPanel("Export Level", "", "");
            if (!Directory.Exists(path)) return;

            double exportTime = ExportLevel(path);
            if (exportTime == -1) return;

            EditorUtility.DisplayDialog("Level Exported!", $"Level '{Path.GetFileName(path)}' successfully exported to '{path}' in {exportTime} seconds", "OK");
        }

        [MenuItem("TT Modding/Import/Level", priority = 23)]
        static void ImportModLevel()
        {

        }

        public static double ExportLevel(string path)
        {
            string filename = Path.GetFileName(path);
            string filepath = Path.Combine(path, filename);
            double startTime = EditorApplication.timeSinceStartup;

            Errored = false;
            GSCExporter.Export($"{filepath}.gsc");
            if (Errored) return -1;
            TERExporter.Export($"{filepath}.ter");
            if (Errored) return -1;
            GIZExporter.Export($"{filepath}.giz");
            if (Errored) return -1;
            EditorUtility.ClearProgressBar();

            return EditorApplication.timeSinceStartup - startTime;
        }

        [MenuItem("TT Modding/New/Level",priority = 20)]
        public static void NewLevel()
        {
            string scenesPath = TTUnityProject.ProjectAssetPath + "/" + TTUnityProject.Instance.scenesPath+"/NewLevel";
            string templatePath = TTUnityProject.ProjectAssetPath + "/TTUnityKit/template/Level.scenetemplate";
            SceneTemplateAsset template = AssetDatabase.LoadAssetAtPath<SceneTemplateAsset>(templatePath);
            var res = SceneTemplateService.Instantiate(template, false, scenesPath+".unity");
            var ttlevel = ScriptableObject.CreateInstance<TTLevel>();
            ttlevel.name = "NewLevel";
            ttlevel.scene = res.sceneAsset;
            AssetDatabase.CreateAsset(ttlevel, scenesPath+".asset");
            AssetDatabase.Refresh();
        }

        public static void Error(string message)
        {
            EditorUtility.ClearProgressBar();
            Errored = true;
            EditorUtility.DisplayDialog("TT Level Editor Error", message, "OK");
        }
    }
}
#endif