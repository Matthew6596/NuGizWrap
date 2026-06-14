#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class ShadowEditor : Gizmo
    {
        public float unknown1;
        public float unknown2, unknown3;
        public float unknown4, unknown5;
        public float unknown6;
        public float unknown7, unknown8;
        public float unknown9, unknown10, unknown11;
        public float unknown12;
        public int unknown13;
        public float unknown14;

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