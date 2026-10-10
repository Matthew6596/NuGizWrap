#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Text;
using System.Text.RegularExpressions;

namespace NuGizWrap.GizFlow
{
    using Helper;
    
    public static class GITExporter
    {
        public static GitManager gm;
        public readonly static Dictionary<GitBox, int> boxIDs = new();

        [MenuItem("Nu Giz Wrap/Export/File/GIT")]
        public static void Export()
        {
            string path = EditorUtility.SaveFilePanel("Export GIT File", TTUnityProject.GetDefaultFileExplorerPath(), "levelgit", "git");
            if (string.IsNullOrEmpty(path) || !Directory.Exists(Path.GetDirectoryName(path))) return;

            Export(path, true);
        }

        public static void Export(string filepath, bool notify = false)
        {
            try
            {
                if(gm == null) gm = Object.FindFirstObjectByType<GitManager>(FindObjectsInactive.Exclude);
                if (gm == null)
                {
                    Debug.LogError("An Instance of GitManager must be present and active in your current scene to export a .GIT file.");
                    return;
                }

                //Generate a dictionary of ID's for the boxes
                int currentBoxID = -1;
                List<CollapseBox> collapses = new();
                List<FlowBox> flowboxes = new();

                void CountNodeID(GitBox box)
                {
                    if (box is CollapseBox collapse)
                    {
                        boxIDs.Add(collapse, currentBoxID);
                        collapses.Add(collapse);
                        currentBoxID--;
                    }
                    else if (box is FlowBox flowbox)
                    {
                        if (currentBoxID < 0) currentBoxID = 0;
                        boxIDs.Add(flowbox, currentBoxID);
                        flowboxes.Add(flowbox);
                        currentBoxID++;
                    }
                    else if (box is CustomGitBox custom)
                    {
                        foreach(var subn in custom.nodes) CountNodeID(subn);
                    }
                }

                foreach (var n in gm.boxes) CountNodeID(n);

                StringBuilder sb = new(gm.options.ToString());
                foreach(var n in collapses) sb.Append(n.ToString());
                foreach(var n in flowboxes) sb.Append(n.ToString());

                string finalStr = sb.ToString();

                var prefs = TTUnityProject.Prefs.gizFlow;
                if (prefs.exportCompressed)
                {
                    //Reduce whitespace
                    finalStr = finalStr.Replace("\t", "").Replace("\n\n", "\n");

                    //Reduce editor only values (position and name)
                    string pattern = @"(\r?\nx )([\.\d-]+)(\r?\ny )([\.\d-]+)(\r?\nName "")([^""]+)";
                    finalStr = Regex.Replace(finalStr, pattern, "${1}0${3}0$5");
                }

                //Replace variable placeholders
                foreach(var v in gm.variables)
                {
                    string varName = prefs.caseSensitiveVariables ? v.name : v.name.ToLower();
                    finalStr = finalStr.Replace($"${varName}", v.value);
                }

                File.WriteAllText(filepath, finalStr[..^1]);

                boxIDs.Clear();

                if (notify) EditorUtility.DisplayDialog("GIT Exported!", $"Successfully exported GIT to '{filepath}'", "OK");
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
            }
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif