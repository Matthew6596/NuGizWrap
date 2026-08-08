#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    using GameScene;
    using Helper;

    public class GizBuildit : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Finished" };

        public static float GizmoScale = 0.25f, GizmoAlpha = 0.5f;

        public byte specialObjectVersion = 3;
        public SpecialObject[] specialObjects;
        public float jumpIntensity = 1.5f;
        public ushort minStuds, maxStuds;
        public byte unknown2, unknown3;
        public float unknown10 = 1;
        public float unknown4;
        public BlowupReference blowup;
        [Tooltip("Transform of the stud spawn. Uses the position and yaw.")]
        public Transform studsSpawn;
        public float studsSpawnSpeed = 1.75f;
        public short unknown7, unknown8;
        public string unknown9;

        public Vector3 StudsSpawnPos => studsSpawn.position - transform.position;

        private static Texture2D icon;

        private void OnValidate()
        {
            this.SetIcon(ref icon, "Textures/GizmoIcons/BuilditIcon");

            specialObjects ??= new SpecialObject[0];
            if (specialObjects.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many Special Objects", "Maximum of 255 special objects allowed on a single Buildit.", "OK");
                specialObjects = specialObjects.Take(255).ToArray();
            }
        }

        private void OnDrawGizmos()
        {
            Color col = new(1,1,0,GizmoAlpha);
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