#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Terrain
{
    public class InfiniteWall : MonoBehaviour
    {
        public List<Vector3> points = new();

        private void OnValidate()
        {
            //y-value don't matter
            for (int i = 0; i < points.Count; i++) points[i] = new(points[i].x,0,points[i].z);
        }

        private void OnDrawGizmos()
        {
            Giz.color = Color.darkCyan;
            for(int i=0; i<points.Count-1; i++) Giz.DrawLine(points[i], points[i + 1]);
        }
    }
}
#endif