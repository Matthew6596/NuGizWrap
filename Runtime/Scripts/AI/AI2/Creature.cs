#if UNITY_EDITOR
using System.IO;
using TTModdingKit.Helper;
using UnityEngine;

namespace TTModdingKit.AI
{
    public class Creature : MonoBehaviour
    {
        public string scriptName, characterType, itemName;

        public byte unk72, unk73, unk74;
        public int unk75;
        public float unk76, unk77;
        public int unk78;
        public byte unk79, unk80;
        public short unk81;
        public float unk82, unk83, unk84, unk85;
        public string trigger1Ref, trigger2Ref, locator1Ref, locator2Ref;
        public byte activateDifficulty, minNumRespawns, maxNumRespawns;
        public float minRespawnTime, maxRespawnTime, staggerStart, viewRange, hearDistance, maxViewHeight;
        public int unk97;

        public void FromBytes(BinaryReader br, int version)
        {
            name = br.ReadString(16).Trim(); //character name
            scriptName = br.ReadString(16).Trim();
            characterType = br.ReadString(version >= 14 ? 32 : 16).Trim();
            if (characterType != string.Empty) characterType = characterType[..characterType.IndexOf(' ')];
            if (version >= 21) itemName = br.ReadString8().Trim();

            transform.position = br.ReadVector3(); //start position
            transform.eulerAngles = new(0, ((ushort)br.ReadInt16()).ToFloatAng(), 0); //start angle

            if (version >= 16)
            {
                unk72 = br.ReadByte();
            }
            unk73 = br.ReadByte();
            unk74 = br.ReadByte();
            unk75 = br.ReadInt32();
            unk76 = br.ReadSingle();
            unk77 = br.ReadSingle();
            unk78 = br.ReadInt32();
            unk79 = br.ReadByte();
            unk80 = br.ReadByte();
            unk81 = br.ReadInt16();
            if (version >= 3)
            {
                unk82 = br.ReadSingle();
                unk83 = br.ReadSingle();
                unk84 = br.ReadSingle();
                unk85 = br.ReadSingle();
            }
            if (version >= 4)
            {
                int unk86 = br.ReadInt32();
                if (unk86 != 0) trigger1Ref = br.ReadString(16);
            }
            if (version >= 6)
            {
                int unk88 = br.ReadInt32();
                if (unk88 != 0) locator1Ref = br.ReadString(16);
            }
            if (version >= 17)
            {
                int unk90 = br.ReadInt32();
                if (unk90 != 0) locator2Ref = br.ReadString(16);
            }

            if (version >= 8)
            {
                activateDifficulty = br.ReadByte();
                minNumRespawns = br.ReadByte();
                maxNumRespawns = br.ReadByte();
                byte unk95 = br.ReadByte();
                minRespawnTime = br.ReadSingle();
                maxRespawnTime = br.ReadSingle();
                if (version >= 10)
                {
                    staggerStart = br.ReadSingle();
                }
                if (unk95 == 1) trigger2Ref = br.ReadString(16);
                if (version >= 11)
                {
                    viewRange = br.ReadSingle();
                    hearDistance = br.ReadSingle();
                    maxViewHeight = br.ReadSingle();
                    unk97 = br.ReadInt32();
                    br.ReadInt32(); //padding
                }
            }
        }

        public void ToBytes(BinaryWriter bw)
        {

        }
    }
}
#endif