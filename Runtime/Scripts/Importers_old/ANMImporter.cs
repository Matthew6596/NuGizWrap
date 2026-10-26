#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NuGizWrap.Animations
{
    using Helper;

    public static class ANMImporter
    {
        [MenuItem("Nu Giz Wrap/Import/File/ANM")]
        static void Import()
        {
            string path = EditorUtility.OpenFilePanel("Import ANM File", TTUnityProject.GetDefaultFileExplorerPath(), "anm");
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            Import(path, true);
        }

        public static void Import(string path, bool notify)
        {
            BinaryReader br = null;
            try
            {
                br = new(File.OpenRead(path));

                GameObject animObj = GameObject.Find("Animations");
                if (animObj == null) animObj = new GameObject("Animations");
                Transform animParent = animObj.transform;
                if (!animObj.TryGetComponent(out AnimManager am)) am = animObj.AddComponent<AnimManager>();

                int version = br.ReadInt32();
                am.version = version;

                int anmCount = br.ReadInt32();

                for(int i=0; i<anmCount; i++)
                {
                    var anm = new GameObject($"anm_anim_{i}").AddComponent<ANMAnim>();
                    anm.transform.SetParent(animParent);
                    anm.Load(br, version);
                }

                br.Close();
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
                br?.Close();
                return;
            }

            if (notify) EditorUtility.DisplayDialog("ANM Imported!", $"Successfully imported ANM from '{path}'", "OK");
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif