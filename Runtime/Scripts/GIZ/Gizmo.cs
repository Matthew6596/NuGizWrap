#if UNITY_EDITOR
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    public abstract class Gizmo : MonoBehaviour
    {
        public virtual Color MainColor => Color.white;
        public virtual string[] GetOutputNames(TTGame game) => new string[0];
    }
}
#endif