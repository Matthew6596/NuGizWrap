#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace NuGizWrap.GizFlow
{
    using Helper;

    public static class GITExporter
    {
        [MenuItem("Nu Giz Wrap/Export/File/GIT")]
        static void Export()
        {
            string path = EditorUtility.SaveFilePanel("Export GIT File", TTUnityProject.GetDefaultFileExplorerPath(), "levelgit", "git");
            if (string.IsNullOrEmpty(path) || !Directory.Exists(Path.GetDirectoryName(path))) return;

            Export(path, true);
        }

        public static void Export(string filepath, bool notify = false)
        {
            EditorUtility.DisplayProgressBar("Exporting", $"Exporting GIT...", 0);

            try
            {
                File.WriteAllLines(filepath, GitManager.Instance.ToLines());

                if (notify) EditorUtility.DisplayDialog("GIT Exported!", $"Successfully exported GIT to '{filepath}'", "OK");
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif