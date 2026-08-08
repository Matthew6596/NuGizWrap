#if UNITY_EDITOR
using System.IO;
using NuGizWrap.Helper;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.AI
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

        private void OnValidate()
        {
            if (unk18 != null && unk18.Length > 255)
            {
                unk18 = unk18.Take(255).ToArray();
                EditorUtility.DisplayDialog("Max Unk18", "AIPathPoint can only have a maximum of 255 Unk18.", "OK");
            }
        }

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

        public void ToBytes(BinaryWriter bw, int version)
        {
            bw.WriteString32(name);
            bw.Write(transform.position);
            bw.Write(xzSize);
            if (version >= 8)
            {
                bw.Write(minY);
                bw.Write(maxY);
            }

            byte unk18Count = (byte)unk18.Length;
            bw.Write(unk18Count);

            bw.Write(unk19);
            bw.Write((byte)0); //padding
            bw.Write(unk21);
            bw.Write(unk22);
            bw.Write(unk23);

            bw.WriteString8(specialObj);
            if (specialObj.Length > 0) bw.Write(specialObjPos);

            for(int i=0; i<unk18Count; i++) bw.Write(unk18[i]);
            if ((unk18Count & 0b00000001) == 1) bw.Write((short)0); //padding if unk18Count odd

            if (version >= 5)
            {
                bw.Write(unk27);
                bw.Write(unk28);
            }
        }

        private void OnDrawGizmos()
        {
            Giz.color = new(.68f,1,.18f,0.25f);
            Giz.DrawCube(transform.position, new(xzSize, maxY - minY, xzSize));
        }
    }
}
#endif