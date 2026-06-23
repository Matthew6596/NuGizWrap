#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;

namespace TTModdingKit.GizFlow
{
    using Gizmos;

    public class FlowBox : GitBox
    {
        public override string ID => "FlowBox";

        public List<GitGizmo> gizmos = new();
        public GitCondition condition;
        public GitAction action;
        public int AiAssistID;

        public FlowBox() : base("New Flow Box") { CreateFlowBox(); }

        public FlowBox(string name) : base(name) { CreateFlowBox(); }

        private void CreateFlowBox()
        {
            TTGame game = TTUnityProject.Game;

            //Property Fields
            rootVisualElement.Add(new Label("----- Condition -----"));

            if (condition == null)
            {
                var addConditionBtn = new Button(() =>
                {
                    condition = new();
                    RefreshFlowBoxElements();
                })
                { text = "Add Condition" };
                rootVisualElement.Add(addConditionBtn);
            }
            else
            {
                var conditionBox = new VisualElement();
                var delConditionBtn = new Button(() =>
                {
                    condition = null;
                    RemoveOutputPort(GetOutputPort("condition_output"));
                    RefreshFlowBoxElements();
                })
                { text = "Remove Condition" };
                conditionBox.Add(delConditionBtn);

                var conditionType = new DropdownField("Condition Type", new List<string>() { "None", "All", "Any", "Loop" }, (int)condition.type);
                conditionType.RegisterValueChangedCallback((e) => { condition.type = System.Enum.Parse<GitCondition.Type>(e.newValue); });
                var monitorInps = new Toggle("Monitor Inputs") { value = condition.monitorInputs };
                monitorInps.RegisterValueChangedCallback((e) => { condition.monitorInputs = e.newValue; });

                conditionBox.Add(conditionType);
                conditionBox.Add(monitorInps);
                rootVisualElement.Add(conditionBox);
            }

            rootVisualElement.Add(new Label("----- Action -----"));

            if (action == null)
            {
                var addActionBtn = new Button(() =>
                {
                    action = new();
                    RefreshFlowBoxElements();
                })
                { text = "Add Action" };
                rootVisualElement.Add(addActionBtn);
            }
            else
            {
                var actionBox = new VisualElement();

                var delActionBtn = new Button(() =>
                {
                    action = null;
                    RemoveOutputPort(GetOutputPort("action_output"));
                    RefreshFlowBoxElements();
                })
                { text = "Remove Action" };
                actionBox.Add(delActionBtn);

                //Add action dropdown/btn
                VisualElement addAct = new();
                addAct.style.flexDirection = FlexDirection.Row;
                var actDrop = GitAction.GetActionDropdown(game);
                addAct.Add(actDrop);

                var addActDropBtn = new Button(() =>
                {
                    var prop = GitAction.GetValidProperties(game).Where(p=>p.name==actDrop.value).FirstOrDefault();
                    if (prop == null) return;
                    action.properties.Add(prop);
                    RefreshFlowBoxElements();
                })
                { text = "+" };
                addAct.Add(addActDropBtn);
                actionBox.Add(addAct);

                //Actions
                var props = action.properties;
                for(int i=0; i<props.Count; i++)
                {
                    int ind = i;
                    VisualElement propGroup = new();
                    propGroup.style.flexDirection = FlexDirection.Row;

                    propGroup.Add(props[ind].RootVisualElement);
                    propGroup.Add(new Button(() =>
                    {
                        action.properties.RemoveAt(ind);
                        RefreshFlowBoxElements();
                    }){ text = "-"});

                    actionBox.Add(propGroup);
                }

                rootVisualElement.Add(actionBox);
            }

            rootVisualElement.Add(new Label("----- Gizmos -----"));

            var addGizBtn = new Button(() =>
            {
                gizmos.Add(null);
                RefreshFlowBoxElements();
            }) 
            { text = "Add Gizmo" };
            rootVisualElement.Add(addGizBtn);

            for(int i=0; i<gizmos.Count; i++)
            {
                int ind = i;
                var giz = gizmos[i];
                var gizInp = new ObjectField($"Gizmo {ind}") { objectType = typeof(Gizmo), value = giz };

                var gizDelBtn = new Button(() =>
                {
                    gizmos.RemoveAt(ind);
                    if(giz == null)
                    {
                        RefreshFlowBoxElements();
                        return;
                    }
                    var ports = OutputPorts.Where(p => p.name.StartsWith(giz.name+"_"));
                    if (ports.Any())
                    {
                        var portsArr = ports.ToArray();
                        for (int j = portsArr.Length-1; j >= 0; j--) RemoveOutputPort(portsArr[j]);
                    }
                    RefreshFlowBoxElements();
                })
                { text = $"Remove Gizmo {ind}" };
                rootVisualElement.Add(gizDelBtn);

                gizInp.RegisterValueChangedCallback((e) => 
                { 
                    if (e.newValue is Gizmo g && g != null)
                    {
                        if (g.TryGetComponent(out GitGizmo gitGiz)) gizmos[ind] = gitGiz;
                        else gizmos[ind] = g.gameObject.AddComponent<GitGizmo>();
                    }
                    else if (e.newValue == null) { gizmos[ind] = null; }
                    RefreshFlowBoxElements();
                });
                rootVisualElement.Add(gizInp);
            }

            //Ai Assist ID
            var aiIdField = new IntegerField("Ai Assist ID") { value = AiAssistID };
            aiIdField.RegisterValueChangedCallback((e) => { AiAssistID = e.newValue; });
            rootVisualElement.Add(aiIdField);

            //Output Ports
            //ClearOutputPorts();

            if (condition != null) AddOutputPort("condition_output","Condition Output", Color.red);
            if (action != null) AddOutputPort("action_output","Action Output", Color.orange);

            foreach(var giz in gizmos)
            {
                if (giz == null) continue;
                var gizmo = giz.connectedGizmo;
                if (gizmo == null) continue;

                //outputContainer.Add(new Label($"Gizmo '{gizmo.name}' Outputs"));
                foreach(var output in gizmo.GetOutputNames(game))
                {
                    AddOutputPort($"{gizmo.name}_{output}_output",output, gizmo.MainColor);
                }
            }

            //RefreshPortConnections();
        }

        public void RefreshFlowBoxElements()
        {
            RefreshVisualElements();
            CreateFlowBox();
        }

        public override IEnumerable<string> ContentToLines()
        {
            BasePropsToLines();
            throw new System.NotImplementedException();
        }

        public override void ContentFromLines(IEnumerable<string> linesIen, ref int index)
        {
            BasePropsFromLines(linesIen, ref index);

            var lines = linesIen.ToArray();
            while(index < lines.Length && !lines[index].Contains('}'))
            {
                string line = lines[index].Trim();
                if (line == string.Empty) goto Continue;

                char subStrChar = line.Contains('{') ? '{' : ' ';
                int subInd = line.IndexOf(subStrChar);
                string propName = subInd == -1 ? line : line[..subInd].Trim();

                switch (propName.ToLower())
                {
                    case "action":
                        action = new();
                        index++;
                        action.Load(lines, ref index);
                        break;
                    case "condition":
                        index++;
                        condition = new();
                        if (lines[index].ToLower().Contains("monitorinputs"))
                        {
                            condition.monitorInputs = true;
                            index++;
                            string conTypeLine = lines[index].Trim();
                            int conTypeSpaceInd = conTypeLine.IndexOf(' ')+1;
                            condition.type = Enum.Parse<GitCondition.Type>(conTypeLine[conTypeSpaceInd..]);
                            index++;
                        }
                        else
                        {
                            string conTypeLine = lines[index].Trim();
                            int conTypeSpaceInd = conTypeLine.IndexOf(' ') + 1;
                            condition.type = Enum.Parse<GitCondition.Type>(conTypeLine[conTypeSpaceInd..]);
                            index++;
                            if (lines[index].Contains('}')) condition.monitorInputs = false;
                            else { condition.monitorInputs = true; index++; }
                        }
                        break;
                    case "gizmo":
                        gizmos.Add(GitGizmo.Load(lines, ref index));
                        break;
                    case "num_gizmos": break;
                    case "aiassistid":
                        AiAssistID = int.TryParse(line[(subInd + 1)..], out int aiid) ? aiid : -1;
                        break;
                    default: Debug.Log("Unknown FlowBox Property: " + line); break;
                }

                Continue:
                index++;
            }

            index++;
        }
    }
}
#endif