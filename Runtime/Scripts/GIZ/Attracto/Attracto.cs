#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class Attracto : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Filled" };

        public byte pieceCount;
        public string unknown1;

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