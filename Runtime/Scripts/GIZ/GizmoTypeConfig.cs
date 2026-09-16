#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    [CreateAssetMenu(fileName = "GizmoTypeConfig", menuName = "Scriptable Objects/GizmoTypeConfig")]
    public abstract class GizmoTypeConfig : ScriptableObject, IGameCompatible
    {
        public abstract bool IsGameCompatible(TTGame game);
        public abstract int GetMaxVersion(TTGame game);
        public abstract int Load(BinaryReader br, Transform parent);
        public abstract void Save(BinaryWriter bw);
    }
}
#endif