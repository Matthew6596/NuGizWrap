#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class Whipper : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Done" };

        public float platformDistance = 0.2f;
        public bool vertical, hasSupportBeams;
        public float unknown4;
        public GizObstacleReference gizObstacle;

        public static float GizmoScale = 0.25f, GizmoAlpha = 0.5f;
        public static Color GizmoColor = Color.white;
        private void OnDrawGizmos()
        {
            Color col = GizmoColor;
            col.a = GizmoAlpha;
            Giz.color = col;
            Giz.DrawSphere(transform.position, GizmoScale);
        }
    }
}
#endif