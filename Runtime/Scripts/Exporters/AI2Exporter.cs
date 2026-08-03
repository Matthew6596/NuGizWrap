#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace TTModdingKit.AI
{
    using Helper;

    public static class AI2Exporter
    {
        const int MaxAI2Version = 21;

        [MenuItem("TT Modding/Export/File/AI2")]
        static void Export()
        {
            string path = EditorUtility.SaveFilePanel("Export AI2 File", TTUnityProject.GetDefaultFileExplorerPath(), "levelai2", "ai2");
            if (string.IsNullOrEmpty(path) || !Directory.Exists(Path.GetDirectoryName(path))) return;

            Export(path, true);
        }

        public static void Export(string filepath, bool notify = false)
        {
            EditorUtility.DisplayProgressBar("Exporting", $"Exporting AI2...", 0);

            try
            {
                BinaryWriter bw = new(File.OpenWrite(filepath));

                var aiprefs = TTUnityProject.Instance.prefs.ai2;
                int version = aiprefs.alwaysExportMaxVersion ? MaxAI2Version : aiprefs.version;
                bw.Write(version);

                var paths = Object.FindObjectsByType<AIPath>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
                int pathCount = paths.Length;
                bw.Write(pathCount);
                foreach(var path in paths) path.ToBytes(bw, version);

                if (version >= 19)
                {
                    bw.Write((short)0);
                    /*var unk39s = Object.FindObjectsByType<Unk39>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
                    short unk39Count = (short)unk39s.Length;
                    bw.Write(unk39Count);
                    foreach(var unk39 in unk39s) unk39.ToBytes(bw);*/
                }

                if (version >= 4)
                {
                    var triggers = Object.FindObjectsByType<Trigger>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
                    int triggerCount = triggers.Length;
                    bw.Write(triggerCount);
                    foreach (var trigger in triggers) trigger.ToBytes(bw, version);
                }

                if (version >= 6)
                {
                    var locators = Object.FindObjectsByType<Locator>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
                    int locatorCount = locators.Length;
                    bw.Write(locatorCount);
                    foreach (var locator in locators) locator.ToBytes(bw, version);

                    if (version >= 18)
                    {
                        var locatorSets = Object.FindObjectsByType<LocatorSet>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
                        int locatorSetCount = locatorSets.Length;
                        bw.Write(locatorSetCount);
                        foreach (var locatorSet in locatorSets) locatorSet.ToBytes(bw, locators);
                    }
                }

                var creatures = Object.FindObjectsByType<Creature>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
                int creatureCount = creatures.Length;
                bw.Write(creatureCount);
                foreach (var creature in creatures) creature.ToBytes(bw, version);

                if (version >= 13)
                {
                    var obstacles = Object.FindObjectsByType<AIObstacle>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
                    int obstaclesCount = obstacles.Length;
                    bw.Write(obstaclesCount);
                    foreach (var obstacle in obstacles) obstacle.ToBytes(bw, version);
                }

                if (version >= 7)
                {
                    bw.Write(5);
                    bw.Write(new char[] { 'L', 'E', 'G', 'O', '\0' });
                }

                bw.Write(1);

                if (notify) EditorUtility.DisplayDialog("AI2 Exported!", $"Successfully exported AI2 to '{filepath}'", "OK");
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