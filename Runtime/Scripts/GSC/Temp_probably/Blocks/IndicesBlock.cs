#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    public class IndicesBlock : GscBlock
    {
        public int unknown;
        public short[] indices;

        public override void Load(BinaryReader br)
        {
            int indexCount = br.ReadInt32();
            indices = new short[indexCount];
            for(int i=0; i<indexCount; i++) indices[i] = br.ReadInt16();
        }

        public override void Save(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif