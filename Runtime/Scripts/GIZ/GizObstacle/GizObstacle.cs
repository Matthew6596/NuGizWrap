#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    using Audio;
    using GameScene;
    using Helper;

    public class GizObstacle : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => (game) switch { 
            TTGame.TCS => new[] { "AtEnd", "NotAtStart", "Proximity", "AtStart", "PlayingForward" },
            _ => new[] { "AtEnd", "NotAtStart", "Proximity", "AtStart", "PlayingForward", "Destroyed", "WithinActiveFrames" }
        };

        public static float GizmoScale = 0.2f, GizmoAlpha = 0.5f;

        public Transform triggerTransform;
        public float unknown1;
        public Vector3 unknown3 = new(0.00390625f, 0.00390625f, 0.00390625f);
        public short unknown4;
        public int unknown5, unknown6;
        public byte unknown9, unknown10;

        public float unknown17, unknown18, unknown19, unknown20 = 1;

        public byte unknown11 = 0xff;

        public byte specialObjectVersion = 3;
        public SpecialObject[] specialObjects;

        public float unknown12 = 1, unknown13, unknown14;
        public BlowupReference blowup;

        public ushort studsValue;
        [Tooltip("Transform of the stud spawn. Uses the position and yaw.")]
        public Transform studsSpawn;
        public float studsSpawnSpeed = 1.5f;

        public SampleReference unknownSfx1, unknownSfx2;
        public string unknown23;

        public int unknown21 = 1, unknown22 = 1;

        public Vector3 StudsSpawnPos => studsSpawn.position - transform.position;

        private static Texture2D icon;

        private void OnValidate()
        {
            this.SetIcon(ref icon, "Textures/GizmoIcons/ObstacleIcon");

            specialObjects ??= new SpecialObject[0];
            if(specialObjects.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many Special Objects","Maximum of 255 special objects allowed on a single GizObstacle.","OK");
                specialObjects = specialObjects.Take(255).ToArray();
            }
        }

        private void OnDrawGizmos()
        {
            Color col = Color.green;
            col.a = GizmoAlpha;
            Giz.color = col;

            if (triggerTransform == null || !triggerTransform.TryGetComponent(out GizObstacleTrigger t)) 
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