#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace NuGizWrap.GizFlow
{
    
    public class GitWindow_old : EditorWindow
    {
        private static GitGraphView_old gitGraph;
        private VisualElement boxPropertiesView;

        [MenuItem("Nu Giz Wrap/Giz Flow/Open Editor Window")]
        public static void OpenGitEditor()
        {
            // This method is called when the user selects the menu item in the Editor
            EditorWindow wnd = GetWindow<GitWindow_old>();
            wnd.titleContent = new GUIContent("GizFlow GIT Editor");
        }

        public void CreateGUI()
        {
            gitGraph ??= new GitGraphView_old();

            // Create a two-pane view with the left pane being fixed width
            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

            // Add the view to the visual tree by adding it as a child to the root element
            rootVisualElement.Add(splitView);

            // A TwoPaneSplitView always needs exactly two child elements
            var gitTabPane = new ScrollView(ScrollViewMode.Vertical);
            splitView.Add(gitTabPane);
            splitView.Add(gitGraph);

            //Create side tab
            gitTabPane.Add(new Label("===== Git Editor Side Panel ====="));

            // === Git Options ===
            gitTabPane.Add(new Label("Git Options"));
            Foldout optionsFoldout = new();
            gitTabPane.Add(optionsFoldout);

            // === Edit Controls ===
            gitTabPane.Add(new Label("Edit Controls"));
            //Reset view button
            var resetCamBtn = new Button(() => { gitGraph.FrameAll(); });
            resetCamBtn.Add(new Label("Reset View"));
            gitTabPane.Add(resetCamBtn);

            //Add flowbox and collapse buttons
            var addFlowBoxBtn = new Button(() => { GitManager_old.AddBox(new FlowBox_old("New FlowBox")); });
            addFlowBoxBtn.Add(new Label("Add FlowBox"));
            var addCollapseBtn = new Button(() => { GitManager_old.AddBox(new CollapseBox_old("New Collapse")); });
            addCollapseBtn.Add(new Label("Add Collapse Box"));
            gitTabPane.Add(addFlowBoxBtn);
            gitTabPane.Add(addCollapseBtn);

            // === Box Properties ===
            gitTabPane.Add(new Label("Box Properties"));
            boxPropertiesView = new ScrollView(ScrollViewMode.Vertical);
            gitTabPane.Add(boxPropertiesView);
            gitGraph.OnSelectionChange.AddListener((box) =>
            {
                boxPropertiesView.Clear();
                if (box != null) boxPropertiesView.Add(box.GetRootVisualElement());
            });
        }

        public static void SyncGraphNodes(List<GitBox_old> boxes)
        {
            gitGraph.ClearBoxes();
            foreach (var box in boxes)
            {
                gitGraph.AddBox(box);
                var pos = box.GetPosition();
                pos.x = box.x;
                pos.y = box.y;
                box.SetPosition(pos);
            }
        }

        public static void DeleteElements(IEnumerable<GraphElement> elements) => gitGraph.DeleteElements(elements);

        public static void AddElement(GraphElement el) => gitGraph.AddElement(el);
    }
}
#endif