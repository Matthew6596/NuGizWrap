#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Obj = UnityEngine.Object;

namespace NuGizWrap.GizFlow
{
    [Serializable]
    public abstract class GitNode
    {
        //public abstract string ID { get; }

        //protected abstract void Load();
    }

    [Serializable]
    public class GitOptions : GitNode
    {
        public Color backgroundColor, collapseBoxColor, textColor;
        public bool rightClickToDelete;

        //Extra properties (Unity Editor only)
        public Color defaultFlowboxColor=Color.darkSlateGray, customBoxColor=Color.lightGray, defaultEdgeColor=Color.lightCyan;
        public Color defaultConditionFlowboxColor = Color.blueViolet, defaultActionFlowboxColor = Color.orangeRed;
        public GizmoColors flowboxGizmoColors = GizmoColors.Default;

        public GitOptions(Color bgCol, Color cbCol, Color txtCol, bool rcDel = false)
        {
            backgroundColor = bgCol;
            collapseBoxColor = cbCol;
            textColor = txtCol;
            rightClickToDelete = rcDel;
        }

        public GitOptions(GITAssetImporter.TextReader tr, GitManager gm)
        {
            string t;
            while((t = tr.NextToken()) != "}")
            {
                switch (t)
                {
                    case "CameraPos": gm.cameraPosition = tr.ParseVector3(); break;
                    case "BgColour": backgroundColor = tr.ParseColor(); break;
                    case "CbColour": collapseBoxColor = tr.ParseColor(); break;
                    case "txtColour": textColor = tr.ParseColor(); break;
                    case "RClickDel": rightClickToDelete = tr.NextToken() != "0"; break;
                }
            }
        }

        [Serializable]
        public struct GizmoColors
        {
            public Color gizObstacleColor, gizBuilditColor, gizForceColor, gizmoPickupColor;
            public Color panelColor, messageColor, gizTimerColor, gizRandomColor;

            public static GizmoColors Default => new()
            {
                gizObstacleColor = new Color32(0x35, 0x50, 0x3f, 0xff),
                gizBuilditColor = new Color32(0x4d, 0x4a, 0x2e, 0xff),
                gizForceColor = new Color32(0x2e, 0x4f, 0x52, 0xff),
                gizmoPickupColor = new Color32(0x32, 0x4b, 0x62, 0xff),
                panelColor = new Color32(0x44, 0x44, 0x6b, 0xff),
                messageColor = new Color32(0x62, 0x3a, 0x4e, 0xff),
                gizTimerColor = new Color32(0x42, 0x4e, 0x31, 0xff),
                gizRandomColor = new Color32(0x65, 0x3b, 0x3d, 0xff),
            };

            public readonly Color GetGizmoColor(FlowBox.Gizmo.Type type) => (type) switch
            {
                FlowBox.Gizmo.Type.GizObstacle => gizObstacleColor,
                FlowBox.Gizmo.Type.GizBuildit => gizBuilditColor,
                FlowBox.Gizmo.Type.GizForce => gizForceColor,
                FlowBox.Gizmo.Type.GizmoPickup => gizmoPickupColor,
                FlowBox.Gizmo.Type.Panel => panelColor,
                FlowBox.Gizmo.Type.Message => messageColor,
                FlowBox.Gizmo.Type.GizTimer => gizTimerColor,
                FlowBox.Gizmo.Type.GizRandom => gizRandomColor,
                _ => gizObstacleColor,
            };
        }

        public override string ToString()
        {
            Vector3 camPos = GITExporter.gm.cameraPosition;
            float bgr = backgroundColor.r, bgg = backgroundColor.g, bgb = backgroundColor.b;
            float cbr = collapseBoxColor.r, cbg = collapseBoxColor.g, cbb = collapseBoxColor.b;
            float txr = textColor.r, txg = textColor.g, txb = textColor.b;
            return $"GitOptions {{\n\tCameraPos {camPos.x} {camPos.y} {camPos.z}\n\tBgColour {bgr} {bgg} {bgb}\n\tCbColour {cbr} {cbg} {cbb}\n\ttxtColour {txr} {txg} {txb}\n\tRClickDel {(rightClickToDelete ? "1" : "0")}\n}}\n\n";
        }
    }

    [Serializable]
    public abstract class GitBox : GitNode
    {
        public abstract string ID { get; }

        public int boxID;

        public Vector2 position;
        public string name;

        //Extra properties (Unity Editor only)
        public Color boxColor;

        protected void ReadBase(GITAssetImporter.TextReader tr, GitManager gm)
        {
            string t;
            while ((t = tr.NextToken()) != "Name")
            {
                switch (t)
                {
                    case "BoxID": boxID = tr.ParseInt(); break;
                    case "Parent": GITAssetImporter.nodeParents.Add((this, tr.ParseInt(), tr.ParseInt())); break;
                    case "Child": break;
                    case "x": position.x = tr.ParseFloat(); break;
                    case "y": position.y = tr.ParseFloat(); break;
                }
            }
            name = tr.ParseString();
        }

        public abstract GitBox CreateCopy();

        public override string ToString()
        {
            string children = string.Empty;
            foreach (var c in GITExporter.gm.GetChildren(this)) children += $"\n\tChild {c}";

            return $"{ID} {{\n\tBoxID {GITExporter.boxIDs[this]}{GITExporter.gm.GetParentsStr(this)}{children}\n\tx {position.x}\n\ty {position.y}\n\tName \"{name}\"";
        }
    }

    [Serializable]
    public class CollapseBox : GitBox
    {
        public override string ID => "Collapse";

        public bool collapsed;

        public CollapseBox()
        {
            name = "unnamed_collapse";
        }

        public CollapseBox(string name, bool collapsed, GitManager gm=null)
        {
            this.name = name;
            this.collapsed = collapsed;

            if(gm != null) boxColor = gm.options.node.collapseBoxColor;
        }

        public CollapseBox(GITAssetImporter.TextReader tr, GitManager gm)
        {
            ReadBase(tr, gm);
            string lastToken = tr.NextToken();
            collapsed = lastToken == "CollapseHead";
            tr.AdvanceToBracketEnd();
            boxColor = gm.options.node.collapseBoxColor;
        }

        public override GitBox CreateCopy() => new CollapseBox(name, collapsed)
        {
            boxColor = boxColor
        };

        public override string ToString()
        {
            string collapseStr = collapsed ? "\n\tCollapseHead" : string.Empty;
            return $"{base.ToString()}{collapseStr}\n}}\n\n";
        }
    }

    [Serializable]
    public class FlowBox : GitBox
    {
        public override string ID => "FlowBox";

        public Gizmo[] gizmos;

        public bool hasAction;
        public Action action;

        public bool hasCondition;
        public Condition condition;

        public bool hasAssistID;
        public int AIAssistID;

        public FlowBox()
        {
            name = "unnamed_flowbox";
            position = Vector2.zero;
            gizmos = new Gizmo[0];
            hasAction = false;
            hasCondition = false;
            hasAssistID = false;
        }

        public FlowBox(string name, Gizmo[] gizmos, bool hasAction, bool hasCondition, bool hasAssistID, Action action, Condition condition, int assistID, GitManager gm=null)
        {
            this.name = name;
            this.gizmos = gizmos;
            this.hasAction = hasAction;
            this.hasCondition = hasCondition;
            this.hasAssistID = hasAssistID;
            this.action = action;
            this.condition = condition;
            AIAssistID = assistID;

            if(gm != null)
            {
                boxColor = gm.options.node.defaultFlowboxColor;
                if (hasAction) boxColor = gm.options.node.defaultActionFlowboxColor;
                else if (hasCondition) boxColor = gm.options.node.defaultConditionFlowboxColor;
                else if (gizmos.Length > 0) boxColor = gm.options.node.flowboxGizmoColors.GetGizmoColor(gizmos[0].type);
            }
        }

        public FlowBox(GITAssetImporter.TextReader tr, GitManager gm)
        {
            boxColor = gm.options.node.defaultFlowboxColor;

            string[] GetList()
            {
                var list = (from str in tr.text[tr.index..tr.NextIndexOf('}')].Trim().Split('\n') select str.Trim()).ToArray();
                if (list[0] == string.Empty) return new string[0];
                return list;
            }

            ReadBase(tr, gm);

            gizmos = new Gizmo[0];
            int numGizmos = 0;
            int gizmoCount = 0;

            string t;
            while ((t = tr.NextToken()) != "}")
            {
                switch (t)
                {
                    case "Num_Gizmos": numGizmos = tr.ParseInt(); gizmos = new Gizmo[numGizmos]; break;
                    case "Gizmo": 
                        tr.index = tr.NextIndexOf('{') + 1;
                        if (tr.NextToken() != "Type") { tr.AdvanceToBracketEnd(); break; }

                        string gizTypeStr = tr.ParseString();
                        string gizTypeToken = tr.NextToken();
                        if (!Enum.TryParse<Gizmo.Type>(gizTypeStr, out var gizmoType))
                        {
                            if (!Enum.TryParse(gizTypeToken, out gizmoType))
                            {
                                tr.AdvanceToBracketEnd();
                                break;
                            }
                        }

                        if (tr.NextToken() != "Name") { tr.AdvanceToBracketEnd(); break; }
                        string gizName = tr.ParseString();
                        tr.NextToken();

                        boxColor = gm.options.node.flowboxGizmoColors.GetGizmoColor(gizmoType);

                        gizmos[gizmoCount] = new()
                        {
                            name = gizName,
                            type = gizmoType,
                            tempGizmoLines = GetList()
                        };

                        tr.AdvanceToBracketEnd();
                        gizmoCount++;
                        break;
                    case "Action": 
                        tr.index = tr.NextIndexOf('{') + 1;
                        hasAction = true;
                        boxColor = gm.options.node.defaultActionFlowboxColor;
                        action = new() { tempActionLines = GetList() };
                        break;
                    case "Condition": 
                        tr.index = tr.NextIndexOf('{') + 1;
                        hasCondition = true;
                        boxColor = gm.options.node.defaultConditionFlowboxColor;
                        if (tr.NextToken() != "Type") break;

                        string conditionTypeStr = tr.NextToken();
                        if (!Enum.TryParse<Condition.Type>(conditionTypeStr, out var conditionType)) break;

                        int conditionNum = 0;
                        if (conditionType == Condition.Type.Exactly || conditionType == Condition.Type.Sum) conditionNum = tr.ParseInt();

                        bool monitorInps = tr.NextToken() == "MonitorInputs";
                        if (monitorInps) tr.AdvanceToBracketEnd();

                        condition = new()
                        {
                            type = conditionType,
                            numberValue = conditionNum,
                            monitorInputs = monitorInps
                        };
                        break;
                    case "AiAssistID": hasAssistID = true; AIAssistID = tr.ParseInt(); break;
                    default: break;
                }
            }

            if (numGizmos != gizmoCount) Debug.LogWarning($"Num_Gizmos ({numGizmos}) and actual Gizmo count ({gizmoCount}) did not match onFlowBox '{name}' ID: {boxID}");
        }

        public override GitBox CreateCopy() => new FlowBox(name, gizmos, hasAction, hasCondition, hasAssistID, action, condition, AIAssistID)
        {
            boxColor = boxColor
        };

        public override string ToString()
        {
            string numGizStr = gizmos.Length > 0 ? $"\n\tNum_Gizmos {gizmos.Length}" : string.Empty;
            string gizStr = string.Empty;
            foreach (var g in gizmos) gizStr += g.ToString();
            string actionStr = hasAction ? action.ToString() : string.Empty;
            string conditionStr = hasCondition ? condition.ToString() : string.Empty;
            string aiAssistStr = hasAssistID ? $"\n\tAiAssistID {AIAssistID}" : string.Empty;
            return $"{base.ToString()}{numGizStr}{gizStr}{actionStr}{conditionStr}{aiAssistStr}\n}}\n\n";
        }

        public string[] GetOutputs()
        {
            List<string> outputs = new();
            foreach (var giz in gizmos) outputs.AddRange(giz.GetOutputs());

            if (outputs.Count == 0) outputs.Add("Output");
            return outputs.ToArray();
        }

        [Serializable]
        public struct Gizmo
        {
            public enum Type { GizObstacle, GizBuildit, GizForce, GizmoPickup, Panel, Message, GizTimer, GizRandom }

            public Type type;
            public string name;
            public string[] tempGizmoLines;

            public readonly override string ToString()
            {
                string typeStr = type == Type.GizTimer ? $"{type}" : $"\"{type}\"";
                string linesStr = string.Empty;
                foreach (var tmp in tempGizmoLines) linesStr += $"\t\t{tmp}\n";
                return $"\n\tGizmo {{\n\t\tType {typeStr}\n\t\tName \"{name}\"\n{linesStr}\t}}";
            }

            public readonly Gizmos.Gizmo GetConnectedGizmo()
            {
                string name = this.name;
                var gizmos = Obj.FindObjectsByType(type switch
                {
                    Type.GizObstacle => typeof(Gizmos.GizObstacle),
                    Type.GizBuildit => typeof(Gizmos.GizBuildit),
                    Type.GizForce => typeof(Gizmos.GizForce),
                    Type.GizmoPickup => typeof(Gizmos.GizmoPickup),
                    Type.Panel => typeof(Gizmos.Panel),
                    Type.Message => typeof(AI.AIMessage),
                    Type.GizTimer => typeof(GizTimer),
                    Type.GizRandom => typeof(GizRandom),
                    _ => typeof(Gizmos.Gizmo)
                }, FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
                return gizmos.Where(g => g.name == name).FirstOrDefault() as Gizmos.Gizmo;
            }

            public readonly IEnumerable<string> GetOutputs()
            {
                var giz = GetConnectedGizmo();
                if(giz == null)
                {
                    Debug.LogWarning($"Cannot find the '{name}' {type} gizmo in the currently open scene, the available outputs may be incorrect.");
                    return new string[] { "Output" };
                }
                return giz.GetOutputNames(TTUnityProject.Game);
            }
        }

        [Serializable]
        public struct Condition
        {
            public enum Type { None, All, Loop, Any, Exactly, Sum }
            public Type type;
            public int numberValue;
            public bool monitorInputs;

            public readonly bool HasNumberValue => type == Type.Exactly || type == Type.Sum;

            public readonly override string ToString()
            {
                string numVal = HasNumberValue ? numberValue.ToString() : string.Empty;
                string monitorInpVal = monitorInputs ? "\n\t\tMonitorInputs" : string.Empty;
                return $"\n\tCondition {{\n\t\tType {type}{numVal}{monitorInpVal}\n\t}}";
            }
        }

        [Serializable]
        public struct Action
        {
            public string[] tempActionLines;

            public readonly override string ToString()
            {
                string str = "\n\tAction {\n";
                foreach(var tmp in tempActionLines) str += $"\t\t{tmp}\n";
                return str + "\t}";
            }
        }
    }

    [Serializable]
    public class CustomGitBox : GitBox
    {
        public override string ID => "CUSTOM-NO-ID";

        public InputMap[] inputs;
        public OutputMap[] outputs;
        [SerializeReference] public GitBox[] nodes;
        public Connection[] connections;

        public CustomGitBox()
        {
            name = "unnamed_custombox";
            inputs = new InputMap[0];
            outputs = new OutputMap[0];
            nodes = new GitBox[0];
            connections = new Connection[0];
        }

        public CustomGitBox(string name, Vector2 position, GitBox[] nodes, Connection[] connections, InputMap[] inputs, OutputMap[] outputs)
        {
            this.name = name;
            this.position = position;
            this.nodes = nodes;
            this.connections = connections;
            this.inputs = inputs;
            this.outputs = outputs;
        }

        public override GitBox CreateCopy()
        {
            var newNodes = nodes.Select(n => n.CreateCopy());
            List<Connection> newConns = new();
            foreach(var conn in connections)
            {
                newConns.Add(new Connection()
                {
                    child = newNodes.Where(n=>object.Equals(n,conn.child)).FirstOrDefault(),
                    parent = newNodes.Where(n=>object.Equals(n,conn.parent)).FirstOrDefault(),
                    parentOutput = conn.parentOutput,
                    childInput = conn.childInput,
                });
            }
            return new CustomGitBox(name, position, newNodes.ToArray(), newConns.ToArray(), inputs, outputs);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            foreach (var n in nodes)
            {
                if (n == null) continue;
                sb.Append(n.ToString());
            }
            return sb.ToString();
        }

        [Serializable]
        public struct InputMap
        {
            public string name;
            public int nodeIndex;
            public int inputPortIndex;
        }

        [Serializable]
        public struct OutputMap
        {
            public string name;
            public int nodeIndex;
            public int outputPortIndex;
        }
    }
}
#endif