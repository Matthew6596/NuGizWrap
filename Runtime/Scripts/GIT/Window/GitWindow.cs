#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TTModdingKit.GizFlow
{
    
    public class GitWindow : EditorWindow
    {
        private static GitGraphView gitGraph;
        private VisualElement boxPropertiesView;

        [MenuItem("TT Modding/Giz Flow/Open Editor Window")]
        public static void OpenGitEditor()
        {
            // This method is called when the user selects the menu item in the Editor
            EditorWindow wnd = GetWindow<GitWindow>();
            wnd.titleContent = new GUIContent("GizFlow GIT Editor");
        }

        public void CreateGUI()
        {
            gitGraph ??= new GitGraphView();

            // Create a two-pane view with the left pane being fixed with
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
            var addFlowBoxBtn = new Button(() => { gitGraph.AddBox(new FlowBox("New FlowBox")); });
            addFlowBoxBtn.Add(new Label("Add FlowBox"));
            var addCollapseBtn = new Button(() => { gitGraph.AddBox(new CollapseBox("New Collapse")); });
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
    }
}
#endif