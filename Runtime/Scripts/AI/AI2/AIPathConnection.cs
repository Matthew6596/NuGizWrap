#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace TTModdingKit.AI
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

        public void ToBytes(BinaryWriter bw)
        {

        }
    }
}
#endif