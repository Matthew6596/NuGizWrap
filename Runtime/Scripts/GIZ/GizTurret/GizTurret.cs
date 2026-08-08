#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    using GameScene;
    using Audio;

    public class GizTurret : Gizmo
    {
        public byte specialObjectVersion;
        public SpecialObject[] specialObjects;
        public Vector3 unknown2, unknown3, unknown4;
        public int unknown5, unknown6, unknown7, unknown8, unknown9, unknown10, unknown11;
        public Vector3[] unknown12;
        public float unknown13, shootRange, unknown15, fireRate, yRotationSpeed, xRotationSpeed;
        public ushort studsValue;
        public Transform studsSpawn;
        public float studsSpawnSpeed;
        public byte unknown19, unknown20;
        public short unknown21;
        public string boltType;
        public SampleReference unknownSfx1, unknownSfx2, unknownSfx3;
        public BlowupReference blowup;
        public short unknown22;

        private void OnValidate()
        {
            specialObjects ??= new SpecialObject[0];
            if (specialObjects.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many Special Objects", "Maximum of 255 special objects allowed on a single GizObstacle.", "OK");
                specialObjects = specialObjects.Take(255).ToArray();
            }

            unknown12 ??= new Vector3[0];
            if (unknown12.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many Vector3", "Maximum of 255 vector3 in array.", "OK");
                unknown12 = unknown12.Take(255).ToArray();
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