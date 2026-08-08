#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class Techno : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Active", "Special Button Down", "Got Handle" };

        public string controlType;
        public byte unknown1;
        public string controlledEntity;
        public byte unknown2;
        public float cameraEmphasisAmount;
        public int unknown3;
        public float unknown4;

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