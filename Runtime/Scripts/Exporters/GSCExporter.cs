#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace NuGizWrap
{
    public static class GSCExporter
    {
        [MenuItem("Nu Giz Wrap/Export/File/GSC")]
        static void Export() 
        {
            string path = EditorUtility.SaveFilePanel("Export GSC File", TTUnityProject.GetDefaultFileExplorerPath(), "levelgsc", "gsc");
            if (string.IsNullOrEmpty(path) || !Directory.Exists(Path.GetDirectoryName(path))) return;

            Export(path);
        }

        public static void Export(string path)
        {
            EditorUtility.DisplayProgressBar("Exporting", $"Exporting Game Scene as {Path.GetFileName(path)}...", 0);
        }

        public static byte[] GetBytes()
        {
            return new byte[0];
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif