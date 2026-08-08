#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;

namespace NuGizWrap.GizFlow
{
    using Gizmos;
    using Helper;

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

                var conditionType = new DropdownField("Condition Type", new List<string>() { "None", "All", "Any", "Loop", "Exactly", "Sum" }, (int)condition.type);

                var exactlyInpAmt = new IntegerField("Input Amount") { value = condition.inputAmount };
                exactlyInpAmt.SetVisible(condition.type == GitCondition.Type.Exactly || condition.type == GitCondition.Type.Sum);

                conditionType.RegisterValueChangedCallback((e) => { 
                    condition.type = Enum.Parse<GitCondition.Type>(e.newValue);
                    exactlyInpAmt.SetVisible(condition.type == GitCondition.Type.Exactly || condition.type == GitCondition.Type.Sum);
                });

                var monitorInps = new Toggle("Monitor Inputs") { value = condition.monitorInputs };
                monitorInps.RegisterValueChangedCallback((e) => { condition.monitorInputs = e.newValue; });

                conditionBox.Add(conditionType, exactlyInpAmt, monitorInps);
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
            var lines = BasePropsToLines().ToList();
            if (gizmos.Count > 0)
            {
                lines.Add($"\tNum_Gizmos {gizmos.Count}");
                foreach (var giz in gizmos)
                {
                    lines.Add("\tGizmo {");
                    lines.Add($"\t\tType \"{giz.connectedGizmoType}\"");
                    lines.Add($"\t\tName \"{(giz.connectedGizmo is PushBlocks p ? p.specialObject.specialObject : giz.connectedGizmoName)}\"");
                    lines.Add("\t}");
                }

                if (condition != null)
                {
                    lines.Add("\tCondition {");
                    bool hasInputAmount = condition.type == GitCondition.Type.Exactly || condition.type == GitCondition.Type.Sum;
                    lines.Add($"\t\tType {condition.type}{(hasInputAmount ? " " + condition.inputAmount : "")}");
                    if (condition.monitorInputs) lines.Add("\t\tMonitorInputs");
                    lines.Add("\t}");
                }

                if (action != null)
                {
                    lines.Add("\tAction {");
                    foreach (var act in action.properties) lines.Add($"\t\t{act.ToLine()}");
                    lines.Add("\t}");
                }

                if (AiAssistID != -1) lines.Add($"\tAiAssistID {AiAssistID}");
            }

            return lines;
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
                        bool conContainsMonitorInps = lines[index].ToLower().Contains("monitorinputs");
                        if (conContainsMonitorInps)
                        {
                            condition.monitorInputs = true;
                            index++;
                        }
                        //Parse Condition Type
                        string conTypeLine = lines[index].Trim();
                        int conTypeSpaceInd = conTypeLine.IndexOf(' ') + 1;
                        string conTypeStr = conTypeLine[conTypeSpaceInd..];
                        bool isExactlyCon = conTypeStr.Contains("Exactly");
                        if (isExactlyCon || conTypeStr.Contains("Sum"))
                        {
                            int conTypeSpaceInd2 = conTypeStr.IndexOf(' ') + 1;
                            condition.type = isExactlyCon ? GitCondition.Type.Exactly : GitCondition.Type.Sum;
                            condition.inputAmount = int.Parse(conTypeStr[conTypeSpaceInd2..]);
                        }
                        else condition.type = Enum.Parse<GitCondition.Type>(conTypeLine[conTypeSpaceInd..]);
                        index++;
                        //
                        if (!conContainsMonitorInps)
                        {
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