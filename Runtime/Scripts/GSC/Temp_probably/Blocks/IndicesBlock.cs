#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    public class IndicesBlock : GscBlock
    {
        public int unknown;
        public ushort[] indices;

        public override void Load(BinaryReader br)
        {
            int indexCount = br.ReadInt32();
            unknown = br.ReadInt32();
            indices = new ushort[indexCount];
            for(int i=0; i<indexCount; i++) indices[i] = br.ReadUInt16();
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.INIDAddress = bw.BaseStream.Position;

            bw.Write(indices.Length);
            bw.Write(unknown);
            foreach(ushort ind in indices) bw.Write(ind);
        }
    }
}
#endif