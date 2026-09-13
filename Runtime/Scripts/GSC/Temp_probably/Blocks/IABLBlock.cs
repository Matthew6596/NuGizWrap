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
        public IABLObject[] unknown1s;

        public override void Load(BinaryReader br)
        {
            int unk1Count = br.ReadInt32();
            unknown1s = new IABLObject[unk1Count];
            unknown2 = br.ReadInt32();
            for(int i=0; i<unk1Count; i++)
            {
                unknown1s[i] = IABLObject.FromBytes(br);
            }
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.IABLAddress = bw.Pos();

            bw.Write(unknown1s.Length);
            bw.Write(unknown2);

            for(int i=0; i<unknown1s.Length; i++)
            {
                var unk1 = unknown1s[i];
                bw.Write(unk1.matrix);
                bw.Write(unk1.unk1);
                bw.Write(unk1.unk2);
                bw.Write(unk1.unk3);
                bw.Write(unk1.unk4);
                bw.Write(unk1.unk5);
                bw.Write(unk1.unk6);
                bw.Write(unk1.unk7);
                bw.Write(unk1.unk8);
                bw.Write(unk1.unk9);
            }
        }

        [Serializable]
        public struct IABLObject
        {
            public Matrix4x4 matrix;
            public float unk1, unk2, unk3, unk4;
            public int unk5, unk6, unk7;
            public short unk8, unk9;

            public static IABLObject FromBytes(BinaryReader br)
            {
                return new()
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
    }
}
#endif