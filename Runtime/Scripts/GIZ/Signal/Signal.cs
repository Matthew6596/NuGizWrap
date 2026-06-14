#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class Signal : Gizmo
    {
        public enum Character { Batman=(byte)'b', Robin=(byte)'r' }
        public enum Suit { 
            BatmanDefault= (byte)'b', RobinDefault= (byte)'r',
            Demolition = (byte)'c', Techno=(byte)'t',
            Glide = (byte)'a', Magnetic = (byte)'m',
            Sonic = (byte)'i', Attract = (byte)'d',
            Heatsuit = (byte)'h', Water = (byte)'w',
        }

        public Character character;
        public Suit suit;
        public short unknown2;
        public Vector3 unknown3;
        public string unknown4;
        public string[] unknown5;

        private void OnValidate()
        {
            if (!Enum.IsDefined(typeof(Character), character)) character = Character.Batman;
            if (!Enum.IsDefined(typeof(Suit), suit)) suit = character == Character.Batman ? Suit.BatmanDefault : Suit.RobinDefault;

            unknown5 ??= new string[0];
            if (unknown5.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many strings", "Maximum of 255 unknown5 strings allowed on Signal.", "OK");
                unknown5 = unknown5.Take(255).ToArray();
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