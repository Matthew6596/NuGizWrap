#if UNITY_EDITOR
using System;
using System.Text;
using UnityEngine;

namespace NuGizWrap.GizFlow
{
    [CreateAssetMenu(fileName = "GitNodeAsset", menuName = "Scriptable Objects/Git Nodes/Custom")]
    public class CustomGitNodeAsset : GitNodeAsset
    {
        public CustomGitBox node;

        //Regarding custom gitnode reusability:
        //Variables list that can allow varitey per instance, where values are specified in GitManager
        //Variables usage in nodes has $ at start so they can be replaced in each instance

        public override string ToString() => node.ToString();
    }
}
#endif