#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class TightRope : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Active" };

        public Vector3 unknown1, unknown2, unknown3, unknown4;
        public short unknown5, unknown6, unknown7, unknown8;
        public byte unknown9, unknown10, unknown11;

        public static float GizmoScale = 0.15f, GizmoAlpha = 0.5f;
        public static Color GizmoColor = Color.white;
        private void OnDrawGizmos()
        {
            Color col = GizmoColor;
            col.a = GizmoAlpha;
            Giz.color = col;
            Giz.DrawSphere(unknown1, GizmoScale);
            Giz.DrawSphere(unknown2, GizmoScale);
        }
    }
}
#endif