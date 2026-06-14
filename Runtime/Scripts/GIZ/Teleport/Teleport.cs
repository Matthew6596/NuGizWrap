#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class Teleport : Gizmo
    {
        public string unknown1, unknown2, unknown3;
        public Vector3 unknown4, unknown5;
        public float unknown6, unknown7, unknown8, unknown9, unknown10, unknown11;
        public short unknown12, unknown13, unknown14;
        public Vector3 unknown15, unknown16;

        public static float GizmoScale = 0.15f, GizmoAlpha = 0.5f;
        public static Color GizmoColor = Color.white;
        private void OnDrawGizmos()
        {
            Color col = GizmoColor;
            col.a = GizmoAlpha;
            Giz.color = col;
            Giz.DrawSphere(unknown4, GizmoScale);
            Giz.DrawSphere(unknown5, GizmoScale);
        }
    }
}
#endif