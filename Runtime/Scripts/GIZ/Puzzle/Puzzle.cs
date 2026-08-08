#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class Puzzle : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Solved" };

        public static float GizmoAlpha = 0.5f;

        public float unknown1;
        public bool unknown2;
        public Vector3 characterFacingPosition, targetPosition;

        private void OnDrawGizmos()
        {
            Color col = Color.saddleBrown;
            col.a = GizmoAlpha;
            Giz.color = col;

            Giz.matrix = Matrix4x4.TRS(transform.position, transform.localRotation, transform.localScale);
            Giz.DrawCube(Vector3.zero, new Vector3(.5f,.5f,.05f));

            Giz.color = new(1, 0, 0, GizmoAlpha);
            Giz.DrawLine(Vector3.zero, Vector3.back * 0.1f);
        }
    }
}
#endif