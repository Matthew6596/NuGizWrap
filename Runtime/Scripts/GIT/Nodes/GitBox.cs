#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TTModdingKit.GizFlow
{
    public abstract class GitBox : Node, IGitNode
    {
        public enum NodeOutput { NoOutput=0, False=1, True=2 }

        public abstract string ID { get; }

        public int boxID;
        public List<GitBox> children;
        public List<(GitBox,int)> parents;
        public float x, y;

        protected Port InputPort { get; private set; }
        protected List<Port> OutputPorts { get; private set; } = new();

        protected VisualElement rootVisualElement;

        public int GetOutputPortIndex(Port port) => OutputPorts.IndexOf(port);

        public GitBox(string name)
        {
            this.name = name;
            title = name;

            children = new();
            parents = new();

            rootVisualElement = new();
            RefreshVisualElements();

            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(NodeOutput));
            InputPort.name = "Input";
            inputContainer.Add(InputPort);
        }

        public void RefreshVisualElements()
        {
            //Create visual elements
            rootVisualElement.Clear();

            //Name field
            title = name;
            var nameField = new TextField("Name");
            nameField.SetValueWithoutNotify(name);
            nameField.RegisterValueChangedCallback((s) => { this.name = s.newValue; title = this.name; });
            rootVisualElement.Add(nameField);
        }

        protected void AddOutputPort(string name, string portName, Color? color=null)
        {
            if (OutputPorts.Where(p => p.name == name).FirstOrDefault() != null) return;
            Port p = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(NodeOutput));
            p.name = name;
            p.portName = portName;
            if (color.HasValue) p.portColor = color.Value;
            OutputPorts.Add(p);
            outputContainer.Add(p);
        }

        protected void RemoveOutputPort(Port port)
        {
            GitWindow.DeleteElements(port.connections);
            outputContainer.Remove(port);
            OutputPorts.Remove(port);
        }
        protected Port GetOutputPort(string name) => OutputPorts.Where(p=>p.name == name).FirstOrDefault();

        protected void ClearOutputPorts()
        {
            foreach(var port in OutputPorts) GitWindow.DeleteElements(port.connections);
            outputContainer.Clear();
            OutputPorts.Clear();
            //RefreshPorts();
        }

        protected void RefreshPortConnections()
        {
            for(int i=parents.Count-1; i>=0; i--)
            {
                var parent = parents[i];
                var parentBox = parent.Item1;
                if (parentBox == null)
                {
                    parents.RemoveAt(i);
                }
                else if (parent.Item2 >= parentBox.OutputPorts.Count)
                {
                    if (parentBox.children.Contains(this)) parentBox.Remove(this);
                    parents.RemoveAt(i);
                }
                else
                {
                    parentBox.OutputPorts[parent.Item2].ConnectTo(InputPort);
                }
            }
        }

        public abstract void ContentFromLines(IEnumerable<string> linesIen, ref int index);

        protected void BasePropsFromLines(IEnumerable<string> linesIen, ref int index)
        {
            string[] lines = linesIen.ToArray();

            string line = lines[index].Trim();
            boxID = int.Parse(line["BoxID ".Length..]);
            index++;

            while (index < lines.Length)
            {
                line = lines[index].Trim();
                int ind = line.IndexOf(' ');

                string propName = line[..ind];
                switch (propName)
                {
                    case "Parent":
                        int space2Ind = line.IndexOf(" ", ind+1);
                        int parentBoxId = int.Parse(line[(ind+1)..space2Ind]);
                        int outputNum = int.Parse(line[(space2Ind+1)..]);
                        EditorApplication.delayCall += () => {
                            var parentBox = GitManager.FindBoxByID(parentBoxId);
                            parents.Add((parentBox, outputNum));
                            if (parentBox != null && outputNum < parentBox.OutputPorts.Count)
                            {
                                GitWindow.AddElement(parentBox.OutputPorts[outputNum].ConnectTo(InputPort));
                            }
                        };
                        break;
                    case "Child":
                        int childInd = int.Parse(line[(ind + 1)..]);
                        EditorApplication.delayCall += () => { children.Add(GitManager.FindBoxByID(childInd)); };
                        break;
                    case "x":
                        string xStr = line[(ind + 1)..];
                        x = float.Parse(xStr);
                        break;
                    case "y":
                        string yStr = line[(ind + 1)..];
                        y = float.Parse(yStr);
                        break;
                    default: goto LoopEnd;
                }

                index++;
            }
            LoopEnd:
            this.name = line["Name \"".Length..^1];
            index++;
        }

        public abstract IEnumerable<string> ContentToLines();

        protected IEnumerable<string> BasePropsToLines()
        {
            List<string> lines = new() { $"\tBoxID {boxID}" };
            foreach(var p in parents) lines.Add($"\tParent {p.Item1.boxID} {p.Item2}");
            foreach(var c in children) lines.Add($"\tChild {c.boxID}");
            lines.Add($"\tx {x}");
            lines.Add($"\ty {y}");
            lines.Add($"\tName \"{this.name}\"");
            return lines;
        }

        public VisualElement GetRootVisualElement() => rootVisualElement;
    }
}
#endif