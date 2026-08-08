#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace NuGizWrap.Tools 
{
    public static class GameFileCleaner
    {
        public static void RemovePAKs(TTGame game)
        {
            string dir = Path.GetDirectoryName(TTUnityProject.GetGamePath(game));
            if (!Directory.Exists(dir)) return;

            var pakFiles = Directory.EnumerateFiles(dir, "*.PAK", SearchOption.AllDirectories);
            int pakCount = pakFiles.Count();
            int pakDelCount = pakCount;

            foreach (var pak in pakFiles)
            {
                try
                {
                    File.Delete(pak);
                }
                catch(IOException ioe)
                {
                    Debug.LogError($"Failed to delete {game} PAK file: {pak}\n{ioe}");
                    pakDelCount--;
                }
            }

            Debug.Log($"Successfully deleted {pakDelCount}/{pakCount} PAK files in {game}.");
        }

        [MenuItem("Nu Giz Wrap/Tools/Remove PAKs/TCS")]
        public static void RemovePAKsTCS() => RemovePAKs(TTGame.TCS);
        [MenuItem("Nu Giz Wrap/Tools/Remove PAKs/LIJ1")]
        public static void RemovePAKsLIJ1() => RemovePAKs(TTGame.LIJ1);
        [MenuItem("Nu Giz Wrap/Tools/Remove PAKs/LB1")]
        public static void RemovePAKsLB1() => RemovePAKs(TTGame.LB1);
    }
}
#endif