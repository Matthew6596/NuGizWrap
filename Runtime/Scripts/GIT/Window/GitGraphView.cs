#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace NuGizWrap.GizFlow
{
    public class GitGraphView : GraphView
    {
        public UnityEvent<GitBox> OnSelectionChange { get; private set; } = new();

        public GitGraphView()
        {
            var stylesheet = TTResourceManager.LoadEditorAsset<StyleSheet>("Stylesheets/GitGraphView", ".uss");
            if (stylesheet != null) styleSheets.Add(stylesheet);

            SetupZoom(ContentZoomer.DefaultMinScale/4, ContentZoomer.DefaultMaxScale*2);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            graphViewChanged = OnGraphViewChanged;

            Insert(0, new GridBackground());
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            //Deletions
            change.elementsToRemove?.ForEach(el =>
            {
                if (el is GitBox box) GitManager.RemoveBox(box);
                else if (el is Edge edge)
                {
                    var outputNode = edge.output.node as GitBox;
                    var inputNode = edge.input.node as GitBox;

                    outputNode.children.Remove(inputNode);
                    inputNode.parents.Remove((outputNode, outputNode.GetOutputPortIndex(edge.output)));
                }
            });

            //Moves
            change.movedElements?.OfType<GitBox>().ToList().ForEach(n => { 
                var pos = n.GetPosition().position;
                n.x = pos.x;
                n.y = pos.y;
            });

            //Creations
            change.edgesToCreate?.ForEach(edge =>
            {
                if (edge.input.node is not GitBox inputNode || edge.output.node is not GitBox outputNode) return;

                outputNode.children.Add(inputNode);
                inputNode.parents.Add((outputNode, outputNode.GetOutputPortIndex(edge.output)));
            });

            EditorUtility.SetDirty(GitManager.Instance);

            return change;
        }

        public void AddBox(GitBox node, Vector2? position=null)
        {
            Vector2 pos = position ?? contentViewContainer.WorldToLocal(layout.center);
            node.SetPosition(new Rect(pos, new Vector2(150, 200)));
            AddElement(node);
        }

        public void ClearBoxes()
        {
            foreach(var el in graphElements) if(el is Node n) RemoveElement(n);
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            var selectedBox = selection.Where(s => s is GitBox).FirstOrDefault();
            OnSelectionChange.Invoke(selectedBox as GitBox);
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            var selectedBox = selection.Where(s => s is GitBox).FirstOrDefault();
            OnSelectionChange.Invoke(selectedBox as GitBox);
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            var selectedBox = selection.Where(s => s is GitBox).FirstOrDefault();
            OnSelectionChange.Invoke(selectedBox as GitBox);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(p => p.direction != startPort.direction && p.portType == startPort.portType && p.node != startPort.node).ToList();
        }
    }
}
#endif