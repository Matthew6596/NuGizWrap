#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    public class VBIBBlock : GscBlock
    {
        public override void Load(BinaryReader br)
        {
            
        }

        public override void Save(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif