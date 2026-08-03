#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;

namespace TTModdingKit.GizFlow
{
    public class CollapseBox : GitBox
    {
        public override string ID => "Collapse";

        private bool _collapsed = false;
        private Toggle nodeToggle, propToggle;

        public CollapseBox() : base("New Collapse Box") { CreateCollapseBox(); }

        public CollapseBox(string name) : base(name) { CreateCollapseBox(); }

        private void CreateCollapseBox()
        {
            //ClearOutputPorts();
            AddOutputPort("collapse_output", "Output");
            //RefreshPortConnections();

            //Collapse toggle that appears on the node box
            nodeToggle = new("Collapsed");
            nodeToggle.RegisterValueChangedCallback((e) => { Collapse(e.newValue); });
            contentContainer.Add(nodeToggle);

            //Collapse toggle that appears on the properties side-panel
            propToggle = new("Collapsed");
            propToggle.RegisterValueChangedCallback((e) => { Collapse(e.newValue); });
            rootVisualElement.Add(propToggle);

            //Delay call so child nodes can be created first
            EditorApplication.delayCall += () => { Collapse(_collapsed); };
        }

        //public void ToggleCollapseChildren() => Collapse(!_collapsed);

        public void Collapse(bool collapsed)
        {
            _collapsed = collapsed;

            //Ensure toggle inputs match collapsed state
            nodeToggle.SetValueWithoutNotify(collapsed);
            propToggle.SetValueWithoutNotify(collapsed);

            //Prevent connections while collapsed
            outputContainer.visible = !collapsed;

            void CollapseChildren(Node node)
            {
                //Iterate all node ports and connections for child nodes
                foreach(var childPort in node.outputContainer.Children().Where(v=>v is Port))
                {
                    foreach(var conn in (childPort as Port).connections)
                    {
                        var childNode = conn.input.node;

                        //Skip already collapsed nodes (avoid issues with loops)
                        if (childNode.visible != collapsed) continue;

                        //Collapse child nodes
                        CollapseChildren(childNode);

                        //Make this child node and it's connection invisible
                        childNode.visible = !collapsed;
                        conn.visible = !collapsed;
                    }
                }
            }

            //Collapse child boxes
            CollapseChildren(this);
        }

        public override IEnumerable<string> ContentToLines()
        {
            var lines = BasePropsToLines().ToList();
            if (_collapsed) lines.Add($"\tCollapseHead");
            return lines;
        }

        public override void ContentFromLines(IEnumerable<string> linesIen, ref int index)
        {
            BasePropsFromLines(linesIen, ref index);
            string[] lines = linesIen.ToArray();
            while (index < lines.Length && !lines[index].Contains('}')) index++;
            index++;
        }
    }
}
#endif