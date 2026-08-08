#if UNITY_EDITOR
using System;
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    [Serializable]
    public struct GizObstacleReference
    {
        public bool referenceInScene;
        public string gizObstacle;
        public GizObstacle objectReference;
    }
}
#endif