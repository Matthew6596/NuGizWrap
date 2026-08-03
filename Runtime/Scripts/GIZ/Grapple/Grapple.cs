#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    using GameScene;

    public class Grapple : Gizmo
    {
        public enum RopeGrappleType { TanRope=0, WhiteRope=1, GreenRope=2, Vines=3 }
        public enum ZipGrappleType { Default=0, Fast=3 }

        public override string[] GetOutputNames(TTGame game) => new[] { "Active", "Occupied", "Occupied By 2" };

        public float unknown3;
        public bool swingingRope;
        public float length;
        public bool noFreeMovement;
        public SpecialObjectReference specialObject;
        public bool visible;
        public RopeGrappleType ropeType;
        public ZipGrappleType grappleType;
        public BlowupReference blowup;
        public float ropeBrightness;

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