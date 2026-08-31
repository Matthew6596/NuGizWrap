#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    public class GSNHBlock : GscBlock
    {
        public override void Load(BinaryReader br)
        {
            int textureIndexListPtr = br.ReadInt32();
            int textureCount = br.ReadInt32();
            int textureMetaPtr = br.ReadInt32();
            int materialListPtr= br.ReadInt32();
        }

        public override void Save(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif