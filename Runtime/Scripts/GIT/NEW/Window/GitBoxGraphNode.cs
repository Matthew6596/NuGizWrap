#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NuGizWrap.GizFlow
{
    using Helper;

    public class GitBoxGraphNode : Node
    {
        public GitBox box;

        public Vector2 Position => new(style.left.value.value, style.top.value.value);

        public VisualElement contentBox;

        //Collapse Box
        private bool _isCollapsed = false;
        public bool IsCollapsed
        {
            get => _isCollapsed; set
            {
                _isCollapsed = value;
                //Set Collapse State
            }
        }

        //Flow Box
        public enum FlowBoxType { Action, Condition, Gizmo, Any }
        public FlowBoxType flowType = FlowBoxType.Any;

        public bool flowHasAction;
        public FlowBox.Action flowAction;
        public bool flowHasCondition;
        public FlowBox.Condition flowCondition;
        public bool flowHasGizmos;
        public FlowBox.Gizmo[] flowGizmos;

        public bool flowHasAssistID;
        public int flowAssistID;

        //Custom Box


        public GitBoxGraphNode(GitBox box)
        {
            this.box = box;

            //Insert content box between the title and the ports
            contentBox = new Box();
            mainContainer.Insert(1, contentBox);

            RefreshBox();
        }

        public void RefreshBox()
        {
            title = box.name;

            SetPosition(new Rect(box.position, new Vector2(150, 200)));

            var color = box.boxColor;
            color.a = 1;
            style.backgroundColor = color;

            RefreshBoxContent();
            RefreshBoxPorts();
            RefreshExpandedState();
        }

        public void RefreshBoxContent()
        {
            contentBox.Clear();
            titleButtonContainer.Clear();

            //Collapse Button
            if (box is CollapseBox c)
            {
                var collapseBtn = new Button(() =>
                {
                    IsCollapsed = !IsCollapsed;
                    Debug.Log("collapsed state: " + IsCollapsed);
                });
                collapseBtn.Add(new Label("Collapse"));
                IsCollapsed = c.collapsed;
                titleButtonContainer.Add(collapseBtn);
            }
            else if(box is FlowBox f) CreateFlowBoxContent(f);
            else if(box is CustomGitBox custom) CreateCustomBoxContent(custom);
        }

        private void CreateFlowBoxContent(FlowBox flowbox)
        {
            //Load flowbox data (action, condition, gizmos)
            flowHasAction = flowbox.hasAction;
            flowHasCondition = flowbox.hasCondition;
            flowHasGizmos = flowbox.gizmos.Length > 0;
            flowHasAssistID = flowbox.hasAssistID;
            flowAssistID = flowbox.AIAssistID;

            flowType = FlowBoxType.Any;
            if (flowHasAction)
            {
                flowAction = flowbox.action;
                if (!flowHasCondition && !flowHasGizmos) flowType = FlowBoxType.Action;
            }
            if (flowHasCondition)
            {
                flowCondition = flowbox.condition;
                if (!flowHasAction && !flowHasGizmos) flowType = FlowBoxType.Condition;
            }
            if (flowHasGizmos)
            {
                int gizCount = flowbox.gizmos.Length;
                flowGizmos = new FlowBox.Gizmo[gizCount];
                Array.Copy(flowbox.gizmos, flowGizmos, gizCount);
                if (!flowHasCondition && !flowHasAction) flowType = FlowBoxType.Gizmo;
            }
            else flowGizmos = new FlowBox.Gizmo[0];

            void CreateActionContent()
            {
                //Making sure box is cleared
                Box actionBox = new() { name = "actionBox" };
                bool alreadyHasActionBox = false;
                foreach(var child in contentBox.Children())
                {
                    if (actionBox.name == child.name)
                    {
                        actionBox = (Box)child;
                        alreadyHasActionBox = true;
                        actionBox.Clear();
                    }
                }
                if(!alreadyHasActionBox) contentBox.Add(actionBox);

                //Action Lines
                var lines = flowAction.tempActionLines;
                for (int i=0; i<lines.Length; i++)
                {
                    VisualElement lineArea = new();
                    lineArea.style.flexDirection = FlexDirection.Row;
                    actionBox.Add(lineArea);

                    int ind = i;

                    var lineField = new TextField() { value = lines[i] };

                    lineField.RegisterValueChangedCallback((e) => { flowAction.tempActionLines[ind] = e.newValue; });
                    lineArea.Add(lineField);

                    Button lineDelBtn = new(() =>
                    {
                        flowAction.tempActionLines = flowAction.tempActionLines.RemoveAt(ind);
                        CreateActionContent();
                    })
                    { text = "-" };
                    lineArea.Add(lineDelBtn);
                }

                //Add btn
                Button addLineBtn = new(() =>
                {
                    flowAction.tempActionLines = flowAction.tempActionLines.Append("").ToArray();
                    CreateActionContent();
                })
                { text = "Add line" };
                actionBox.Add(addLineBtn);

            }

            void CreateConditionContent()
            {
                Box conditionBox = new();
                contentBox.Add(conditionBox);

                //Prepare horizontal for type+number
                VisualElement typeArea = new();
                typeArea.style.flexDirection = FlexDirection.Row;
                conditionBox.Add(typeArea);

                //Prepare number field
                IntegerField numValField = new() { value = flowCondition.numberValue };

                //Type field
                var conditionTypes = Enum.GetNames(typeof(FlowBox.Condition.Type)).ToList();
                DropdownField typeDropdown = new("Condition Type", conditionTypes, (int)flowCondition.type);
                typeDropdown.RegisterValueChangedCallback((e) =>
                {
                    flowCondition.type = Enum.Parse<FlowBox.Condition.Type>(e.newValue);
                    numValField.SetVisible(flowCondition.HasNumberValue);
                });
                typeArea.Add(typeDropdown);

                //Number field
                numValField.SetVisible(flowCondition.HasNumberValue);
                numValField.RegisterValueChangedCallback(e => { flowCondition.numberValue = e.newValue; });
                typeArea.Add(numValField);

                //Monitor Inputs field
                Toggle monitorInpsField = new("Monitor Inputs") { value = flowCondition.monitorInputs };
                monitorInpsField.RegisterValueChangedCallback(e => flowCondition.monitorInputs = e.newValue);
                conditionBox.Add(monitorInpsField);
            }

            void CreateGizmosContent()
            {
                //Making sure box is cleared
                Box gizmosRoot = new() { name = "gizmosRoot" };
                bool alreadyHasActionBox = false;
                foreach (var child in contentBox.Children())
                {
                    if (gizmosRoot.name == child.name)
                    {
                        gizmosRoot = (Box)child;
                        alreadyHasActionBox = true;
                        gizmosRoot.Clear();
                    }
                }
                if (!alreadyHasActionBox) contentBox.Add(gizmosRoot);
                for(int i=0; i<flowGizmos.Length; i++) CreateGizmoContent(i, gizmosRoot);
            }

            void CreateGizmoContent(int gizIndex, VisualElement gizmosRoot)
            {
                var giz = flowGizmos[gizIndex];
                Box gizBox = new();
                gizmosRoot.Add(gizBox);

                //Gizmo Type Field
                var gizTypes = Enum.GetNames(typeof(FlowBox.Gizmo.Type)).ToList();
                DropdownField typeDropdown = new("Gizmo Type", gizTypes, (int)giz.type);
                typeDropdown.RegisterValueChangedCallback(e =>
                {
                    flowGizmos[gizIndex].type = Enum.Parse<FlowBox.Gizmo.Type>(e.newValue);
                    //TO-DO: refresh output ports
                });
                gizBox.Add(typeDropdown);

                //Gizmo Name Field - TO-DO: ObjectField alternative
                TextField nameField = new("Gizmo Name") { value = giz.name };
                nameField.RegisterValueChangedCallback(e =>
                {
                    flowGizmos[gizIndex].name = e.newValue;
                });
                gizBox.Add(nameField);

                //Gizmo Lines
                var lines = giz.tempGizmoLines;
                for (int i = 0; i < lines.Length; i++)
                {
                    VisualElement lineArea = new();
                    lineArea.style.flexDirection = FlexDirection.Row;
                    gizBox.Add(lineArea);

                    int ind = i;

                    var lineField = new TextField() { value = lines[i] };

                    lineField.RegisterValueChangedCallback((e) => { flowGizmos[gizIndex].tempGizmoLines[ind] = e.newValue; });
                    lineArea.Add(lineField);

                    Button lineDelBtn = new(() =>
                    {
                        flowGizmos[gizIndex].tempGizmoLines = flowGizmos[gizIndex].tempGizmoLines.RemoveAt(ind);
                        CreateGizmosContent();
                    })
                    { text = "-" };
                    lineArea.Add(lineDelBtn);
                }

                //Add btn
                Button addLineBtn = new(() =>
                {
                    flowGizmos[gizIndex].tempGizmoLines = flowGizmos[gizIndex].tempGizmoLines.Append("").ToArray();
                    CreateGizmosContent();
                })
                { text = "Add line" };
                gizBox.Add(addLineBtn);

                //AI Assist horizontal group
                VisualElement aiAssistArea = new();
                aiAssistArea.style.flexDirection = FlexDirection.Row;
                gizBox.Add(aiAssistArea);

                //Assist ID Field
                IntegerField aiAssistField = new() { value = flowAssistID };
                aiAssistField.RegisterValueChangedCallback(e =>
                {
                    flowAssistID = e.newValue;
                });
                aiAssistField.style.minWidth = 32;
                aiAssistField.SetVisible(flowHasAssistID);

                //AI Assist Toggle
                Toggle aiAssistToggle = new("AI Assist ID") { value = flowHasAssistID };
                aiAssistToggle.RegisterValueChangedCallback(e =>
                {
                    flowHasAssistID = e.newValue;
                    aiAssistField.SetVisible(flowHasAssistID);
                });

                aiAssistArea.Add(aiAssistToggle, aiAssistField);
            }

            //Create flowbox content
            switch(flowType)
            {
                case FlowBoxType.Action: CreateActionContent(); break;
                case FlowBoxType.Condition: CreateConditionContent(); break;
                case FlowBoxType.Gizmo: CreateGizmosContent(); break;
                default:
                    if (flowHasAction) CreateActionContent();
                    if (flowHasCondition) CreateConditionContent();
                    if (flowHasGizmos) CreateGizmosContent();
                    break;
            }
        }

        private void CreateCustomBoxContent(CustomGitBox custom)
        {
            CustomGitNodeAsset nodeAsset = null;
            var guids = AssetDatabase.FindAssets("t:CustomGitNodeAsset", null);
            foreach(var guid in guids)
            {
                nodeAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(CustomGitNodeAsset)) as CustomGitNodeAsset;
                if (nodeAsset != null && nodeAsset.node == custom) break;
            }
            Button editBtn = new(() => {  }) { text = "Edit Node" };
            ObjectField assetField = new("Custom Node Asset")
            {
                allowSceneObjects = false,
                objectType = typeof(CustomGitNodeAsset),
                value = nodeAsset
            };
            contentBox.Add(editBtn);
            contentBox.Add(assetField);
        }

        public void RefreshBoxPorts()
        {
            inputContainer.Clear();
            outputContainer.Clear();

            if (box is CustomGitBox custom)
            {
                CreateCustomPorts(custom);
            }
            else
            {
                //Collapse and FlowBox only need 1 input port
                CreatePort(Direction.Input, "Input");

                if (box is FlowBox flowbox) CreateFlowboxPorts(flowbox);
                else CreatePort(Direction.Output, "Output"); //Collapse Box
            }
        }

        public void CreatePort(Direction direction, string name)
        {
            var port = InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Multi, null);
            port.name = name;
            port.portName = name;
            (direction == Direction.Input ? inputContainer : outputContainer).Add(port);
        }

        private void CreateFlowboxPorts(FlowBox flowbox)
        {
            foreach (var outpt in flowbox.GetOutputs())
                CreatePort(Direction.Output, outpt);
        }

        private void CreateCustomPorts(CustomGitBox custom)
        {
            foreach (var inp in custom.inputs)
                CreatePort(Direction.Input, inp.name);

            foreach (var outpt in custom.outputs)
                CreatePort(Direction.Output, outpt.name);
        }

        public int GetOutputPortIndex(Port outputPort) => outputContainer.IndexOf(outputPort);
        public int GetInputPortIndex(Port inputPort) => inputContainer.IndexOf(inputPort);

        public void SaveBox()
        {
            SaveBaseBox();
            if (box is FlowBox f) SaveFlowbox(f);
            else if (box is CollapseBox co) SaveCollapseBox(co);
            else if (box is CustomGitBox cu) SaveCustomBox(cu);
            else throw new NotSupportedException($"Saving for {box.GetType().Name} box type is not supported.");
        }

        private void SaveBaseBox()
        {
            box.name = title;
            box.position = Position;
            box.boxColor = style.backgroundColor.value;
        }

        private void SaveCollapseBox(CollapseBox collapse)
        {
            collapse.collapsed = _isCollapsed;
        }

        private void SaveFlowbox(FlowBox flowbox)
        {
            flowbox.hasAction = flowHasAction;
            flowbox.hasCondition = flowHasCondition;
            flowbox.hasAssistID = flowHasAssistID;
            flowbox.AIAssistID = flowAssistID;

            flowbox.action = flowAction;
            flowbox.condition = flowCondition;
            flowbox.gizmos = flowGizmos;
        }

        private void SaveCustomBox(CustomGitBox custom)
        {

        }
    }
}
#endif