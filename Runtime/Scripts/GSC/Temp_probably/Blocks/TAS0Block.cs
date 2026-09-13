#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class TAS0Block : GscBlock
    {
        public static TAS0Block Instance { get; private set; }

        public int unknown2;
        public Unknown1[] unknown1s;
        public Unknown3[] unknown3s;
        public int unknown4,unknown5;

        public override void Load(BinaryReader br)
        {
            Instance = this;

            int unk1Count = br.ReadInt32();
            unknown2 = br.ReadInt32();

            unknown1s = new Unknown1[unk1Count];
            for(int i=0; i<unk1Count; i++)
            {
                unknown1s[i] = new Unknown1()
                {
                    unkPtr1 = br.ReadInt32(),
                    unkPtr2 = br.ReadInt32(),
                    unk1 = br.ReadInt32(),
                    unk2 = br.ReadInt16(),
                    unk3 = br.ReadInt16(),
                    unk4 = br.ReadInt32(),
                    unkPtr3 = br.ReadInt32(),
                    unkPtr4 = br.ReadInt32(),
                    unkPtr5 = br.ReadInt32(),
                };
            }

            int unk3Count = br.ReadInt32();
            unknown3s = new Unknown3[unk3Count];
            for(int i=0; i<unk3Count; i++) unknown3s[i] = new() { unk1 = br.ReadInt16(), unk2 = br.ReadInt16() };

            unknown4 = br.ReadInt32(); //might just be padding idk
            unknown5 = br.ReadInt32();
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.TAS0Address = bw.Pos();

            bw.Write(unknown1s.Length);
            bw.Write(unknown2);

            for(int i=0; i<unknown1s.Length; i++)
            {
                var unk1 = unknown1s[i];
                PointerBlock.WritePlaceholdPtr(bw);
                PointerBlock.WritePlaceholdPtr(bw);
                bw.Write(unk1.unk1);
                bw.Write(unk1.unk2);
                bw.Write(unk1.unk3);
                bw.Write(unk1.unk4);
                PointerBlock.WritePlaceholdPtr(bw);
                PointerBlock.WritePlaceholdPtr(bw);
                PointerBlock.WritePlaceholdPtr(bw);
            }

            bw.Write(unknown3s.Length);
            for(int i=0; i<unknown3s.Length; i++)
            {
                var unk3 = unknown3s[i];
                bw.Write(unk3.unk1);
                bw.Write(unk3.unk2);
            }

            bw.Write(unknown4); //might be padding idk
            bw.Write(unknown5);
        }

        [Serializable]
        public struct Unknown1
        {
            public int unkPtr1, unkPtr2, unk1;
            public short unk2, unk3;
            public int unk4, unkPtr3, unkPtr4, unkPtr5;
        }

        [Serializable]
        public struct Unknown3
        {
            public short unk1, unk2;
        }
    }
}
#endif