#if UNITY_EDITOR
using UnityEngine;

namespace NuGizWrap.GizFlow
{
    [CreateAssetMenu(fileName = "GitNodeAsset", menuName = "Scriptable Objects/Git Nodes/GitOptions")]
    public class GitOptionsAsset : GitNodeAsset
    {
        public GitOptions node;

        public override string ToString() => node == null ? string.Empty : node.ToString();

        public static GitOptionsAsset Default 
        { 
            get
            {
                var opts = CreateInstance<GitOptionsAsset>();
                opts.node = new GitOptions(Color.black, new Color32(126, 30, 255, 255), Color.white);
                return opts;
            }
        }
    }
}
#endif