#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NuGizWrap.GameScene
{
    using Helper;

    public static class GSCImporter
    {
        [MenuItem("Nu Giz Wrap/Import/File/GSC")]
        static void Import()
        {
            string path = EditorUtility.OpenFilePanel("Import GSC File", TTUnityProject.GetDefaultFileExplorerPath(), "gsc");
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            Import(path, true);
        }

        public static void Import(string path, bool notify)
        {
            MemoryStream ms = null;
            try
            {
                ms = new(File.ReadAllBytes(path));

                GameObject gscObj = GameObject.Find("Game Scene");
                if (gscObj == null) gscObj = new GameObject("Game Scene");
                if (!gscObj.TryGetComponent(out NuGameScene gsc)) gsc = gscObj.AddComponent<NuGameScene>();

                gsc.Load(ms);

                ms.Close();
                ms.Dispose();
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
                ms?.Close();
                ms?.Dispose();
                return;
            }

            if (notify) EditorUtility.DisplayDialog("GSC Imported!", $"Successfully imported GSC from '{path}'", "OK");
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif