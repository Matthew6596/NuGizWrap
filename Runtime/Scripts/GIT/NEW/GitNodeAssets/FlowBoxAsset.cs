#if UNITY_EDITOR
using UnityEngine;

namespace NuGizWrap.GizFlow
{
    [CreateAssetMenu(fileName = "GitNodeAsset", menuName = "Scriptable Objects/Git Nodes/FlowBox")]
    public class FlowBoxAsset : GitBoxAsset
    {
        public override GitBox Box => node;

        public FlowBox node;
    }
}
#endif