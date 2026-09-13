#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    public class FDNSBlock : GscBlock
    {
        public Vector2[] unknown;

        public override void Load(BinaryReader br)
        {
            int indCount = GSNHBlock.Instance.indexCount;
            unknown = new Vector2[indCount];
            for(int i=0; i<indCount; i++) unknown[i] = new(br.ReadSingle(), br.ReadSingle());
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.FDNSAddress = bw.BaseStream.Position;

            foreach (var unk in unknown)
            {
                bw.Write(unk.x);
                bw.Write(unk.y);
            }
        }
    }
}
#endif