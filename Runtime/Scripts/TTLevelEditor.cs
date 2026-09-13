#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using System;

namespace NuGizWrap
{
    using Gizmos;
    using Terrain;
    using GizFlow;
    using AI;
    using Lighting;
    using Animations;
    using GameScene;

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


        [MenuItem("Nu Giz Wrap/Export/Level", priority = 22)]
        static void ExportModLevel() 
        {
            string path = EditorUtility.SaveFolderPanel("Export Level", TTUnityProject.GetDefaultFileExplorerPath(), "");
            if (!Directory.Exists(path)) return;

            double exportTime = ExportLevel(path);
            if (exportTime == -1) return;

            EditorUtility.DisplayDialog("Level Exported!", $"Level '{Path.GetFileName(path)}' successfully exported to '{path}' in {exportTime} seconds", "OK");
        }

        [MenuItem("Nu Giz Wrap/Import/Level", priority = 23)]
        static void ImportModLevel()
        {
            string dir = EditorUtility.OpenFolderPanel("Import Level", TTUnityProject.GetDefaultFileExplorerPath(), "");
            if (!Directory.Exists(dir)) return;

            double importTime = ImportLevel(dir);
            if (importTime == -1) return;

            EditorUtility.DisplayDialog("Level Imported!", $"Level '{Path.GetFileName(dir)}' successfully imported in {importTime} seconds", "OK");
        }

        public static double ImportLevel(string directory)
        {
            string levelName = Path.GetFileName(directory);
            double startTime = EditorApplication.timeSinceStartup;

            Errored = false;

            if (GetFilePath(directory, $"{levelName}_pc.gsc", out var gscPath)) GSCImporter.Import(gscPath, notify: false);
            if (Errored) return -1;

            if (GetFilePath(directory, $"{levelName}.ter", out var terPath)) TERImporter.Import(terPath, notify: false);
            if (Errored) return -1;

            if (GetFilePath(directory, $"{levelName}.giz", out var gizPath)) GIZImporter.Import(gizPath, notify: false);
            if (Errored) return -1;

            if (GetFilePath(directory, $"AI/{levelName}.ai2", out var ai2Path)) AI2Importer.Import(ai2Path, notify: false);
            if (Errored) return -1;

            //GITImporter.Import(Path.Combine(directory, $"{levelName}.git"), notify: false); //CURRENTLY ERRORS WHEN GIT WINDOW NOT OPEN
            //if (Errored) return -1;

            if (GetFilePath(directory, $"{levelName}.rtl", out var rtlPath)) RTLImporter.Import(rtlPath, notify: false);
            if (Errored) return -1;

            if (GetFilePath(directory, $"{levelName}.bur", out var burPath)) BURImporter.Import(burPath, notify: false);
            if (Errored) return -1;

            if (GetFilePath(directory, $"{levelName}.anm", out var anmPath)) ANMImporter.Import(anmPath, notify: false);
            if (Errored) return -1;

            EditorUtility.ClearProgressBar();

            return EditorApplication.timeSinceStartup - startTime;
        }

        private static bool GetFilePath(string directory, string levelFileName, out string path)
        {
            path = Path.Combine(directory, levelFileName);
            return File.Exists(path);
        }

        public static double ExportLevel(string path)
        {
            string filename = Path.GetFileName(path);
            string filepath = Path.Combine(path, filename);
            double startTime = EditorApplication.timeSinceStartup;

            Errored = false;
            GSCExporter.Export($"{filepath}_pc.gsc");
            if (Errored) return -1;
            TERExporter.Export($"{filepath}.ter");
            if (Errored) return -1;
            GIZExporter.Export($"{filepath}.giz");
            if (Errored) return -1;
            EditorUtility.ClearProgressBar();

            return EditorApplication.timeSinceStartup - startTime;
        }

        [MenuItem("Nu Giz Wrap/New/Level",priority = 20)]
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