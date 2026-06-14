#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    using GameScene;

    public class Spinner : Gizmo
    {
        public SpecialObjectReference specialObject;
        public byte flapCount;
        public int unknown1;
        public float unknown2, unknown3;
        public short unknown4;
        public byte specialObjectVersion;
        public SpecialObject[] animObjects;
        public float[] unknown5;
        public float unknown6, unknown7;

        public string unknown8;
        public float unknown9;
        public int unknown10;

        private void OnValidate()
        {
            animObjects ??= new SpecialObject[0];
            if (animObjects.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many Special Objects", "Maximum of 255 anim objects allowed on a single Spinner.", "OK");
                animObjects = animObjects.Take(255).ToArray();
            }

            unknown5 ??= new float[0];
            if (unknown5.Length > 255)
            {
                EditorUtility.DisplayDialog("Maxed Array", "Maximum of 255 floats allowed in this array.", "OK");
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

        [Serializable]
        public struct SpecialObject
        {
            public SpecialObjectReference specialObject;
            public float unknown1;
            public float animationTime;
            public int unknown2;
        }
    }
}
#endif