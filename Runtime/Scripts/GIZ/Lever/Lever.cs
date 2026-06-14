#if UNITY_EDITOR
using UnityEngine;
using System;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class Lever : Gizmo
    {
        public enum HandleColor
        {
            None = 0, Red = (byte)'r', Orange = (byte)'o', Yellow = (byte)'y',
            Lime = (byte)'l', Green = (byte)'g', LightBlue = (byte)'u',
            Blue = (byte)'b', Purple = (byte)'p', Brown = (byte)'w'
        }

        public HandleColor handleColor;
        public bool multiplePulls;
        public float pullTime;
        public bool invisible;
        public Transform target;
        public bool targetInvisible;
        public string unknown1;
        public bool unknown2;
        public byte unknown3, unknown4;

        private void OnValidate()
        {
            if (!Enum.IsDefined(typeof(HandleColor), handleColor)) handleColor = HandleColor.Yellow;
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