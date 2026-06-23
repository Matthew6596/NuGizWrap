#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class GizObstacleTrigger : MonoBehaviour
    {
        public float radius;

        private void OnDrawGizmos()
        {
            Color col = Color.green;
            col.a = GizObstacle.GizmoAlpha;
            Giz.color = col;
            Giz.DrawSphere(transform.position, radius);
        }
    }
}
#endif