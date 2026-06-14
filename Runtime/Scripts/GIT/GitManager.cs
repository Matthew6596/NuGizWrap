#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.GizFlow
{
    public class GitManager : MonoBehaviour
    {
        public static GitManager Instance { get; private set; }

        public GitOptions gitOptions = new();

        [NonSerialized]
        public List<GitBox> boxes = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                EditorUtility.DisplayDialog("GitManager Already Exists", $"Only one GitManager can exist in a scene, and there is already one on gameObject '{Instance.name}'.", "OK");
                Destroy(gameObject);
            }
        }

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

        public static GitBox FindBoxByID(int boxID) => Instance.boxes.Where(b=>b.boxID == boxID).FirstOrDefault();

        public string[] ToLines()
        {
            List<string> lines = new() { "GitOptions {" };
            lines.AddRange(gitOptions.ContentToLines());
            lines.Add("}\n");

            boxes.Sort(new Comparison<GitBox>((b1, b2) => b1.boxID - b2.boxID));
            foreach(var box in boxes)
            {
                lines.Add($"{box.ID} {{");
                lines.AddRange(box.ContentToLines());
                lines.Add("}\n");
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
                        CollapseBox collapse = new();
                        collapse.ContentFromLines(lines, ref index);
                        boxes.Add(collapse);
                        break;
                    case "FlowBox":
                        FlowBox flow = new();
                        flow.ContentFromLines(lines, ref index);
                        boxes.Add(flow);
                        break;
                }
            }
        }
    }
}
#endif