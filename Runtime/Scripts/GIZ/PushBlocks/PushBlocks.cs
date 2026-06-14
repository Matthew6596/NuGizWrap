#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    using GameScene;

    public class PushBlocks : Gizmo
    {
        public SpecialObjectReference specialObject;
        public float snapRange;
        public bool pushLocation, unknown1, lockZ, lockX, unknown2, unknown3, unknown4, noSlip;
        public SpecialObjectReference[] linkObjects;
        public bool unknown5;
        public float unknown6;
        public bool unknown7, unknown8, unknown9, unknown10;

        private void OnValidate()
        {
            linkObjects ??= new SpecialObjectReference[0];
            if (linkObjects.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many Link Objects", "Maximum of 255 link objects allowed on a single PushBlocks.", "OK");
                linkObjects = linkObjects.Take(255).ToArray();
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