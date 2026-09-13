#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class TREFBlock : GscBlock
    {
        public override void Load(BinaryReader br)
        {
            
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.TREFAddress = bw.Pos();
        }
    }
}
#endif