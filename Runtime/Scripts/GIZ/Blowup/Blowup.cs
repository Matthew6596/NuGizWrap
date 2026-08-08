#if UNITY_EDITOR
using System;
using NuGizWrap.Helper;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class Blowup : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => (game) switch { 
            TTGame.TCS => new[] { "Blownup", "Punched", "Plugging" }, 
            _ => new[] { "Blownup", "Punched", "Plugging", "BeenPickedUp" }
        };

        public static float GizmoScale = 0.25f, GizmoAlpha = 0.5f;

        [Flags]
        public enum InteractionOptions
        {
            None = 0, Unk1 = 1, Unk2 = 2, Collision = 4, ProximityTrigger = 8, Unk3 = 16, Unk4 = 32,
            Glass = 64, Unk6_DropHealth = 128, DropPowerup = 256, Unk7 = 512, Unk8 = 1024, Unk9 = 2048,
            Unk10 = 4096, Unk11 = 8192, Unk12 = 16384, RangedAttackable = 32768, MeleeAttackable = 0x10000, Unk13 = 0x20000, Unk14 = 0x40000,
            Unk15 = 0x80000, Unk16 = 0x100000, Unk17_ThermalSticky = 0x200000, Unk18 = 0x400000, Unk19 = 0x800000, Unk20_Torpedo = 0x1000000,
            Unk21 = 0x2000000, Unk22 = 0x4000000, Unk23 = 0x8000000, Unk24 = 0x10000000, Unk25 = 0x20000000, Unk26 = 0x40000000,
            //Default = Unk1 | Collision | Unk6_DropHealth | RangedAttackable | MeleeAttackable,
            //ThermalMetal = Unk2 | Unk6_DropHealth | Unk17_ThermalSticky,
        }

        public BlowupTypeReference type;
        public short unknown1, unknown2, unknown3, unknown4a;
        public InteractionOptions interactionOptions;
        public int unknown4b, unknown5;

        public short unknown33, unknown34;
        public byte unknown35, unknown36, unknown37;

        public int studsValue;
        public byte unknown6, unknown7, damage;
        public float range, unknown8, unknown9;
        public short unknown10, unknown11, unknown12;
        public float unknown13, unknown14, unknown15, unknown16, unknown17, unknown18, unknown19;
        public byte unknown20;
        public short unknown21, unknown22, unknown23, unknown24, unknown25;
        public float unknown26, unknown27, unknown28, unknown29, unknown30, unknown31, unknown32;

        public bool unknown38;
        public string unknown39;
        public bool unknown40, unknown41, unknown42, unknown43;
        public string unknown44;
        public bool unknown45;

        private static Texture2D icon;

        private void OnValidate()
        {
            this.SetIcon(ref icon, "Textures/GizmoIcons/BlowupIcon");
        }

        private void OnDrawGizmos()
        {
            Color col = new(1, 0, 0, GizmoAlpha);
            Giz.color = col;
            Giz.DrawSphere(transform.position, GizmoScale);
        }
    }
}
#endif