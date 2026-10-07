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
        public UnityEvent<GitBoxGraphNode> OnSelectionChange { get; private set; } = new();

        public Vector2 Center => contentViewContainer.WorldToLocal(layout.center);
        public List<GitBoxGraphNode> boxes = new();

        public GitGraphView()
        {
            var stylesheet = TTResourceManager.LoadEditorAsset<StyleSheet>("Stylesheets/GitGraphView", ".uss");
            if (stylesheet != null) styleSheets.Add(stylesheet);

            SetupZoom(ContentZoomer.DefaultMinScale / 4, ContentZoomer.DefaultMaxScale * 2);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            //graphViewChanged = OnGraphViewChanged;

            Insert(0, new GridBackground());
        }

        public void AddBox(GitBoxGraphNode node)
        {
            node.SetPosition(new Rect(node.box.position, new Vector2(150, 200)));
            boxes.Add(node);
            AddElement(node);
        }

        public GitBox GetGitBox(GitBoxGraphNode node)
        {
            int ind = boxes.IndexOf(node);
            return ind == -1 || node == null ? null : boxes[ind].box;
        }

        public GitBoxGraphNode GetGraphNode(GitBox box) => boxes.Where(n=>n.box == box).FirstOrDefault();

        /*private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            //Deletions
            change.elementsToRemove?.ForEach(el =>
            {
                if (el is GitBox_old box) GitManager_old.RemoveBox(box);
                else if (el is Edge edge)
                {
                    var outputNode = edge.output.node as GitBox_old;
                    var inputNode = edge.input.node as GitBox_old;

                    outputNode.children.Remove(inputNode);
                    inputNode.parents.Remove((outputNode, outputNode.GetOutputPortIndex(edge.output)));
                }
            });

            //Moves
            change.movedElements?.OfType<GitBox_old>().ToList().ForEach(n => {
                var pos = n.GetPosition().position;
                n.x = pos.x;
                n.y = pos.y;
            });

            //Creations
            change.edgesToCreate?.ForEach(edge =>
            {
                if (edge.input.node is not GitBox_old inputNode || edge.output.node is not GitBox_old outputNode) return;

                outputNode.children.Add(inputNode);
                inputNode.parents.Add((outputNode, outputNode.GetOutputPortIndex(edge.output)));
            });

            EditorUtility.SetDirty(GitManager_old.Instance);

            return change;
        }*/

        public void ClearBoxes()
        {
            var elsToDelete = graphElements.Where(e => e is Node || e is Edge);
            DeleteElements(elsToDelete);
            boxes.Clear();
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            var selectedBox = selection.Where(s => s is GitBoxGraphNode).FirstOrDefault();
            OnSelectionChange.Invoke(selectedBox as GitBoxGraphNode);
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            var selectedBox = selection.Where(s => s is GitBoxGraphNode).FirstOrDefault();
            OnSelectionChange.Invoke(selectedBox as GitBoxGraphNode);
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            var selectedBox = selection.Where(s => s is GitBoxGraphNode).FirstOrDefault();
            OnSelectionChange.Invoke(selectedBox as GitBoxGraphNode);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(p => p.direction != startPort.direction && p.portType == startPort.portType && p.node != startPort.node).ToList();
        }
    }
}
#endif