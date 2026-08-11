#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class SecurityDoor : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Opened" };

        public static string[] LIJ1Types => new string[] { "Enemy", "Thuggee", "PostBox" };
        public static string[] LB1Types => new string[] { "NoDoor", "LoveHearts" };

        public static string[] GetTypes(TTGame game) => (game) switch { TTGame.LIJ1 => LIJ1Types, TTGame.LB1 => LB1Types, _ => new string[0] };

        public string type;
        public GameScene.SpecialObjectReference specialObject;

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