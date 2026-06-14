#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<GitBox> parents;
        public float x, y;

        protected Port InputPort { get; private set; }

        protected VisualElement rootVisualElement;

        public GitBox(string name)
        {
            this.name = name;
            title = name;

            children = new();
            parents = new();
            //properties = new();

            //Create visual elements
            rootVisualElement = new();

            //Name field
            var nameField = new TextField("Name");
            nameField.SetValueWithoutNotify(name);
            nameField.RegisterValueChangedCallback((s) => { this.name = s.newValue; title = this.name; });
            rootVisualElement.Add(nameField);

            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(NodeOutput));
            inputContainer.Add(InputPort);
            //outputContainer.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(NodeOutput)));
        }

        public abstract void ContentFromLines(IEnumerable<string> linesIen, ref int index);

        protected void BasePropsFromLines(IEnumerable<string> linesIen, ref int index)
        {
            string[] lines = linesIen.ToArray();
            string line;
            while(index < lines.Length)
            {
                line = lines[index].Trim();
                index++;
                if (line.Contains('}')) break;

                //Read properties

            }
        }

        public abstract IEnumerable<string> ContentToLines();

        protected IEnumerable<string> BasePropsToLines()
        {
            throw new NotImplementedException();
        }

        public VisualElement GetRootVisualElement() => rootVisualElement;
    }
}
#endif