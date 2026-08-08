//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.9swlweafd8xt
//-Matton
//===== ===== ===== ===== =====

#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class BlowupSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 31, TTGame.LIJ1=>40, TTGame.LB1=>45, _ => 1 };

        public override string ID => "blowup";

        public static BlowupSection Instance { get; private set; }

        public int version = 31;
        public float unknown;

        private static Texture2D icon;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);

            Instance.SetIcon(ref icon, "Textures/GizmoIcons/BlowupIcon");
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var blowupTypes = FindObjectsByType<BlowupType>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            var blowups = FindObjectsByType<Blowup>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int blowupTypeCount = blowupTypes.Length;
            int blowupCount = blowups.Length;

            if (version >= 2) bytes.AddInt(blowupTypeCount);
            bytes.AddInt(blowupCount);

            // Blowup Types
            if (version >= 2)
            {
                for (int i = 0; i < blowupTypeCount; i++)
                {
                    var bt = blowupTypes[i];

                    bytes.AddString8(bt.specialObject.specialObject);
                    bytes.AddString8(bt.name);

                    if (version >= 17)
                    {
                        bytes.AddString8(bt.parRef1);
                        bytes.AddString8(bt.parRef2);
                    }
                    if (version >= 4)
                    {
                        bytes.AddString8(bt.ptlRef1);
                        bytes.AddString8(bt.ptlRef2);
                        bytes.AddString8(bt.ptlRef3);
                    }
                    if (version >= 26)
                    {
                        bytes.AddString8(bt.unkRef1);
                        bytes.AddString8(bt.unkRef2);
                    }
                    if (version >= 27)
                    {
                        bytes.AddString8(bt.unkRef3);
                        bytes.AddString8(bt.unkRef4);
                    }

                    bytes.AddInt(bt.unknown1);
                    if (version >= 7)
                    {
                        bytes.AddInt(bt.unknown2);
                        bytes.Add(bt.unknown3);
                    }
                    if (version >= 8) bytes.AddFloat(bt.unknown4);
                    if (version >= 9) bytes.AddString8(bt.decal);
                    if (version >= 14)
                    {
                        bytes.AddFloat(bt.unknown5);
                        bytes.AddFloat(bt.unknown6);
                    }
                    if (version >= 15)
                    {
                        bytes.Add(bt.unknown7);
                        bytes.Add(bt.unknown8);
                    }

                    if (version >= 16)
                    {
                        bytes.Add((byte)(bt.nextData ? 1 : 0));
                        if (bt.nextData)
                        {
                            var sd = bt.subDataSet;
                            bytes.AddVector3(sd.unk1);
                            bytes.AddFloat(sd.unk2);
                            bytes.AddFloat(sd.unk3);
                            bytes.AddFloat(sd.unk4);
                            bytes.AddFloat(sd.unk5);
                            bytes.AddFloat(sd.unk6);
                            bytes.AddShort(sd.unk7);
                            bytes.Add(sd.unk8);
                            bytes.Add(sd.unk9);
                        }
                    }

                    if (version >= 18) bytes.AddString8(bt.emitObj1);
                    if (version >= 22)
                    {
                        bytes.AddString8(bt.emitObj2);
                        bytes.AddString8(bt.emitObj3);
                        bytes.AddString8(bt.emitObj4);
                    }
                    if (version >= 18)
                    {
                        bytes.Add(bt.unknown9);
                        bytes.AddFloat(bt.unknown10);
                        bytes.AddFloat(bt.unknown11);
                    }
                    if (version >= 19) bytes.AddString8(bt.shadow);
                    if (version >= 20) bytes.AddString8(bt.swap);
                    if (version >= 23) bytes.AddFloat(bt.unknown12);
                    if (version >= 24) bytes.AddFloat(bt.unknown13);

                    if (version >= 33) bytes.AddString8(bt.unknown14);
                    if (version >= 38) bytes.AddString8(bt.unknown15);
                }
            }

            // Blowups
            for (int i = 0; i < blowupCount; i++)
            {
                var blowup = blowups[i];

                bytes.AddString8(blowup.type.GetBlowupType());
                if (version >= 2) bytes.AddString8(blowup.name);
                bytes.AddVector3(blowup.transform.position);
                bytes.AddShort(blowup.unknown1);
                bytes.AddShort(blowup.unknown2);
                bytes.AddShort(blowup.unknown3);

                if (version >= 2 && version <= 19) bytes.AddShort((short)blowup.interactionOptions);
                if (version >= 20) bytes.AddInt((int)blowup.interactionOptions);

                if (version == 28) bytes.AddInt(blowup.unknown4b);
                if (version >= 30 && version < 34) bytes.AddInt(blowup.unknown5);
                if (version >= 34)
                {
                    bytes.AddShort(blowup.unknown33);
                    bytes.AddShort(blowup.unknown34);
                }
                if (version >= 41)
                {
                    bytes.Add(blowup.unknown35);
                    bytes.Add(blowup.unknown36);
                    bytes.Add(blowup.unknown37);
                }

                if (version >= 2)
                {
                    bytes.AddInt(blowup.studsValue);
                    bytes.Add(blowup.unknown6);
                    bytes.Add(blowup.unknown7);
                }
                if (version >= 4) bytes.Add(blowup.damage);
                if (version >= 6) bytes.AddFloat(blowup.range);
                if (version >= 8)
                {
                    bytes.AddFloat(blowup.unknown8);
                    bytes.AddFloat(blowup.unknown9);
                }

                if (version >= 9)
                {
                    bytes.AddShort(blowup.unknown10);
                    bytes.AddShort(blowup.unknown11);
                    bytes.AddShort(blowup.unknown12);
                    bytes.AddFloat(blowup.unknown13);
                    bytes.AddFloat(blowup.unknown14);
                    bytes.AddFloat(blowup.unknown15);
                }

                if (version >= 10) bytes.AddFloat(blowup.unknown16);
                if (version >= 11)
                {
                    bytes.AddFloat(blowup.unknown17);
                    bytes.AddFloat(blowup.unknown18);
                    bytes.AddFloat(blowup.unknown19);
                }
                if (version >= 12) bytes.Add(blowup.unknown20);

                if (version >= 13)
                {
                    bytes.AddShort(blowup.unknown21);
                    bytes.AddShort(blowup.unknown22);
                }
                if (version >= 19)
                {
                    bytes.AddShort(blowup.unknown23);
                    bytes.AddShort(blowup.unknown24);
                    bytes.AddShort(blowup.unknown25);
                    bytes.AddFloat(blowup.unknown26);
                    bytes.AddFloat(blowup.unknown27);
                    bytes.AddFloat(blowup.unknown28);
                    bytes.AddFloat(blowup.unknown29);
                }

                if (version >= 21) bytes.AddFloat(blowup.unknown30);
                if (version >= 23) bytes.AddFloat(blowup.unknown31);
                if (version >= 31) bytes.AddFloat(blowup.unknown32);

                if (version >= 32) bytes.Add((byte)(blowup.unknown38 ? 0xff : 0));
                if (version >= 36) bytes.AddString8(blowup.unknown39);
                if (version >= 37) bytes.Add((byte)(blowup.unknown40 ? 1 : 0));
                if (version >= 38) bytes.Add((byte)(blowup.unknown41 ? 1 : 0));
                if (version >= 40) bytes.Add((byte)(blowup.unknown42 ? 1 : 0));
                if (version >= 41) bytes.Add((byte)(blowup.unknown43 ? 1 : 0));
                if (version >= 44) bytes.AddString8(blowup.unknown44);
                if (version >= 45) bytes.Add((byte)(blowup.unknown45 ? 1 : 0));
            }

            if (version >= 39) bytes.AddFloat(unknown);
            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);

            int blowupTypeCount = 0;
            if (version >= 2) blowupTypeCount = bytes.ReadInt(ref index);
            int blowupCount = bytes.ReadInt(ref index);

            foreach (var bt in FindObjectsByType<BlowupType>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) bt.gameObject.DelayDestroy();
            foreach (var b in FindObjectsByType<Blowup>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) b.gameObject.DelayDestroy();

            //Blowup Types
            if (version >= 2)
            {
                for (int i = 0; i < blowupTypeCount; i++)
                {
                    string specObjName = bytes.ReadString8(ref index);

                    GameObject btObj = new(bytes.ReadString8(ref index));
                    btObj.transform.SetParent(transform);
                    var bt = btObj.AddComponent<BlowupType>();
                    bt.specialObject.specialObject = specObjName;

                    if (version >= 17)
                    {
                        bt.parRef1 = bytes.ReadString8(ref index);
                        bt.parRef2 = bytes.ReadString8(ref index);
                    }
                    if (version >= 4)
                    {
                        bt.ptlRef1 = bytes.ReadString8(ref index);
                        bt.ptlRef2 = bytes.ReadString8(ref index);
                        bt.ptlRef3 = bytes.ReadString8(ref index);
                    }
                    if (version >= 26)
                    {
                        bt.unkRef1 = bytes.ReadString8(ref index);
                        bt.unkRef2 = bytes.ReadString8(ref index);
                    }
                    if (version >= 27)
                    {
                        bt.unkRef3 = bytes.ReadString8(ref index);
                        bt.unkRef4 = bytes.ReadString8(ref index);
                    }

                    bt.unknown1 = bytes.ReadInt(ref index);
                    if (version >= 7)
                    {
                        bt.unknown2 = bytes.ReadInt(ref index);
                        bt.unknown3 = bytes.ReadByte(ref index);
                    }
                    if (version >= 8) bt.unknown4 = bytes.ReadFloat(ref index);
                    if (version >= 9) bt.decal = bytes.ReadString8(ref index);
                    if (version >= 14)
                    {
                        bt.unknown5 = bytes.ReadFloat(ref index);
                        bt.unknown6 = bytes.ReadFloat(ref index);
                    }
                    if (version >= 15)
                    {
                        bt.unknown7 = bytes.ReadByte(ref index);
                        bt.unknown8 = bytes.ReadByte(ref index);
                    }

                    if (version >= 16)
                    {
                        bt.nextData = bytes.ReadByte(ref index) != 0;
                        if (bt.nextData)
                        {
                            BlowupType.SubDataSet sd = new();
                            sd.unk1 = bytes.ReadVector3(ref index);
                            sd.unk2 = bytes.ReadFloat(ref index);
                            sd.unk3 = bytes.ReadFloat(ref index);
                            sd.unk4 = bytes.ReadFloat(ref index);
                            sd.unk5 = bytes.ReadFloat(ref index);
                            sd.unk6 = bytes.ReadFloat(ref index);
                            sd.unk7 = bytes.ReadShort(ref index);
                            sd.unk8 = bytes.ReadByte(ref index);
                            sd.unk9 = bytes.ReadByte(ref index);
                            bt.subDataSet = sd;
                        }
                    }

                    if (version >= 18) bt.emitObj1 = bytes.ReadString8(ref index);
                    if (version >= 22)
                    {
                        bt.emitObj2 = bytes.ReadString8(ref index);
                        bt.emitObj3 = bytes.ReadString8(ref index);
                        bt.emitObj4 = bytes.ReadString8(ref index);
                    }
                    if (version >= 18)
                    {
                        bt.unknown9 = bytes.ReadByte(ref index);
                        bt.unknown10 = bytes.ReadFloat(ref index);
                        bt.unknown11 = bytes.ReadFloat(ref index);
                    }
                    if (version >= 19) bt.shadow = bytes.ReadString8(ref index);
                    if (version >= 20) bt.swap = bytes.ReadString8(ref index);
                    if (version >= 23) bt.unknown12 = bytes.ReadFloat(ref index);
                    if (version >= 24) bt.unknown13 = bytes.ReadFloat(ref index);

                    if (version >= 33) bt.unknown14 = bytes.ReadString8(ref index);
                    if (version >= 38) bt.unknown15 = bytes.ReadString8(ref index);
                }
            }

            //Blowups
            for (int i = 0; i < blowupCount; i++)
            {
                string blowupType = bytes.ReadString8(ref index);
                string blowupName = version >= 2 ? bytes.ReadString8(ref index) : $"blowup_{i}";

                GameObject blowupObj = new(blowupName);
                blowupObj.transform.SetParent(transform);
                blowupObj.transform.position = bytes.ReadVector3(ref index);
                var blowup = blowupObj.AddComponent<Blowup>();
                blowup.type.SetBlowupType(blowupType);

                blowup.unknown1 = bytes.ReadShort(ref index);
                blowup.unknown2 = bytes.ReadShort(ref index);
                blowup.unknown3 = bytes.ReadShort(ref index);

                if (version >= 2 && version <= 19) blowup.interactionOptions = (Blowup.InteractionOptions)bytes.ReadShort(ref index);
                if (version >= 20) blowup.interactionOptions = (Blowup.InteractionOptions)bytes.ReadInt(ref index);

                if (version == 28) blowup.unknown4b = bytes.ReadInt(ref index);
                if (version >= 30 && version < 34) blowup.unknown5 = bytes.ReadInt(ref index);

                if (version >= 34)
                {
                    blowup.unknown33 = bytes.ReadShort(ref index);
                    blowup.unknown34 = bytes.ReadShort(ref index);
                }
                if (version >= 41)
                {
                    blowup.unknown35 = bytes.ReadByte(ref index);
                    blowup.unknown36 = bytes.ReadByte(ref index);
                    blowup.unknown37 = bytes.ReadByte(ref index);
                }

                if (version >= 2)
                {
                    blowup.studsValue = bytes.ReadInt(ref index);
                    blowup.unknown6 = bytes.ReadByte(ref index);
                    blowup.unknown7 = bytes.ReadByte(ref index);
                }
                if (version >= 4) blowup.damage = bytes.ReadByte(ref index);
                if (version >= 6) blowup.range = bytes.ReadFloat(ref index);
                if (version >= 8)
                {
                    blowup.unknown8 = bytes.ReadFloat(ref index);
                    blowup.unknown9 = bytes.ReadFloat(ref index);
                }

                if (version >= 9)
                {
                    blowup.unknown10 = bytes.ReadShort(ref index);
                    blowup.unknown11 = bytes.ReadShort(ref index);
                    blowup.unknown12 = bytes.ReadShort(ref index);
                    blowup.unknown13 = bytes.ReadFloat(ref index);
                    blowup.unknown14 = bytes.ReadFloat(ref index);
                    blowup.unknown15 = bytes.ReadFloat(ref index);
                }

                if (version >= 10) blowup.unknown16 = bytes.ReadFloat(ref index);
                if (version >= 11)
                {
                    blowup.unknown17 = bytes.ReadFloat(ref index);
                    blowup.unknown18 = bytes.ReadFloat(ref index);
                    blowup.unknown19 = bytes.ReadFloat(ref index);
                }
                if (version >= 12) blowup.unknown20 = bytes.ReadByte(ref index);

                if (version >= 13)
                {
                    blowup.unknown21 = bytes.ReadShort(ref index);
                    blowup.unknown22 = bytes.ReadShort(ref index);
                }
                if (version >= 19)
                {
                    blowup.unknown23 = bytes.ReadShort(ref index);
                    blowup.unknown24 = bytes.ReadShort(ref index);
                    blowup.unknown25 = bytes.ReadShort(ref index);
                    blowup.unknown26 = bytes.ReadFloat(ref index);
                    blowup.unknown27 = bytes.ReadFloat(ref index);
                    blowup.unknown28 = bytes.ReadFloat(ref index);
                    blowup.unknown29 = bytes.ReadFloat(ref index);
                }

                if (version >= 21) blowup.unknown30 = bytes.ReadFloat(ref index);
                if (version >= 23) blowup.unknown31 = bytes.ReadFloat(ref index);
                if (version >= 31) blowup.unknown32 = bytes.ReadFloat(ref index);

                if (version >= 33) blowup.unknown38 = bytes.ReadByte(ref index) != 0;
                if (version >= 36) blowup.unknown39 = bytes.ReadString8(ref index);
                if (version >= 37) blowup.unknown40 = bytes.ReadByte(ref index) != 0;
                if (version >= 38) blowup.unknown41 = bytes.ReadByte(ref index) != 0;
                if (version >= 40) blowup.unknown42 = bytes.ReadByte(ref index) != 0;
                if (version >= 41) blowup.unknown43 = bytes.ReadByte(ref index) != 0;
                if (version >= 44) blowup.unknown44 = bytes.ReadString8(ref index);
                if (version >= 45) blowup.unknown45 = bytes.ReadByte(ref index) != 0;
            }

            if (version >= 39) unknown = bytes.ReadFloat(ref index);
        }
    }
}
#endif