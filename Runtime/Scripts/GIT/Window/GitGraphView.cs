#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace TTModdingKit.GizFlow
{
    public class GitGraphView : GraphView
    {
        public UnityEvent<GitBox> OnSelectionChange { get; private set; } = new();

        public GitGraphView()
        {
            var stylesheet = TTResourceManager.LoadEditorAsset<StyleSheet>("Stylesheets/GitGraphView", ".uss");
            if (stylesheet != null) styleSheets.Add(stylesheet);

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            Insert(0, new GridBackground());
        }

        public void AddBox(GitBox node, Vector2? position=null)
        {
            Vector2 pos = position ?? contentViewContainer.WorldToLocal(layout.center);
            node.SetPosition(new Rect(pos, new Vector2(150, 200)));
            AddElement(node);
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