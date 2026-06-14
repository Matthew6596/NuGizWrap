#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class ZipUp : Gizmo
    {
        public Transform start, axis, end;
        public short unknown1, unknown2;
        public bool swing, unknown3, twoWay, invisible, unknown4, targetsInvisible, unknown5;
        public byte unknown6, unknown7;

        public static float GizmoScale = 0.15f, GizmoAlpha = 0.5f;
        public static Color GizmoColor = Color.white;
        private void OnDrawGizmos()
        {
            Color col = GizmoColor;
            col.a = GizmoAlpha;
            Giz.color = col;
            Giz.DrawSphere(start.transform.position, GizmoScale);
            Giz.DrawSphere(axis.transform.position, GizmoScale);
            Giz.DrawSphere(end.transform.position, GizmoScale);
        }
    }
}
#endif