#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    using Audio;
    using GameScene;
    using Helper;

    public class GizForce : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "AtEnd", "NotAtStart", "AtStart", "StackComplete", "StackCompleteInOrder", "Destroyed/Thrown", "Complete", "BeingUsed" };

        public static float GizmoScale = 0.25f, GizmoAlpha = 0.5f;

        [Flags]
        public enum InteractionOptions
        {
            None = 0, Unk1 = 1, Returns=2, CanReturnLater=4, Unk2=8, DarkSide=16, Unk3=32,
            Unk4_TurnOnLight=64, Unk5=128, Unk6=256, Unk7=512, Unk8_CannotUndo=1024, Unk9_TwoPlayer=2048,
            Unk10=4096, Unk11=8192, Unk12=16384, Unk13=32768, Unk14=0x10000, Unk15=0x20000, Unk16=0x40000,
            Unk17=0x80000, Unk18=0x100000, Unk19=0x200000, Unk20=0x400000, Unk21=0x800000, Unk22=0x1000000
        }

        public Vector3 unknown1;
        public float returnTime, shakeTime, range;
        public Vector3 unknown2;
        public short unknown3;
        public InteractionOptions interactionOptions;
        public bool togglable;
        public byte unknown4, unknown5, unknown6;
        public byte specialObjectVersion;
        public SpecialObject[] specialObjects;
        public float forceSpeed, returnSpeed;
        public float autoForce, effectScale;
        public float unknown7;
        public short unknown8;
        public BlowupReference blowup;
        public ushort minStuds, maxStuds;
        public Transform studsSpawn;
        public float studsSpawnSpeed;
        public SampleReference processSound, completeSound, returnSound;

        private static Texture2D icon;

        private void OnValidate()
        {
            this.SetIcon(ref icon, "Textures/GizmoIcons/ForceIcon");

            specialObjects ??= new SpecialObject[0];
            if (specialObjects.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many Special Objects", "Maximum of 255 special objects allowed on a single GizForce.", "OK");
                specialObjects = specialObjects.Take(255).ToArray();
            }
        }

        private void OnDrawGizmos()
        {
            Color col = Color.cyan;
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