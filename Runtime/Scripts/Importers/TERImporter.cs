#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TTModdingKit.Terrain
{
    using Helper;

    public static class TERImporter
    {
        [MenuItem("TT Modding/Import/File/TER")]
        static void Import()
        {
            string path = EditorUtility.OpenFilePanel("Import TER File", "", "ter");
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            Import(path, true);
        }

        public static void Import(string path, bool notify)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                if (bytes.Length == 0) return;
                LoadBytes(bytes);
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
                return;
            }

            if (notify) EditorUtility.DisplayDialog("Terrain Imported!", $"Successfully imported Terrain from '{path}'", "OK");
        }

        public static int LoadBytes(byte[] bytes, int index=0)
        {
            GameObject terParentObj = GameObject.Find("Terrain");
            Transform terParent = terParentObj == null ? new GameObject("Terrain").transform : terParentObj.transform;

            int dataLen = bytes.ReadInt(ref index) * 2;
            index = dataLen;

            int startPos = index;
            short objCount = bytes.ReadShort(ref index);

            return index;
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif