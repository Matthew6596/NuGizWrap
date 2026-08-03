#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class TightRope : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Active" };

        public TightRopeKnob startKnob, endKnob;
        public Vector3 unknown3, unknown4;
        public bool alwaysShowStartKnob;

        public static float GizmoScale = 0.15f, GizmoAlpha = 0.5f;
        public static Color GizmoColor = Color.white;
        private void OnDrawGizmos()
        {
            Color col = GizmoColor;
            col.a = GizmoAlpha;
            Giz.color = col;
            Giz.DrawSphere(startKnob == null ? Vector3.zero : startKnob.transform.position, GizmoScale);
            Giz.DrawSphere(endKnob == null ? Vector3.zero : endKnob.transform.position, GizmoScale);
        }
    }
}
#endif