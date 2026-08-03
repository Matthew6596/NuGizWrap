#if UNITY_EDITOR
using UnityEngine;
using System;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    using GameScene;

    public class Ledge : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "CanUse", "Occupied" };

        public enum Type { One = '1', Two = '2', Three = '3', Four = '4', Eight = '8', Inner = 'i', Outer = 'o', End = 'e' }

        public Type type;
        public Ledge leftLedge, rightLedge;
        public byte interactOptions;
        public SpecialObjectReference specialObject;
        public Vector3 specialObjectPos;
        public short specialObjectAng;

        private void OnValidate()
        {
            if (!Enum.IsDefined(typeof(Type), type)) type = Type.Two;
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