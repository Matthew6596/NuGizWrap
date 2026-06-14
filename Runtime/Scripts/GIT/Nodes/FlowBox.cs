#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace TTModdingKit.GizFlow
{
    public class FlowBox : GitBox
    {
        public override string ID => "FlowBox";

        public List<GitGizmo> gizmos;
        public GitCondition condition;
        public GitAction action;
        public int AiAssistID;

        public FlowBox() : base("New Flow Box") { CreateFlowBox(); }

        public FlowBox(string name) : base(name) { CreateFlowBox(); }

        private void CreateFlowBox()
        {

        }

        public override IEnumerable<string> ContentToLines()
        {
            BasePropsToLines();
            throw new System.NotImplementedException();
        }

        public override void ContentFromLines(IEnumerable<string> linesIen, ref int index)
        {
            BasePropsFromLines(linesIen, ref index);
            throw new System.NotImplementedException();
        }
    }
}
#endif