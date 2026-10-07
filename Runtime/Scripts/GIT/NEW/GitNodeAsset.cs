#if UNITY_EDITOR
using UnityEngine;

namespace NuGizWrap.GizFlow
{
    public abstract class GitNodeAsset : ScriptableObject
    {
        public abstract override string ToString();
    }

    public abstract class GitBoxAsset : GitNodeAsset
    {
        public abstract GitBox Box { get; }

        public override string ToString() => Box == null ? string.Empty : Box.ToString();
    }
}
#endif