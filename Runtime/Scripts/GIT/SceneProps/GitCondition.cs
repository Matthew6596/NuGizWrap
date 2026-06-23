#if UNITY_EDITOR
using UnityEngine;

namespace TTModdingKit.GizFlow
{
    public class GitCondition
    {
        public enum Type { None=0, All=1, Any=2, Loop=3 }
        public Type type;
        public bool monitorInputs;
    }
}
#endif