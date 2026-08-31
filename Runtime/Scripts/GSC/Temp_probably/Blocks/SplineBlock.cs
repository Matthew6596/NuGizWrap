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

        public override void Load(BinaryReader br)
        {
            int splineCount = br.ReadInt32();
            splines = new Spline[splineCount];
            int splineDataSize = br.ReadInt32();

            for(int i=0; i<splineCount; i++)
            {
                int pointCount = br.ReadInt16();
                Vector3[] points = new Vector3[pointCount];

                Spline spline = new()
                {
                    unk = br.ReadInt16(),
                };

                if(TTUnityProject.Game == TTGame.TCS)
                {
                    int namePtr = (int)(br.ReadUInt32() - 0xED5BFFDC) + 4;
                    long pos = br.BaseStream.Position;
                    br.BaseStream.Seek(namePtr, SeekOrigin.Current);
                    spline.name = NameTableBlock.LoadStr(br);
                    br.BaseStream.Position = pos;
                }

                for(int j=0; j<pointCount; j++)
                {
                    points[j] = br.ReadVector3();
                }
                spline.points = points;

                splines[i] = spline;
            }
        }

        public override void Save(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override void CalculatePointers(BinaryWriter bw)
        {
            bw.BaseStream.Seek(8, SeekOrigin.Current); //skip spline count / spline data size
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
        }
    }
}
#endif