#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class SplineBlock : GscBlock
    {
        public Spline[] splines;

        private int splineDataSize;

        public override void Load(BinaryReader br)
        {
            int splineCount = br.ReadInt32();
            splines = new Spline[splineCount];
            splineDataSize = br.ReadInt32();

            for(int i=0; i<splineCount; i++)
            {
                splines[i] = Spline.FromBytes(br);
            }
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.SST0Address = bw.Pos();

            bw.Write(splines.Length);
            bw.Write(0); //write spline data size later

            splineDataSize = 0;
            foreach(var spline in splines)
            {
                int pointCount = spline.points.Length;
                bw.Write((short)pointCount);
                bw.Write(spline.unk);

                PointerBlock.WritePlaceholdPtr(bw);
                bw.Write(0); //resolve name ptr later

                foreach(var p in spline.points) bw.Write(p);

                splineDataSize = 8 + pointCount * 12;
            }
        }

        public override void PostSave(BinaryWriter bw)
        {
            bw.BaseStream.Seek(4, SeekOrigin.Current); //skip spline count
            bw.Write(splineDataSize);
            for(int i=0; i<splines.Length; i++)
            {
                var spline = splines[i];
                bw.BaseStream.Seek(8, SeekOrigin.Current); //skip point count / unknown
                long nameAddr = NameTableBlock.Instance.GetPtrAddress(spline.name);
                if(TTUnityProject.Game == TTGame.TCS) bw.Write(nameAddr + 0xED5BFFDC);
                else { Debug.LogError("LIJ1/LB1 not supported yet, failed to calculate name pointer in SplineBlock"); break; }
                bw.BaseStream.Seek(12 * spline.points.Length, SeekOrigin.Current); //skip points
            }
        }

        [Serializable]
        public struct Spline
        {
            public short unk;
            public string name;
            public Vector3[] points;

            public static Spline FromBytes(BinaryReader br)
            {
                int pointCount = br.ReadInt16();
                Vector3[] points = new Vector3[pointCount];

                Spline spline = new()
                {
                    unk = br.ReadInt16(),
                };

                if (TTUnityProject.Game == TTGame.TCS)
                {
                    int namePtr = (int)(br.ReadUInt32() - 0xED5BFFDC) + 4;
                    long pos = br.BaseStream.Position + namePtr;
                    spline.name = NameTableBlock.GetName(pos);
                }

                for (int j = 0; j < pointCount; j++)
                {
                    points[j] = br.ReadVector3();
                }
                spline.points = points;

                return spline;
            }
        }
    }
}
#endif