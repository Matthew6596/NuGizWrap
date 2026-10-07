#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Collections.Generic;

namespace NuGizWrap.GizFlow
{
    using Helper;

    public class GitWindow : EditorWindow
    {
        private bool nullGm;
        private GitManager _gm;
        public GitManager gm { get => _gm; 
            set
            {
                _gm = value;
                nullGm = _gm == null;
                Refresh();
            }
        }

        public GitGraphView graphView;

        [MenuItem("Nu Giz Wrap/Windows/Git (Giz Flow) Editor Window")]
        public static void ShowWindow() => ShowWindow(null);

        public static GitWindow ShowWindow(GitManager gm)
        {
            var window = EditorWindow.GetWindow<GitWindow>();

            if(!TTUnityProject.Prefs.gizFlow.onlyOneGitWindow && (window.gm != null && window.gm != gm)) 
                window = EditorWindow.CreateWindow<GitWindow>();
            window.gm = gm;

            return window;
        }

        public void Refresh()
        {
            rootVisualElement.Clear();
            titleContent = new GUIContent($"GitWindow - {(gm == null ? "No GitManager" : gm.name)}");
            CreateGUI();
        }

        private void OnEnable()
        {
            graphView = new();
        }

        private void CreateGUI()
        {
            if (gm == null)
            {
                rootVisualElement.Add(new Label("No GitManager is connected to this window.\nPlease close this window or connect another GitManager below."));
                var gmField = new ObjectField("GitManager")
                {
                    allowSceneObjects = true,
                    objectType = typeof(GitManager)
                };
                gmField.RegisterValueChangedCallback((o) => gm = (GitManager)o.newValue);
                rootVisualElement.Add(gmField);
                return;
            }

            LoadGraph();

            var splitView = new TwoPaneSplitView(0, 240, TwoPaneSplitViewOrientation.Horizontal);

            // Add the view to the visual tree by adding it as a child to the root element
            rootVisualElement.Add(splitView);

            // A TwoPaneSplitView always needs exactly two child elements
            var sideTab = new ScrollView(ScrollViewMode.Vertical);
            splitView.Add(sideTab);
            splitView.Add(graphView);

            //Create side tab
            sideTab.Add(new HelpBox("Git Editor Side Panel", HelpBoxMessageType.None));
            var mainBox = new Box();
            sideTab.Add(mainBox);

            mainBox.Add(new Label(""));
            var addBoxRoot = new VisualElement();
            addBoxRoot.style.flexDirection = FlexDirection.Row;
            mainBox.Add(addBoxRoot);

            var addBoxCustomField = new ObjectField()
            {
                objectType = typeof(CustomGitNodeAsset),
                allowSceneObjects = false,
                visible = false
            };
            List<string> boxTypes = new() { "FlowBox", "Collapse", "Custom" };
            var addBoxDropdown = new DropdownField(boxTypes, 0);
            addBoxDropdown.RegisterValueChangedCallback((o) => 
            {
                string type = o.newValue;
                addBoxCustomField.visible = type == "Custom";
            });
            var addBoxBtn = new Button(() =>
            {
                string boxType = addBoxDropdown.value;
                if (boxType == "Custom" && addBoxCustomField.value == null) Debug.Log("Cannot add null custom box");
                else
                {
                    var box = (boxType) switch
                    {
                        "Collapse" => new CollapseBox("New Collapse", graphView.Center, false),
                        "FlowBox" => new FlowBox("New FlowBox", graphView.Center, new FlowBox.Gizmo[0], false, false, false, default, default, 0),
                        "Custom" => ((CustomGitNodeAsset)addBoxCustomField.value).node.CreateCopy(),
                        _ => throw new NotSupportedException($"GitBox type '{boxType}' is not supported.")
                    };

                    graphView.AddBox(new GitBoxGraphNode(box));
                }
            });
            addBoxBtn.Add(new Label("Add Box"));
            
            addBoxRoot.Add(addBoxBtn);
            addBoxRoot.Add(addBoxDropdown);
            addBoxRoot.Add(addBoxCustomField);

            var saveBtn = new Button(SaveChanges);
            saveBtn.Add(new Label("Save"));
            mainBox.Add(saveBtn);

            var resetCamBtn = new Button(() => { graphView.FrameAll(); });
            resetCamBtn.Add(new Label("Reset View"));
            mainBox.Add(resetCamBtn);

        }

        private void LoadGraph()
        {
            graphView.ClearBoxes();

            var pos = gm.cameraPosition;
            pos.z = graphView.resolvedStyle.translate.z;
            graphView.UpdateViewTransform(pos, Vector3.one);

            //Load Boxes
            foreach(var box in gm.boxes)
            {
                graphView.AddBox(new GitBoxGraphNode(box));
            }

            //Load Connections/Edges
            foreach(var conn in gm.connections)
            {
                var parentNode = graphView.GetGraphNode(conn.parent);
                var childNode = graphView.GetGraphNode(conn.child);

                var edge = parentNode.outputPorts[conn.parentOutput].ConnectTo(childNode.inputPorts[conn.childInput]);
                graphView.AddElement(edge);
            }
        }

        private void Save()
        {
            //Sync GitGraphView nodes and content to GitManager
        }

        private void Update()
        {
            if (gm == null != nullGm)
            {
                nullGm = gm == null;
                Refresh();
            }
        }
    }
}
#endif