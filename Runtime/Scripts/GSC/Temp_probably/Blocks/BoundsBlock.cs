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
            GSCExporter.BNDSAddress = bw.Pos();

            bw.Write(boundsFlags);
            bw.Write(bounds.Length);
            if (!boundsFlags.IsBitSet(32))
            {
                if(unkBounds.Length != bounds.Length)
                {
                    throw new DataMisalignedException("unkBounds and bounds must be the same size. Alternatively, avoid exporting unkBounds by setting bit 32 in boundsFlags.");
                }

                bw.Write(unknown1);
                bw.Write(unknown2);
                foreach(var unkBound in unkBounds)
                {
                    bw.Write(unkBound.unk1);
                    bw.Write(unkBound.unk2);
                    bw.Write(unkBound.unk3);
                    bw.Write(unkBound.unk4);
                }
            }

            foreach(var bound in bounds)
            {
                bw.Write(bound.position);
                bw.Write(bound.unk1);
                bw.Write(bound.size);
                bw.Write(bound.unk2);
            }
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