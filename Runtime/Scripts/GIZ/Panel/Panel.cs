#if UNITY_EDITOR
using UnityEngine;
using System;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class Panel : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Finished" };

        public enum Type { AstromechDroid = 0, ProtocolDroid = 1, BountyHunter = 2, Stormtrooper = 3 }

        public Type type;
        public bool invisible;
        public Transform target;
        public bool targetInvisible;
        public bool alternativeFace, alternativeBody;
        public bool unknown1, unknown2;

        private void OnValidate()
        {
            if (!Enum.IsDefined(typeof(Type), type)) type = Type.AstromechDroid;
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