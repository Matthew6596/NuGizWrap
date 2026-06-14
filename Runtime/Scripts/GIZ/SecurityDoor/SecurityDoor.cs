#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class SecurityDoor : Gizmo
    {
        public static string[] LIJ1Types => new string[] { "Enemy", "Thuggee", "PostBox" };
        public static string[] LB1Types => new string[] { "NoDoor", "LoveHearts" };

        public string type;
        public string unknown1;

        private void OnValidate()
        {
            if (!LIJ1Types.Contains(type) && !LB1Types.Contains(type))
            {
                //Debug.Log("Unknown type: " + type);
                type = TTUnityProject.Game == TTGame.LB1 ? LB1Types[0] : LIJ1Types[0];
            }
        }

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