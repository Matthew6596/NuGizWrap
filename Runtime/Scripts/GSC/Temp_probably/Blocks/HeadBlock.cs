#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class HeadBlock : GscBlock
    {
        public static HeadBlock Instance { get; private set; }

        public int PNTR_Offset, GSNH_Offset;

        public override void Load(BinaryReader br)
        {
            Instance = this;

            void SeekBlock<T>(int offset, string name) where T : GscBlock
            {
                long pos = br.BaseStream.Position;
                br.BaseStream.Seek(offset-4, SeekOrigin.Current);
                var block = NuGameScene.Instance.CreateBlock<T>($"{name} Block");
                block.Load(br);
                br.BaseStream.Position = pos;
            }

            PNTR_Offset = br.ReadInt32();
            GSNH_Offset = br.ReadInt32();

            SeekBlock<PointerBlock>(PNTR_Offset, "PNTR");
            SeekBlock<GSNHBlock>(GSNH_Offset, "GSNH");
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.HEADAddress = bw.BaseStream.Position;

            bw.Write(0); //placehold pntrs for PNTR_Offset and GSNH_Offset (PNTR_Offset isn't in PNTR Block)
            PointerBlock.WritePlaceholdPtr(bw);
        }

        public override void PostSave(BinaryWriter bw)
        {
            bw.WritePtr(GSCExporter.PNTRAddress);
            bw.WritePtr(GSCExporter.GSNHAddress);
        }
    }
}
#endif