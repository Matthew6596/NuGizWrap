#if UNITY_EDITOR
using UnityEngine;
using System;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class Ledge : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "CanUse", "Occupied" };

        //public enum Type { None = '\0', Empty = 'e', Four = '4', Eight = '8' }

        public byte unknown1;
        public short unknown2, unknown3;
        //public Type type;
        public byte type;
        public string unknown4;
        public Vector3 unknown4Pos;
        public short unknown4Ang;

        private void OnValidate()
        {
            //if (!Enum.IsDefined(typeof(Type), type)) type = Type.None;
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