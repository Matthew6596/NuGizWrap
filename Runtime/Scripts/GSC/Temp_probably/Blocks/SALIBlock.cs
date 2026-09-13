#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class SALIBlock : GscBlock
    {
        public static SALIBlock Instance { get; private set; }

        public int version;
        public Unknown1[] unknown1s;

        public override void Load(BinaryReader br)
        {
            Instance = this;

            int unkCount = br.ReadInt32();
            unknown1s = new Unknown1[unkCount];
            version = br.ReadInt32();

            for(int i=0; i<unkCount; i++)
            {
                ushort unk2Count = br.ReadUInt16();
                ushort unk = br.ReadUInt16();
                long unk2sAddr = br.ReadPtr();
                long unk3sAddr = br.ReadPtr();

                long addr = br.GoToAddr(unk2sAddr);
                float[] unk2s = new float[unk2Count];
                for (int j = 0; j < unk2Count; j++) unk2s[j] = br.ReadSingle();

                br.GoToAddr(unk3sAddr);
                byte[] unk3s = br.ReadBytes(unk2Count);

                Unknown1 unk1 = new()
                {
                    unk = unk,
                    unk2s = unk2s,
                    unk3s = unk3s,
                };
                unknown1s[i] = unk1;

                br.BaseStream.Position = addr;
            }
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.SALIAddress = bw.Pos();

            bw.Write(unknown1s.Length);
            bw.Write(version);

            List<long> unk1Addrs = new();
            foreach(var unk1 in unknown1s)
            {
                int unk2Count = unk1.unk2s.Length;
                if (unk2Count != unk1.unk3s.Length) 
                    throw new DataMisalignedException($"unk2s (length: {unk2Count}) and unk3s (length: {unk1.unk3s.Length}) must both have the same length.");

                bw.Write((ushort)unk2Count);
                bw.Write(unk1.unk);
                unk1Addrs.Add(bw.Pos());
                PointerBlock.WritePlaceholdPtr(bw);
                PointerBlock.WritePlaceholdPtr(bw);
            }

            for(int i=0; i<unknown1s.Length; i++)
            {
                var unk1 = unknown1s[i];
                long unk1Addr = unk1Addrs[i];

                long unk2Addr = bw.Pos();
                bw.BaseStream.Position = unk1Addr;
                bw.WritePtr(unk2Addr);
                bw.BaseStream.Position = unk2Addr;

                foreach(var unk2 in unk1.unk2s) bw.Write(unk2);
            }

            for (int i = 0; i < unknown1s.Length; i++)
            {
                var unk1 = unknown1s[i];
                long unk1Addr = unk1Addrs[i] + 4;

                long unk3Addr = bw.Pos();
                bw.BaseStream.Position = unk1Addr;
                bw.WritePtr(unk3Addr);
                bw.BaseStream.Position = unk3Addr;

                bw.Write(unk1.unk3s);
            }
        }

        [Serializable]
        public struct Unknown1
        {
            public ushort unk;
            public float[] unk2s;
            public byte[] unk3s;
        }
    }
}
#endif