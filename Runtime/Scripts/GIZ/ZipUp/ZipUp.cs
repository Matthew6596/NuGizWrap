#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class ZipUp : Gizmo
    {
        public enum PlatformStyle { SupportBeams=0, NoSupportBeams=1, Nothing=2 }

        public override string[] GetOutputNames(TTGame game) => (game) switch { TTGame.TCS => new[] { "Active" }, _ => new[] { "Active", "InUse" } };

        public Transform start, hook, end;
        public bool swing, activeForPlayer, twoWay, hookVisible, inactive, targetsVisible, unknown5;
        public PlatformStyle startPlatformStyle, endPlatformStyle;

        public static float GizmoScale = 0.15f, GizmoAlpha = 0.5f;
        public static Color GizmoColor = Color.white;
        private void OnDrawGizmos()
        {
            Color col = GizmoColor;
            col.a = GizmoAlpha;
            Giz.color = col;
            Vector3 startPos = start.position, endPos = end.position;
            if (start != null) Giz.DrawSphere(startPos, GizmoScale);
            if (hook != null) Giz.DrawSphere(hook.position, GizmoScale);
            if (end != null) Giz.DrawSphere(endPos, GizmoScale);

            if (ZipUpSection.Instance != null && ZipUpSection.Instance.version >= 5 && start != null && end != null)
            {
                //face platforms towards each other
                float ang = Mathf.Atan2(-(endPos.z-startPos.z), endPos.x-startPos.x) * Mathf.Rad2Deg;
                start.eulerAngles = new(0, ang + 90, 0);
                end.eulerAngles = new(0, ang + 270, 0);

                Giz.DrawRay(startPos, start.forward);
                Giz.DrawRay(endPos, end.forward);
            }
        }
    }
}
#endif