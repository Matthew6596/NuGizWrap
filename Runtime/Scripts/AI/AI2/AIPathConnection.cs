#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.AI
{
    public class AIPathConnection : MonoBehaviour
    {
        public byte unk5, unk6;
        public int unk7, unk8;
        public short unk9, unk10;
        public float unk11, unk12;

        public void FromBytes(BinaryReader br, int version)
        {
            unk5 = br.ReadByte();
            unk6 = br.ReadByte();
            if (version >= 12) (unk7, unk8) = (br.ReadInt32(), br.ReadInt32());
            else if (version >= 9) (unk7, unk8) = (br.ReadInt16(), br.ReadInt16());
            else (unk7, unk8) = (br.ReadByte(), br.ReadByte());
            unk9 = br.ReadInt16();
            unk10 = br.ReadInt16();
            unk11 = br.ReadSingle();
            unk12 = br.ReadSingle();
        }

        public void ToBytes(BinaryWriter bw, int version)
        {
            bw.Write(unk5);
            bw.Write(unk6);
            if (version >= 12)
            {
                bw.Write((int)unk7);
                bw.Write((int)unk8);
            }
            else if(version >= 9)
            {
                bw.Write((short)unk7);
                bw.Write((short)unk8);
            }
            else
            {
                bw.Write((byte)unk7);
                bw.Write((byte)unk8);
            }
            bw.Write(unk9);
            bw.Write(unk10);
            bw.Write(unk11);
            bw.Write(unk12);
        }
    }
}
#endif