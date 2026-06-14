#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    using GameScene;

    public class Grapple : Gizmo
    {
        public short unknown1, unknown2;
        public float unknown3;
        public bool unknown4;
        public float length;
        public short unknown6;
        public bool unknown7;
        public SpecialObjectReference specialObject;
        public bool unknown8, unknown9;
        public BlowupReference blowup;
        public bool unknown10;

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