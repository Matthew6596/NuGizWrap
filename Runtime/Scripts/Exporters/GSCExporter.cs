#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace NuGizWrap
{
    using GameScene;

    public static class GSCExporter
    {
        public static long NU20Address, HEADAddress, NTBLAddress, MS00Address, SST0Address, INIDAddress, FDNSAddress;
        public static long BNDSAddress, DISPAddress, IABLAddress, TAS0Address, PORTAddress, TREFAddress, TST0Address;
        public static long VBIBAddress, SALIAddress, ALA3Address, DYNOAddress, GSNHAddress, PNTRAddress;

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

            PointerBlock.Instance.pointerAddresses.Clear();
            //Do initial complete file buffer write

            //write placehold for nu20
            //write texture/vertex/index data (if TCS)
            //write nu20
            //Save() all blocks in order
            //Calculate nu20 pointer manually
            //CalculatePointers() all blocks in order
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif