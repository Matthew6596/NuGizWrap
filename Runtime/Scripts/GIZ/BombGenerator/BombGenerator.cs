#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    using GameScene;
    using UnityEditor;

    public class BombGenerator : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Active" };

        public int unknown1;
        public float unknown2;
        public byte specialObjectVersion;
        public SpecialObject[] specialObjects;

        private void OnValidate()
        {
            specialObjects ??= new SpecialObject[0];
            if (specialObjects.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many Special Objects", "Maximum of 255 special objects allowed on a single BombGenerator.", "OK");
                specialObjects = specialObjects.Take(255).ToArray();
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

        [Serializable]
        public struct SpecialObject
        {
            public SpecialObjectReference specialObject;
            public float unknown1;
            public float animationTime;
            public int unknown2;
            public short unknown3;
        }
    }
}
#endif