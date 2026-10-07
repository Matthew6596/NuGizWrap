#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Giz = UnityEngine.Gizmos;
using Object = UnityEngine.Object;

namespace NuGizWrap.GizFlow
{
    using Helper;

    public class GitManager_old : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private string serializedGraph = "";

        private static GitManager_old _instance;
        public static GitManager_old Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GitManager_old>(FindObjectsInactive.Exclude);
                    if (_instance == null)
                    {
                        _instance = new GameObject("Gizmo Flow").AddComponent<GitManager_old>();
                    }
                }
                return _instance;
            }
        }

        public GitOptions_old gitOptions = new();

        public List<GitBox_old> boxes = new();

        private void OnDrawGizmosSelected()
        {
            //Draw connections and stuff in scene
            /*foreach(var box in boxes)
            {
                if (!box.TryGetProperty("Num_Gizmos", out var prop)) continue;
                int numGizmos = prop.GetValue<int>();
                if (numGizmos <= 0) continue;

                foreach(var gizmo)
                //Draw arrow to children
                foreach(var child in box.children)
                {

                }
            }*/
        }

        [MenuItem("Nu Giz Wrap/Analysis/Log Git Boxes")]
        private static void LogBoxes()
        {
            Debug.Log($"Total Boxes: {Instance.boxes.Count}");
        }

        public static GitBox_old FindBoxByID(int boxID) => Instance.boxes.Where(b=>b.boxID == boxID).FirstOrDefault();

        public static void AddBox(GitBox_old box)
        {
            Instance.boxes.Add(box);
            GitWindow_old.SyncGraphNodes(Instance.boxes);
        }

        public static void RemoveBox(GitBox_old box)
        {
            Instance.boxes.Remove(box);
            GitWindow_old.SyncGraphNodes(Instance.boxes);
        }

        public string[] ToLines()
        {
            List<string> lines = new() { "GitOptions {" };
            lines.AddRange(gitOptions.ContentToLines());
            lines.Add("}");
            lines.Add("");

            //Order by descending when negative and ascending when positive (-1, -2, -3, 0, 1, 2, 3)
            boxes.OrderBy(b => b.boxID < 0 ? 1 : b.boxID == 0 ? 2 : 3).ThenBy(b => b.boxID < 0 ? -b.boxID : b.boxID);

            foreach(var box in boxes)
            {
                lines.Add($"{box.ID} {{");
                lines.AddRange(box.ContentToLines());
                lines.Add("}");
                lines.Add("");
            }

            return lines.ToArray();
        }

        public void FromLines(string[] lines)
        {
            int index = 0;
            while(index < lines.Length)
            {
                string line = lines[index].Trim();
                index++;

                if (line == string.Empty || line.Contains('}')) continue;

                int brackInd = line.IndexOf('{');
                if (brackInd == -1) continue;
                string nodeType = line[..(brackInd - 1)].Trim();

                switch (nodeType)
                {
                    case "GitOptions": gitOptions.ContentFromLines(lines, ref index); break;
                    case "Collapse":
                        CollapseBox_old collapse = new();
                        collapse.ContentFromLines(lines, ref index);
                        AddBox(collapse);
                        collapse.RefreshVisualElements();
                        break;
                    case "FlowBox":
                        FlowBox_old flow = new();
                        flow.ContentFromLines(lines, ref index);
                        AddBox(flow);
                        flow.RefreshFlowBoxElements();
                        break;
                }
            }
        }

        public void OnBeforeSerialize()
        {
            return;
            serializedGraph = string.Join('\n', ToLines());
        }

        public void OnAfterDeserialize()
        {
            return;
            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(serializedGraph)) FromLines(serializedGraph.Split('\n'));
            };
        }
    }
}
#endif