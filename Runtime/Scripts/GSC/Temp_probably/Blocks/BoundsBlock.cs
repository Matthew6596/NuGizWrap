#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class BoundsBlock : GscBlock
    {
        public int boundsFlags;
        public int unknown1, unknown2;

        public UnkBounds[] unkBounds;
        public Bounds[] bounds;

        public override void Load(BinaryReader br)
        {
            boundsFlags = br.ReadInt32();
            int boundsCount = br.ReadInt32();
            if((boundsFlags & 0x80000000) == 0) //Bit32 Clear
            {
                unknown1 = br.ReadInt32();
                unknown2 = br.ReadInt32();
                unkBounds = new UnkBounds[boundsCount];
                for(int i=0; i<boundsCount; i++)
                {
                    unkBounds[i] = new UnkBounds()
                    {
                        unk1 = br.ReadInt32(),
                        unk2 = br.ReadInt32(),
                        unk3 = br.ReadInt32(),
                        unk4 = br.ReadInt32(),
                    };
                }
            }

            bounds = new Bounds[boundsCount];
            for (int i = 0; i < boundsCount; i++)
            {
                bounds[i] = new Bounds()
                {
                    position = br.ReadVector3(),
                    unk1 = br.ReadInt32(),
                    size = br.ReadVector3(),
                    unk2 = br.ReadInt32(),
                };
            }
        }

        public override void Save(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        [Serializable]
        public struct UnkBounds
        {
            public float unk1, unk2, unk3, unk4;
        }

        [Serializable]
        public struct Bounds
        {
            public Vector3 position, size;
            public float unk1, unk2;
        }
    }
}
#endif