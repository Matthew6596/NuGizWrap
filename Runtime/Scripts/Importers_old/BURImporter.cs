#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NuGizWrap.Lighting
{
    using Helper;

    public static class BURImporter
    {
        [MenuItem("Nu Giz Wrap/Import/File/BUR")]
        static void Import()
        {
            string path = EditorUtility.OpenFilePanel("Import BUR File", TTUnityProject.GetDefaultFileExplorerPath(), "bur");
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            Import(path, true);
        }

        public static void Import(string path, bool notify)
        {
            BinaryReader br = null;
            try
            {
                br = new(File.OpenRead(path));

                GameObject burObj = GameObject.Find("RTL Burnout");
                if (burObj == null) burObj = new GameObject("RTL Burnout");
                Transform burParent = burObj.transform;
                if (!burObj.TryGetComponent(out RTLBurnoutManager bm)) bm = burObj.AddComponent<RTLBurnoutManager>();

                bm.Load(br);

                br.Close();
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
                br?.Close();
                return;
            }

            if (notify) EditorUtility.DisplayDialog("BUR Imported!", $"Successfully imported BUR from '{path}'", "OK");
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif