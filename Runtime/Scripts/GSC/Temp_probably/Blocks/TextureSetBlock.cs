#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class TextureSetBlock : GscBlock
    {
        public static TextureSetBlock Instance { get; private set; }

        public int[] unk1;
        public TextureMeta[] textureMetas;

        public override void Load(BinaryReader br)
        {
            Instance = this;

            int txtrCount = GSNHBlock.Instance.textureCount;

            unk1 = new int[txtrCount];
            for(int i=0; i<txtrCount; i++) unk1[i] = br.ReadInt32();

            textureMetas = new TextureMeta[txtrCount];
            for(int i=0; i<txtrCount; i++) textureMetas[i] = TextureMeta.FromBytes(br);
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.TST0Address = bw.Pos();
            int txtrCount = textureMetas.Length; //sus

            if(unk1.Length != txtrCount) throw new DataMisalignedException($"unk1 should be the same length as textureMetas ({txtrCount})");

            for (int i = 0; i < txtrCount; i++) bw.Write(unk1[i]);

            for(int i=0; i<txtrCount; i++)
            {
                var txtrMeta = textureMetas[i];
                bw.Write(txtrMeta.width);
                bw.Write(txtrMeta.height);
                bw.Write(txtrMeta.unk1);
                bw.Write(txtrMeta.unk2);
                bw.Write(txtrMeta.unk3);
                bw.Write(txtrMeta.unk4);
                bw.Write(txtrMeta.unk5);
                bw.Write(txtrMeta.unk6);
                bw.Write(txtrMeta.unk7);
                bw.Write(txtrMeta.unk8);
            }
        }

        [Serializable]
        public struct TextureMeta
        {
            public int width, height;
            public int unk1, unk2, unk3, unk4;
            public int unk5, unk6, unk7, unk8;

            public static TextureMeta FromBytes(BinaryReader br)
            {
                return new()
                {
                    width = br.ReadInt32(),
                    height = br.ReadInt32(),
                    unk1 = br.ReadInt32(),
                    unk2 = br.ReadInt32(),
                    unk3 = br.ReadInt32(),
                    unk4 = br.ReadInt32(),
                    unk5 = br.ReadInt32(),
                    unk6 = br.ReadInt32(),
                    unk7 = br.ReadInt32(),
                    unk8 = br.ReadInt32(),
                };
            }
        }
    }
}
#endif