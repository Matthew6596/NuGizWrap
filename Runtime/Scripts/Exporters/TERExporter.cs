#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace TTModdingKit
{
    public static class TERExporter
    {
        [MenuItem("TT Modding/Export/File/TER")]
        static void Export() 
        {
            string path = EditorUtility.SaveFilePanel("Export TER File", "", "levelter", "ter");
            if (string.IsNullOrEmpty(path) || !Directory.Exists(Path.GetDirectoryName(path))) return;

            Export(path);
        }

        public static void Export(string path)
        {
            EditorUtility.DisplayProgressBar("Exporting", $"Exporting Terrain as {Path.GetFileName(path)}...", 0);
        }

        public static byte[] GetBytes()
        {
            return new byte[0];
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif