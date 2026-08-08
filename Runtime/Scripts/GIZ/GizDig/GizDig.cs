#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    using Audio;
    using GameScene;

    public class GizDig : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "AtEnd", "NotAtStart", "AtStart" };

        [Flags]
        public enum InteractionOptions
        {
            None = 0, Unk1 = 1, Unk2=2, Unk3=4, Unk4=8, Unk5= 16, Unk6=32,
            Unk7=64, Unk8=128, Unk9=256, Unk10=512, Unk11=1024, Unk12=2048,
            Unk13=4096, Unk14=8192, Unk15=16384, Unk16=32768, Unk17=0x10000, Unk18=0x20000, Unk19=0x40000,
            Unk20=0x80000, Unk21=0x100000, Unk22=0x200000, Unk23=0x400000, Unk24=0x800000, Unk25=0x1000000
        }

        public enum Tool { Shovel=0, Wrench=1 }

        public Vector3 unknown1;
        public float unknown2;
        public InteractionOptions interactionOptions;
        public byte specialObjectVersion;
        public SpecialObject[] specialObjects;
        public float animSpeed, animAdvanceAmount;
        public BlowupReference blowup;
        public ushort studsValue;
        public Transform studsSpawn;
        public float studsSpawnSpeed;
        public SampleReference unknownSfx;
        public short numSteps, unknown7;
        public Tool tool;

        private void OnValidate()
        {
            specialObjects ??= new SpecialObject[0];
            if (specialObjects.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many Special Objects", "Maximum of 255 special objects allowed on a single GizDig.", "OK");
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
            public float unknown1, animationTime;
            public int unknown2;
            public short unknown3;
        }
    }
}
#endif