#if UNITY_EDITOR
using System.IO;
using TTModdingKit.Helper;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.AI
{
    public class AIPathPoint : MonoBehaviour
    {
        public float xzSize, minY, maxY;
        public byte unk19, unk21;
        public short unk22;
        public byte unk23;
        public string specialObj;
        public Vector3 specialObjPos;
        public short[] unk18;
        public short unk27, unk28;

        public void FromBytes(BinaryReader br, int version)
        {
            name = br.ReadString32().Trim();
            transform.position = br.ReadVector3();
            xzSize = br.ReadSingle();
            if (version >= 8)
            {
                minY = br.ReadSingle();
                maxY = br.ReadSingle();
            }

            byte unk18Count = br.ReadByte();
            unk19 = br.ReadByte();
            br.ReadByte(); //padding
            unk21 = br.ReadByte();
            unk22 = br.ReadInt16();
            unk23 = br.ReadByte(); //padding if version < 19

            specialObj = br.ReadString8();
            if (specialObj.Length != 0) specialObjPos = br.ReadVector3();

            unk18 = new short[unk18Count];
            for (int k = 0; k < unk18Count; k++) unk18[k] = br.ReadInt16();
            if ((unk18Count & 0b00000001) == 1) br.ReadInt16(); //padding if unk18Count odd

            if (version >= 5)
            {
                unk27 = br.ReadInt16();
                unk28 = br.ReadInt16();
            }
        }

        public void ToBytes(BinaryWriter bw)
        {

        }

        private void OnDrawGizmos()
        {
            Giz.color = new(.68f,1,.18f,0.25f);
            Giz.DrawCube(transform.position, new(xzSize, maxY - minY, xzSize));
        }
    }
}
#endif