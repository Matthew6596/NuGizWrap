#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class DYNOBlock : GscBlock
    {
        public override void Load(BinaryReader br)
        {
            
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.DYNOAddress = bw.Pos();

            //Writing a default DYNO Block (taken from negotations_a_pc.gsc in TCS)
            bw.Write(0);
            bw.Write(0);
            bw.Write(11);
            bw.Write(0);
            bw.Write(0);
            bw.Write(0xfefefefe);
            for(int i=0; i<6; i++)
            {
                bw.Write(0);
                bw.Write(0);
                bw.Write(0xfefefefe);
                bw.Write(0xfefefefe);
            }
        }
    }
}
#endif