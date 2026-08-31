#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class IABLBlock : GscBlock
    {
        public int unknown2;
        public Unknown1[] unknown1s;

        public override void Load(BinaryReader br)
        {
            int unk1Count = br.ReadInt32();
            unknown1s = new Unknown1[unk1Count];
            unknown2 = br.ReadInt32();
            for(int i=0; i<unk1Count; i++)
            {
                unknown1s[i] = new()
                {
                    matrix = br.ReadM4x4(),
                    unk1 = br.ReadSingle(),
                    unk2 = br.ReadSingle(),
                    unk3 = br.ReadSingle(),
                    unk4 = br.ReadSingle(),
                    unk5 = br.ReadInt32(),
                    unk6 = br.ReadInt32(),
                    unk7 = br.ReadInt32(),
                    unk8 = br.ReadInt16(),
                    unk9 = br.ReadInt16(),
                };
            }
        }

        public override void Save(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        [Serializable]
        public struct Unknown1
        {
            public Matrix4x4 matrix;
            public float unk1, unk2, unk3, unk4;
            public int unk5, unk6, unk7;
            public short unk8, unk9;
        }
    }
}
#endif