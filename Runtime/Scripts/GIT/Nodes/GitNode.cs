#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace TTModdingKit.GizFlow
{
    public interface IGitNode
    {
        public string ID { get; }
        public IEnumerable<string> ContentToLines();
        public void ContentFromLines(IEnumerable<string> lines, ref int index);
    }
}
#endif