#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Obj = UnityEngine.Object;

namespace NuGizWrap.GizFlow
{
    using Helper;
    using System;
    using System.Linq;

    [ScriptedImporter(1, "git")]
    public class GITAssetImporter : ScriptedImporter
    {
        [Tooltip("Rotates the entire graph 90deg clockwise. Useful for converting a vertical graph to horizontal.")]
        public bool rotate90Degrees;
        [Tooltip("Multiplies each box position by the given scalar. Useful to avoid overlapping boxes.")]
        public float boxPositionScalar = 1;

        public static List<(GitBox, int, int)> nodeParents = new();

        public override void OnImportAsset(AssetImportContext ctx)
        {
            nodeParents.Clear();

            TextReader tr = new(File.ReadAllText(ctx.assetPath));

            GameObject rootGitObj = new("Gizmo Flow");
            var gm = rootGitObj.AddComponent<GitManager>();
            ctx.AddObjectToAsset(rootGitObj.name, rootGitObj);

            //Load all nodes
            List<GitBox> boxes = new();
            while (TryReadNextNode(tr, gm, out var node))
            {
                if (node is GitOptionsAsset opts)
                {
                    gm.options = opts;
                    ctx.AddObjectToAsset(opts.name, opts);
                }
                else
                {
                    var gitbox = node as GitBox;
                    gitbox.position *= boxPositionScalar;

                    if (rotate90Degrees)
                    {
                        var angR = Mathf.Atan2(gitbox.position.y, gitbox.position.x);
                        var mag = gitbox.position.magnitude;
                        angR += Mathf.PI / 2;
                        gitbox.position = new(Mathf.Cos(angR) * mag, Mathf.Sin(angR) * mag);
                    }

                    boxes.Add(gitbox);
                }
            }

            gm.boxes = boxes.ToArray();

            //Resolve children/parents
            int connCount = nodeParents.Count;
            gm.connections = new Connection[connCount];
            for(int i=0; i < connCount; i++)
            {
                var parent = nodeParents[i];
                var p = boxes.Where(b => b.boxID == parent.Item2).FirstOrDefault();
                var child = boxes.Where(b=>b.boxID == parent.Item1.boxID).FirstOrDefault();
                gm.connections[i] = new Connection()
                {
                    parent = p,
                    parentOutput = parent.Item3,
                    child = child,
                };
            }

            nodeParents.Clear();
        }

        private bool TryReadNextNode(TextReader tr, GitManager gm, out object node)
        {
            node = null;
            string id = tr.NextToken();
            if (id == string.Empty) return false;

            int brackInd = tr.NextIndexOf('{');
            if (brackInd == -1) return false;
            tr.index = brackInd + 1;

            switch (id)
            {
                case "GitOptions":
                    var options = new GitOptions(tr, gm);
                    node = ScriptableObject.CreateInstance<GitOptionsAsset>();
                    (node as GitOptionsAsset).name = "Git Options";
                    (node as GitOptionsAsset).node = options;
                    break;
                case "Collapse":
                    node = new CollapseBox(tr, gm);
                    //var collapse = new CollapseBox(tr,gm);
                    //node = ScriptableObject.CreateInstance<CollapseBoxAsset>();
                    //(node as CollapseBoxAsset).name = $"ID{collapse.boxID}_{collapse.name}";
                    //(node as CollapseBoxAsset).node = collapse;
                    break;
                case "FlowBox":
                    node = new FlowBox(tr, gm);
                    //var flowbox = new FlowBox(tr,gm);
                    //node = ScriptableObject.CreateInstance<FlowBoxAsset>();
                    //(node as FlowBoxAsset).name = $"ID{flowbox.boxID}_{flowbox.name}";
                    //(node as FlowBoxAsset).node = flowbox;
                    break;
                default: break;
            }

            return node != null;
        }

        public class TextReader
        {
            public int index;
            public string text;
            public bool AtEnd => index >= text.Length;

            public TextReader(string text)
            {
                index = 0;
                this.text = text;
            }

            public string NextToken()
            {
                if (!TryAdvanceWhileWhitespace()) return string.Empty;
                int startIndex = index;
                if (!TryAdvanceToWhitespace()) return text[startIndex..];
                return text[startIndex..index];
            }

            public string ParseString()
            {
                int startIndex = text.IndexOf('"', index)+1;
                int endIndex = text.IndexOf('"', startIndex);
                return text[startIndex..endIndex];
            }

            public int ParseInt()
            {
                string t = NextToken();
                if (int.TryParse(t, out int r)) return r;
                return 0;
            }

            public float ParseFloat()
            {
                string t = NextToken();
                if (float.TryParse(t, out float r)) return r;
                return 0;
            }

            public Vector3 ParseVector3() => new(ParseFloat(), ParseFloat(), ParseFloat());
            public Color ParseColor() => new(ParseFloat(), ParseFloat(), ParseFloat());

            public string GetLine()
            {
                int lineEndIndex = text.IndexOf('\n', index);
                int lineStartIndex = text.LastIndexOf("\n", index)+1;
                return text[lineStartIndex..lineEndIndex];
            }

            public bool TryAdvanceTo(params char[] matchingChars)
            {
                while (!AtEnd && !matchingChars.Contains(text[index])) index++;
                return !AtEnd;
            }
            public bool TryAdvanceWhile(params char[] matchingChars)
            {
                while (!AtEnd && matchingChars.Contains(text[index])) index++;
                return !AtEnd;
            }
            public bool TryAdvanceToWhitespace() => TryAdvanceTo(' ', '\t', '\n', '\r', '\v', '\f');
            public bool TryAdvanceWhileWhitespace() => TryAdvanceWhile(' ', '\t', '\n', '\r', '\v', '\f');

            public void AdvanceToBracketEnd()
            {
                int indent = 1;
                while (!AtEnd)
                {
                    switch (text[index])
                    {
                        case '}': indent--; if (indent == 0) { index++; return; } break;
                        case '{': indent++; break;
                        default: break;
                    }
                    index++;
                }
            }

            public int NextIndexOf(char c) => text.IndexOf(c, index);
            public int NextIndexOf(string s) => text.IndexOf(s, index);
        }
    }
}
#endif