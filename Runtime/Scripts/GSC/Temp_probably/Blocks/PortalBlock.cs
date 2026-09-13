#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class PortalBlock : GscBlock
    {
        public int unknown1;

        public short[] unknown4s;
        public PortObj5[] unknown5s;
        public short[] unknown7s;
        public Vector3[] portalRectPoints;
        public PortObj3Room[] unknown3sRooms;
        public PortObj2[] unknown2s;

        public override void Load(BinaryReader br)
        {
            unknown1 = br.ReadInt32();
            short unk2Count = br.ReadInt16();
            short unk3Count = br.ReadInt16();
            short unk4Count = br.ReadInt16();
            short unk5Count = br.ReadInt16();
            short rectPointsCount = br.ReadInt16();
            short unk7Count = br.ReadInt16();

            unknown4s = new short[unk4Count];
            for(int i=0; i<unk4Count; i++) unknown4s[i] = br.ReadInt16();

            unknown5s = new PortObj5[unk5Count];
            for(int i=0; i<unk5Count; i++) unknown5s[i] = new() { unk1=br.ReadSingle(), unk2=br.ReadSingle(), unk3=br.ReadSingle(), unk4=br.ReadSingle()};

            unknown7s = new short[unk7Count];
            for (int i = 0; i < unk7Count; i++) unknown7s[i] = br.ReadInt16();

            portalRectPoints = new Vector3[rectPointsCount];
            for (int i = 0; i < rectPointsCount; i++) portalRectPoints[i] = br.ReadVector3();

            unknown3sRooms = new PortObj3Room[unk3Count];
            for(int i=0; i<unk3Count; i++)
            {
                unknown3sRooms[i] = new()
                {
                    unk4Ptr = br.ReadInt32(),
                    unk5Ptr = br.ReadInt32(),
                    unk7Ptr = br.ReadInt32(),
                    numPortals = br.ReadInt16(),
                    unk2 = br.ReadInt16(),
                    unk3 = br.ReadInt32(),
                    unk4 = br.ReadInt32()
                };
            }

            unknown2s = new PortObj2[unk2Count];
            for (int i = 0; i < unk2Count; i++)
            {
                unknown2s[i] = new()
                {
                    unk1 = br.ReadSingle(),
                    unk2 = br.ReadInt32(),
                    unk3 = br.ReadSingle(),
                    unk4 = br.ReadSingle(),
                    unk5 = br.ReadInt32(),
                    unk6 = br.ReadInt16(),
                    room1ID = br.ReadInt16(),
                    room2ID = br.ReadInt16(),
                    unk7 = br.ReadInt16(),
                    unk8 = br.ReadInt32()
                };
            }
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.PORTAddress = bw.Pos();

            bw.Write(unknown1);

            bw.Write((short)unknown2s.Length);
            bw.Write((short)unknown3sRooms.Length);
            bw.Write((short)unknown4s.Length);
            bw.Write((short)unknown5s.Length);
            bw.Write((short)portalRectPoints.Length);
            bw.Write((short)unknown7s.Length);

            for (int i = 0; i < unknown4s.Length; i++) bw.Write(unknown4s[i]);

            for(int i=0; i<unknown5s.Length; i++)
            {
                var unk5 = unknown5s[i];
                bw.Write(unk5.unk1);
                bw.Write(unk5.unk2);
                bw.Write(unk5.unk3);
                bw.Write(unk5.unk4);
            }

            for (int i = 0; i < unknown7s.Length; i++) bw.Write(unknown7s[i]);
            for (int i = 0; i < portalRectPoints.Length; i++) bw.Write(portalRectPoints[i]);

            for(int i=0; i<unknown3sRooms.Length; i++)
            {
                var unk3 = unknown3sRooms[i];
                PointerBlock.WritePlaceholdPtr(bw); //I think these can be written right away
                PointerBlock.WritePlaceholdPtr(bw); //I think they're pointer to previous lists
                PointerBlock.WritePlaceholdPtr(bw);
                bw.Write(unk3.numPortals);
                bw.Write(unk3.unk2);
                bw.Write(unk3.unk3);
                bw.Write(unk3.unk4);
            }

            for(int i=0; i<unknown2s.Length; i++)
            {
                var unk2 = unknown2s[i];
                bw.Write(unk2.unk1);
                bw.Write(unk2.unk2);
                bw.Write(unk2.unk3);
                bw.Write(unk2.unk4);
                bw.Write(unk2.unk5);
                bw.Write(unk2.unk6);
                bw.Write(unk2.room1ID);
                bw.Write(unk2.room2ID);
                bw.Write(unk2.unk7);
                bw.Write(unk2.unk8);
            }
        }

        [Serializable]
        public struct PortObj5
        {
            public float unk1, unk2, unk3, unk4;
        }

        [Serializable]
        public struct PortObj3Room
        {
            public int unk4Ptr, unk5Ptr, unk7Ptr;
            public short numPortals, unk2;
            public int unk3, unk4;
        }

        [Serializable]
        public struct PortObj2
        {
            public float unk1;
            public int unk2;
            public float unk3, unk4;
            public int unk5;
            public short unk6, room1ID, room2ID, unk7;
            public int unk8;
        }
    }
}
#endif