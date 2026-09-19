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

    public class GizFlockConfig : GizmoTypeConfig
    {
        public override string ID => "GizFlock";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 4, TTGame.LB1 => 2, _ => 1 };

        public int version;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            short flockCount = br.ReadInt16();

            string[] existingNames = new string[flockCount];
            for (int i = 0; i < flockCount; i++)
            {
                string name = br.ReadString8();

                ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject flockObj = new(name);
                flockObj.transform.SetParent(parent);

                var flock = flockObj.AddComponent<GizFlock>();

                flock.creature = br.ReadString8();
                flock.creatureCount = br.ReadInt16();

                flock.interactionOptions = br.ReadInt32();

                flockObj.transform.position = br.ReadVector3();
                flock.unknown3 = br.ReadSingle();
                flock.unknown4 = br.ReadSingle();
                flock.unknown5 = br.ReadSingle();
                flock.unknown6 = br.ReadSingle();

                if (version == 4) //LIJ1
                {
                    flock.unknown7 = br.ReadInt16();
                    flock.unknown8 = br.ReadSingle();
                    flock.unknown9 = br.ReadSingle();
                    flock.unknown10 = br.ReadSingle();
                    flock.unknown11 = br.ReadSingle();
                    flock.unknown12 = br.ReadSingle();
                    flock.unknown13 = br.ReadSingle();
                    flock.unknown14 = br.ReadSingle();
                    flock.unknown15 = br.ReadSingle();
                    flock.unknown16 = br.ReadSingle();
                    flock.unknown17 = br.ReadVector3();
                    flock.unknown18 = br.ReadSingle();
                    flock.unknown19 = br.ReadSingle();
                }
                else if (version == 2) //LB1
                {
                    br.ReadBytes(82);
                    Debug.LogWarning("GizFlock is not implemented fully yet for LB1.");
                }
                else throw new System.ArgumentException($"GizFlockSection version must be either 2 for LB1 or 4 for LIJ1, but it is {version}.");

                flock.unknown20 = br.ReadString8();
                flock.unknown21 = br.ReadSingle();
                flock.unknown22 = br.ReadSingle();
                flock.unknown23 = br.ReadSingle();
                flock.unknown24 = br.ReadSingle();

                ushort unk25Count = br.ReadUInt16();
                flock.unknown25 = new GizFlock.Unk25[unk25Count];
                for (int j = 0; j < unk25Count; j++)
                {
                    GizFlock.Unk25 unk25 = new();
                    byte unk1 = br.ReadByte();
                    unk25.unk1 = unk1;

                    unk25.unk2 = br.ReadByte();
                    unk25.unk3 = br.ReadString8();

                    unk25.unk4 = br.ReadVector3();

                    if (unk1 == 0) unk25.unk5 = br.ReadSingle();
                    else if (unk1 == 1) unk25.unk6 = br.ReadVector3();

                    flock.unknown25[j] = unk25;
                }

                byte unk26 = br.ReadByte();
                flock.unknown26 = unk26;
                flock.unknown27 = br.ReadByte();
                flock.unknown28 = br.ReadVector3();

                if (unk26 == 2 || unk26 == 3) flock.unknown29 = br.ReadSingle();
                else if (unk26 == 4 || unk26 == 5) flock.unknown30 = br.ReadVector3();
            }

            return flockCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var flocks = FindObjectsByType<GizFlock>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            short flockCount = (short)flocks.Length; //technically ushort but I don't care, you do not deserve more than 30k gizflocks
            bw.Write(flockCount);

            for (int i = 0; i < flockCount; i++)
            {
                var flock = flocks[i];
                bw.WriteString8(flock.name);
                bw.WriteString8(flock.creature);
                bw.Write(flock.creatureCount);

                bw.Write(flock.interactionOptions);

                bw.Write(flock.transform.position);

                bw.Write(flock.unknown3);
                bw.Write(flock.unknown4);
                bw.Write(flock.unknown5);
                bw.Write(flock.unknown6);
                bw.Write(flock.unknown7);
                bw.Write(flock.unknown8);
                bw.Write(flock.unknown9);
                bw.Write(flock.unknown10);
                bw.Write(flock.unknown11);
                bw.Write(flock.unknown12);
                bw.Write(flock.unknown13);
                bw.Write(flock.unknown14);
                bw.Write(flock.unknown15);
                bw.Write(flock.unknown16);
                bw.Write(flock.unknown17);
                bw.Write(flock.unknown18);
                bw.Write(flock.unknown19);
                bw.WriteString8(flock.unknown20);
                bw.Write(flock.unknown21);
                bw.Write(flock.unknown22);
                bw.Write(flock.unknown23);
                bw.Write(flock.unknown24);

                ushort unk25Count = (ushort)flock.unknown25.Length;
                bw.Write(unk25Count);
                for (int j = 0; j < unk25Count; j++)
                {
                    GizFlock.Unk25 unk25 = flock.unknown25[j];
                    byte unk1 = unk25.unk1;
                    bw.Write(unk1);

                    bw.Write(unk25.unk2);
                    bw.WriteString8(unk25.unk3);

                    bw.Write(unk25.unk4);

                    if (unk1 == 0) bw.Write(unk25.unk5);
                    else if (unk1 == 1) bw.Write(unk25.unk6);
                }

                byte unk26 = flock.unknown26;
                bw.Write(unk26);
                bw.Write(flock.unknown27);
                bw.Write(flock.unknown28);

                if (unk26 == 2 || unk26 == 3) bw.Write(flock.unknown29);
                else if (unk26 == 4 || unk26 == 5) bw.Write(flock.unknown30);
            }
        }
    }
}
#endif