#if UNITY_EDITOR
using UnityEngine;

namespace TTModdingKit.GizFlow
{
    public class GitCondition : MonoBehaviour
    {
        public enum Type { None, All, Any, Loop }
        public Type type;
        public bool monitorInputs;
    }
}
#endif