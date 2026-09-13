#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class VBIBBlock : GscBlock
    {
        public static VBIBBlock Instance { get; private set; }

        public int[] unknown1, unknown2;

        public override void Load(BinaryReader br)
        {
            Instance = this;

            int unk1Count = GSNHBlock.Instance.vbibUnk1Count;
            int unk2Count = GSNHBlock.Instance.vbibUnk2Count;
            unknown1 = new int[unk1Count];
            unknown2 = new int[unk2Count];

            for (int i = 0; i < unk1Count; i++) unknown1[i] = br.ReadInt32();
            for (int i = 0; i < unk2Count; i++) unknown2[i] = br.ReadInt32();
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.VBIBAddress = bw.Pos();

            foreach(var unk1 in unknown1) bw.Write(unk1);
            foreach(var unk2 in unknown2) bw.Write(unk2);
        }
    }
}
#endif