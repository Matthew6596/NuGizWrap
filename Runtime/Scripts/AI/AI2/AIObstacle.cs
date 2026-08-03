#if UNITY_EDITOR
using System.IO;
using TTModdingKit.Helper;
using UnityEngine;

namespace TTModdingKit.AI
{
    public class AIObstacle : MonoBehaviour
    {
        public Vector3 unk102;
        public int unk105;
        public float unk106, unk107;
        public byte unk108, unk109;
        public string specialObj;
        public Vector3 specialObjPos;
        public int unk113;

        public void FromBytes(BinaryReader br, int version)
        {
            transform.position = br.ReadVector3(); //read as 3 floats, might not be vec3, might not be position
            unk102 = br.ReadVector3(); //read as 3 floats, might not be vec3
            if (version >= 15)
            {
                unk105 = br.ReadInt32();
                unk106 = br.ReadSingle();
                unk107 = br.ReadSingle();
                unk108 = br.ReadByte();
            }
            else br.ReadByte(); //padding
            br.ReadBytes(2); //padding
            unk109 = br.ReadByte();
            string specialObj = br.ReadString8();
            if (specialObj.Length != 0)
            {
                specialObjPos = br.ReadVector3();
                if (version >= 15) unk113 = br.ReadInt32();
            }
        }

        public void ToBytes(BinaryWriter bw, int version)
        {
            bw.Write(transform.position);
            bw.Write(unk102);
            if (version >= 15)
            {
                bw.Write(unk105);
                bw.Write(unk106);
                bw.Write(unk107);
                bw.Write(unk108);
            }
            else bw.Write((byte)0); //padding
            bw.Write((short)0); //padding
            bw.Write(unk109);
            bw.WriteString8(specialObj);
            if (specialObj.Length != 0)
            {
                bw.Write(specialObjPos);
                if (version >= 15) bw.Write(unk113);
            }
        }
    }
}
#endif