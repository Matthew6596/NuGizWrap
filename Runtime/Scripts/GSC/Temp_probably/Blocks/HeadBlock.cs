#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    public class HeadBlock : GscBlock
    {
        public static HeadBlock Instance { get; private set; }

        public int PNTR_Index, GSNH_Index;

        public override void Load(BinaryReader br)
        {
            Instance = this;

            PNTR_Index = br.ReadInt32() - 4;
            GSNH_Index = br.ReadInt32() - 4;

            /*void SeekBlock<T>(int offset, string name) where T : GscBlock
            {
                long pos = br.BaseStream.Position;
                br.BaseStream.Seek(offset-4, SeekOrigin.Current);
                var block = NuGameScene.Instance.CreateBlock<T>($"{name} Block");
                block.Load(br);
                br.BaseStream.Position = pos;
            }

            SeekBlock<PointerBlock>(br.ReadInt32(), "PNTR");
            SeekBlock<GSNHBlock>(br.ReadInt32(), "GSNH");*/
        }

        public override void Save(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif