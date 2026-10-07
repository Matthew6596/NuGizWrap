#if UNITY_EDITOR
using UnityEngine;

namespace NuGizWrap.GizFlow
{
    [CreateAssetMenu(fileName = "GitNodeAsset", menuName = "Scriptable Objects/Git Nodes/Collapse")]
    public class CollapseBoxAsset : GitBoxAsset
    {
        public override GitBox Box => node;

        public CollapseBox node;
    }
}
#endif