#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    public class ALA3Block : GscBlock
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