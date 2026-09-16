#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;

    public class BlowupConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 31, TTGame.LIJ1 => 40, TTGame.LB1 => 45, _ => 1 };

        public int version = 31;
        public float unknown;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();

            int blowupTypeCount = 0;
            if (version >= 2) blowupTypeCount = br.ReadInt32();
            int blowupCount = br.ReadInt32();

            List<string> existingNames = new();
            //Blowup Types
            if (version >= 2)
            {
                for (int i = 0; i < blowupTypeCount; i++)
                {
                    string specObjName = br.ReadString8();

                    string name = br.ReadString8();
                    name = ObjectNames.GetUniqueName(existingNames.ToArray(), name);
                    existingNames.Add(name);
                    GameObject btObj = new(name);
                    btObj.transform.SetParent(parent);
                    var bt = btObj.AddComponent<BlowupType>();
                    bt.specialObject.specialObject = specObjName;

                    if (version >= 17)
                    {
                        bt.partType1 = br.ReadString8();
                        bt.partType2 = br.ReadString8();
                    }
                    if (version >= 4)
                    {
                        bt.debrisEffect1 = br.ReadString8();
                        bt.debrisEffect2 = br.ReadString8();
                        bt.debrisEffect3 = br.ReadString8();
                    }
                    if (version >= 26)
                    {
                        bt.debrisEffect4 = br.ReadString8();
                        bt.debrisEffect5 = br.ReadString8();
                    }
                    if (version >= 27)
                    {
                        bt.debrisEffect6 = br.ReadString8();
                        bt.debrisEffect7 = br.ReadString8();
                    }

                    bt.unknown1 = br.ReadInt32();
                    if (version >= 7)
                    {
                        bt.unknown2 = br.ReadInt32();
                        bt.unknown3 = br.ReadByte();
                    }
                    if (version >= 8) bt.unknown4 = br.ReadSingle();
                    if (version >= 9) bt.decal = br.ReadString8();
                    if (version >= 14)
                    {
                        bt.unknown5 = br.ReadSingle();
                        bt.unknown6 = br.ReadSingle();
                    }
                    if (version >= 15)
                    {
                        bt.unknown7 = br.ReadByte();
                        bt.unknown8 = br.ReadByte();
                    }

                    if (version >= 16)
                    {
                        bt.nextData = br.ReadByte() != 0;
                        if (bt.nextData)
                        {
                            BlowupType.SubDataSet sd = new();
                            sd.unk1 = br.ReadVector3();
                            sd.unk2 = br.ReadSingle();
                            sd.unk3 = br.ReadSingle();
                            sd.unk4 = br.ReadSingle();
                            sd.unk5 = br.ReadSingle();
                            sd.unk6 = br.ReadSingle();
                            sd.unk7 = br.ReadInt16();
                            sd.unk8 = br.ReadByte();
                            sd.unk9 = br.ReadByte();
                            bt.subDataSet = sd;
                        }
                    }

                    if (version >= 18) bt.emitObj1 = br.ReadString8();
                    if (version >= 22)
                    {
                        bt.emitObj2 = br.ReadString8();
                        bt.emitObj3 = br.ReadString8();
                        bt.emitObj4 = br.ReadString8();
                    }
                    if (version >= 18)
                    {
                        bt.unknown9 = br.ReadByte();
                        bt.unknown10 = br.ReadSingle();
                        bt.unknown11 = br.ReadSingle();
                    }
                    if (version >= 19) bt.shadow = br.ReadString8();
                    if (version >= 20) bt.swap = br.ReadString8();
                    if (version >= 23) bt.unknown12 = br.ReadSingle();
                    if (version >= 24) bt.unknown13 = br.ReadSingle();

                    if (version >= 33) bt.unknown14 = br.ReadString8();
                    if (version >= 38) bt.unknown15 = br.ReadString8();
                }
            }

            //Blowups
            for (int i = 0; i < blowupCount; i++)
            {
                string blowupType = br.ReadString8();
                string blowupName = version >= 2 ? br.ReadString8() : $"blowup_{i}";

                blowupName = ObjectNames.GetUniqueName(existingNames.ToArray(), blowupName);
                existingNames.Add(blowupName);

                GameObject blowupObj = new(blowupName);
                blowupObj.transform.SetParent(parent);
                blowupObj.transform.position = br.ReadVector3();
                var blowup = blowupObj.AddComponent<Blowup>();
                blowup.type.SetBlowupType(blowupType);

                blowup.transform.eulerAngles = br.ReadXYZEuler();

                if (version >= 2 && version <= 19) blowup.interactionOptions = (Blowup.InteractionOptions)br.ReadInt16();
                if (version >= 20) blowup.interactionOptions = (Blowup.InteractionOptions)br.ReadInt32();

                if (version == 28) blowup.unknown4b = br.ReadInt32();
                if (version >= 30 && version < 34) blowup.unknown5 = br.ReadInt32();

                if (version >= 34)
                {
                    blowup.plugType = br.ReadInt16();
                    blowup.validPlugs = br.ReadInt16();
                }
                if (version >= 41)
                {
                    blowup.unknown35 = br.ReadByte();
                    blowup.unknown36 = br.ReadByte();
                    blowup.unknown37 = br.ReadByte();
                }

                if (version >= 2)
                {
                    blowup.studsValue = br.ReadInt32();
                    blowup.studsValueMultiplier = (sbyte)br.ReadByte();
                    blowup.unknown7 = br.ReadByte();
                }
                if (version >= 4) blowup.damage = br.ReadByte();
                if (version >= 6) blowup.range = br.ReadSingle();
                if (version >= 8)
                {
                    blowup.unknown8 = br.ReadSingle();
                    blowup.unknown9 = br.ReadSingle();
                }

                if (version >= 9)
                {
                    blowup.unknown10 = br.ReadInt16();
                    blowup.unknown11 = br.ReadInt16();
                    blowup.unknown12 = br.ReadInt16();
                    blowup.unknown13 = br.ReadSingle();
                    blowup.unknown14 = br.ReadSingle();
                    blowup.unknown15 = br.ReadSingle();
                }

                if (version >= 10) blowup.unknown16 = br.ReadSingle();
                if (version >= 11)
                {
                    blowup.unknown17 = br.ReadSingle();
                    blowup.unknown18 = br.ReadSingle();
                    blowup.unknown19 = br.ReadSingle();
                }
                if (version >= 12) blowup.unknown20 = br.ReadByte();

                if (version >= 13)
                {
                    blowup.unknown21 = br.ReadInt16();
                    blowup.unknown22 = br.ReadInt16();
                }
                if (version >= 19)
                {
                    blowup.unknown23 = br.ReadInt16();
                    blowup.unknown24 = br.ReadInt16();
                    blowup.unknown25 = br.ReadInt16();
                    blowup.unknown26 = br.ReadSingle();
                    blowup.unknown27 = br.ReadSingle();
                    blowup.unknown28 = br.ReadSingle();
                    blowup.unknown29 = br.ReadSingle();
                }

                if (version >= 21) blowup.unknown30 = br.ReadSingle();
                if (version >= 23) blowup.unknown31 = br.ReadSingle();
                if (version >= 31) blowup.unknown32 = br.ReadSingle();

                if (version >= 33) blowup.unknown38 = br.ReadByte() != 0;
                if (version >= 36) blowup.unknown39 = br.ReadString8();
                if (version >= 37) blowup.unknown40 = br.ReadByte() != 0;
                if (version >= 38) blowup.unknown41 = br.ReadByte() != 0;
                if (version >= 40) blowup.unknown42 = br.ReadByte() != 0;
                if (version >= 41) blowup.unknown43 = br.ReadByte() != 0;
                if (version >= 44) blowup.unknown44 = br.ReadString8();
                if (version >= 45) blowup.unknown45 = br.ReadByte() != 0;
            }

            if (version >= 39) unknown = br.ReadSingle();

            return blowupCount + blowupTypeCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var blowupTypes = FindObjectsByType<BlowupType>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            var blowups = FindObjectsByType<Blowup>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int blowupTypeCount = blowupTypes.Length;
            int blowupCount = blowups.Length;

            if (version >= 2) bw.Write(blowupTypeCount);
            bw.Write(blowupCount);

            // Blowup Types
            if (version >= 2)
            {
                for (int i = 0; i < blowupTypeCount; i++)
                {
                    var bt = blowupTypes[i];

                    bw.WriteString8(bt.specialObject.specialObject);
                    bw.WriteString8(bt.name);

                    if (version >= 17)
                    {
                        bw.WriteString8(bt.partType1);
                        bw.WriteString8(bt.partType2);
                    }
                    if (version >= 4)
                    {
                        bw.WriteString8(bt.debrisEffect1);
                        bw.WriteString8(bt.debrisEffect2);
                        bw.WriteString8(bt.debrisEffect3);
                    }
                    if (version >= 26)
                    {
                        bw.WriteString8(bt.debrisEffect4);
                        bw.WriteString8(bt.debrisEffect5);
                    }
                    if (version >= 27)
                    {
                        bw.WriteString8(bt.debrisEffect6);
                        bw.WriteString8(bt.debrisEffect7);
                    }

                    bw.Write(bt.unknown1);
                    if (version >= 7)
                    {
                        bw.Write(bt.unknown2);
                        bw.Write(bt.unknown3);
                    }
                    if (version >= 8) bw.Write(bt.unknown4);
                    if (version >= 9) bw.WriteString8(bt.decal);
                    if (version >= 14)
                    {
                        bw.Write(bt.unknown5);
                        bw.Write(bt.unknown6);
                    }
                    if (version >= 15)
                    {
                        bw.Write(bt.unknown7);
                        bw.Write(bt.unknown8);
                    }

                    if (version >= 16)
                    {
                        bw.Write((byte)(bt.nextData ? 1 : 0));
                        if (bt.nextData)
                        {
                            var sd = bt.subDataSet;
                            bw.Write(sd.unk1);
                            bw.Write(sd.unk2);
                            bw.Write(sd.unk3);
                            bw.Write(sd.unk4);
                            bw.Write(sd.unk5);
                            bw.Write(sd.unk6);
                            bw.Write(sd.unk7);
                            bw.Write(sd.unk8);
                            bw.Write(sd.unk9);
                        }
                    }

                    if (version >= 18) bw.WriteString8(bt.emitObj1);
                    if (version >= 22)
                    {
                        bw.WriteString8(bt.emitObj2);
                        bw.WriteString8(bt.emitObj3);
                        bw.WriteString8(bt.emitObj4);
                    }
                    if (version >= 18)
                    {
                        bw.Write(bt.unknown9);
                        bw.Write(bt.unknown10);
                        bw.Write(bt.unknown11);
                    }
                    if (version >= 19) bw.WriteString8(bt.shadow);
                    if (version >= 20) bw.WriteString8(bt.swap);
                    if (version >= 23) bw.Write(bt.unknown12);
                    if (version >= 24) bw.Write(bt.unknown13);

                    if (version >= 33) bw.WriteString8(bt.unknown14);
                    if (version >= 38) bw.WriteString8(bt.unknown15);
                }
            }

            // Blowups
            for (int i = 0; i < blowupCount; i++)
            {
                var blowup = blowups[i];

                bw.WriteString8(blowup.type.GetBlowupType());
                if (version >= 2) bw.WriteString8(blowup.name);
                bw.Write(blowup.transform.position);
                Vector3 euler = blowup.transform.eulerAngles;
                bw.Write((short)euler.x.ToShortAng());
                bw.Write((short)euler.y.ToShortAng());
                bw.Write((short)euler.z.ToShortAng());

                if (version >= 2 && version <= 19) bw.Write((short)blowup.interactionOptions);
                if (version >= 20) bw.Write((int)blowup.interactionOptions);

                if (version == 28) bw.Write(blowup.unknown4b);
                if (version >= 30 && version < 34) bw.Write(blowup.unknown5);
                if (version >= 34)
                {
                    bw.Write(blowup.plugType);
                    bw.Write(blowup.validPlugs);
                }
                if (version >= 41)
                {
                    bw.Write(blowup.unknown35);
                    bw.Write(blowup.unknown36);
                    bw.Write(blowup.unknown37);
                }

                if (version >= 2)
                {
                    bw.Write(blowup.studsValue);
                    bw.Write((byte)blowup.studsValueMultiplier);
                    bw.Write(blowup.unknown7);
                }
                if (version >= 4) bw.Write(blowup.damage);
                if (version >= 6) bw.Write(blowup.range);
                if (version >= 8)
                {
                    bw.Write(blowup.unknown8);
                    bw.Write(blowup.unknown9);
                }

                if (version >= 9)
                {
                    bw.Write(blowup.unknown10);
                    bw.Write(blowup.unknown11);
                    bw.Write(blowup.unknown12);
                    bw.Write(blowup.unknown13);
                    bw.Write(blowup.unknown14);
                    bw.Write(blowup.unknown15);
                }

                if (version >= 10) bw.Write(blowup.unknown16);
                if (version >= 11)
                {
                    bw.Write(blowup.unknown17);
                    bw.Write(blowup.unknown18);
                    bw.Write(blowup.unknown19);
                }
                if (version >= 12) bw.Write(blowup.unknown20);

                if (version >= 13)
                {
                    bw.Write(blowup.unknown21);
                    bw.Write(blowup.unknown22);
                }
                if (version >= 19)
                {
                    bw.Write(blowup.unknown23);
                    bw.Write(blowup.unknown24);
                    bw.Write(blowup.unknown25);
                    bw.Write(blowup.unknown26);
                    bw.Write(blowup.unknown27);
                    bw.Write(blowup.unknown28);
                    bw.Write(blowup.unknown29);
                }

                if (version >= 21) bw.Write(blowup.unknown30);
                if (version >= 23) bw.Write(blowup.unknown31);
                if (version >= 31) bw.Write(blowup.unknown32);

                if (version >= 32) bw.Write((byte)(blowup.unknown38 ? 0xff : 0));
                if (version >= 36) bw.WriteString8(blowup.unknown39);
                if (version >= 37) bw.Write((byte)(blowup.unknown40 ? 1 : 0));
                if (version >= 38) bw.Write((byte)(blowup.unknown41 ? 1 : 0));
                if (version >= 40) bw.Write((byte)(blowup.unknown42 ? 1 : 0));
                if (version >= 41) bw.Write((byte)(blowup.unknown43 ? 1 : 0));
                if (version >= 44) bw.WriteString8(blowup.unknown44);
                if (version >= 45) bw.Write((byte)(blowup.unknown45 ? 1 : 0));
            }

            if (version >= 39) bw.Write(unknown);
        }
    }
}
#endif