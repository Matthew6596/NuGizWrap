#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    using Audio;
    using GameScene;
    using Helper;

    public class GizObstacle : Gizmo
    {
        public static float GizmoScale = 0.25f, GizmoAlpha = 0.5f;

        public Transform triggerTransform;
        public float unknown1;
        public Vector3 unknown3;
        public short unknown4;
        public int unknown5, unknown6;
        public short unknown7;
        public byte unknown8, unknown9, unknown10;

        public float unknown17, unknown18, unknown19, unknown20;

        public byte unknown11;

        public byte specialObjectVersion = 3;
        public SpecialObject[] specialObjects;

        public float unknown12, unknown13, unknown14;
        public short unknown15;
        public string unknown16;
        public ushort minStuds, maxStuds;
        [Tooltip("Transform of the stud spawn. Uses the position and yaw.")]
        public Transform studsSpawn;
        public float studsSpawnSpeed;
        public SampleReference unknownSfx1, unknownSfx2, unknownSfx3;

        public int unknown21, unknown22;

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

            if (triggerTransform != null && triggerTransform.TryGetComponent(out SphereCollider trigger)) 
                Giz.DrawSphere(triggerTransform.position, trigger.radius);
            else 
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