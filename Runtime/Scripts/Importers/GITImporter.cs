#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NuGizWrap.GizFlow
{
    using Helper;

    public static class GITImporter
    {
        [MenuItem("Nu Giz Wrap/Import/File/GIT")]
        static void Import()
        {
            string path = EditorUtility.OpenFilePanel("Import GIT File", TTUnityProject.GetDefaultFileExplorerPath(), "git");
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            Import(path, true);
        }

        public static void Import(string path, bool notify)
        {
            try
            {
                string[] lines = File.ReadAllLines(path);
                if (lines.Length == 0) return;

                GitManager gizFlow = Object.FindFirstObjectByType<GitManager>(FindObjectsInactive.Exclude);
                if (gizFlow == null) gizFlow = new GameObject("Gizmo Flow").AddComponent<GitManager>();
                gizFlow.FromLines(lines);
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
                return;
            }

            if (notify) EditorUtility.DisplayDialog("GizFlow Imported!", $"Successfully imported GizFlow from '{path}'", "OK");
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif