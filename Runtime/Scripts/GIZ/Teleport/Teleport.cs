#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class Teleport : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => (game) switch { TTGame.TCS => new[] { "Occupied" }, _ => new[] { "Occupied", "Not Occupied" } };

        public string hatchBaseSpecialObject;
        public TeleportHatch hatch1, hatch2;
        public Vector3 unknown4, unknown5;
        public float unknown6, unknown7, unknown10, unknown11;
        public short unknown14;

        public static float GizmoScale = 0.15f, GizmoAlpha = 0.5f;
        public static Color GizmoColor = Color.white;
        private void OnDrawGizmos()
        {
            Color col = GizmoColor;
            col.a = GizmoAlpha;
            Giz.color = col;
            Giz.DrawSphere(hatch1 == null ? unknown4 : hatch1.transform.position, GizmoScale);
            Giz.DrawSphere(hatch2 == null ? unknown5 : hatch2.transform.position, GizmoScale);
        }
    }
}
#endif