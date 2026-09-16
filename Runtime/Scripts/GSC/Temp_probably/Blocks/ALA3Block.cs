#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    public class ALA3Block : GscBlock
    {
        public Unknown2[] unknown2s;

        public override void Load(BinaryReader br)
        {
            int unk1Count = br.ReadInt32();
            int unk0 = br.ReadInt32();
            int[] unk2Ptrs = new int[unk1Count];
            for(int i=0; i<unk1Count; i++) unk2Ptrs[i] = br.ReadInt32();
            int unk2Count = br.ReadInt32();
            for (int i = 0; i < unk2Count; i++) unknown2s[i] = Unknown2.FromBytes(br);
        }

        public override void Save(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override void PostSave(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public struct Unknown2
        {
            float magic;
            ushort vers, sampleCount, frameCount;
            ushort unk1, unk2, unk3, unk4, unk5, unk6, unk7;
            int unkPtr1;
            float unk8, unk9;
            int unkPtr2, unkPtr3, unkPtr4, unkPtr5, unkPtr6, unkPtr7;

            public static Unknown2 FromBytes(BinaryReader br)
            {
                return default;
            }
        }
    }
}
#endif