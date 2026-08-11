#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class Plug : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Plugged", "ID1", "ID2", "ID3", "ID4", "ID5", "NotPlugged" };

        public short validBlowups, unknown2;
        public bool blowupObjectVisible;
        public float unknown5;

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