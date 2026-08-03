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
            //if (characterType != string.Empty) characterType = characterType[..characterType.IndexOf(' ')];
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

        public void ToBytes(BinaryWriter bw, int version)
        {
            bw.WriteString(name, 16);
            bw.WriteString(scriptName, 16);
            bw.WriteString(characterType, version >= 14 ? 32 : 16);
            if (version >= 21) bw.WriteString8(itemName);
            bw.Write(transform.position);
            bw.Write(transform.eulerAngles.y.ToShortAng());

            if (version >= 16) bw.Write(unk72);
            bw.Write(unk73);
            bw.Write(unk74);
            bw.Write(unk75);
            bw.Write(unk76);
            bw.Write(unk77);
            bw.Write(unk78);
            bw.Write(unk79);
            bw.Write(unk80);
            bw.Write(unk81);

            if (version >= 3)
            {
                bw.Write(unk82);
                bw.Write(unk83);
                bw.Write(unk84);
                bw.Write(unk85);
            }

            if (version >= 4)
            {
                if (trigger1Ref != string.Empty)
                {
                    bw.Write(1);
                    bw.WriteString(trigger1Ref, 16);
                }
                else bw.Write(0);
            }

            if (version >= 6)
            {
                if (locator1Ref != string.Empty)
                {
                    bw.Write(1);
                    bw.WriteString(locator1Ref, 16);
                }
                else bw.Write(0);
            }

            if (version >= 17)
            {
                if (locator2Ref != string.Empty)
                {
                    bw.Write(1);
                    bw.WriteString(locator2Ref, 16);
                }
                else bw.Write(0);
            }

            if (version >= 8)
            {
                bw.Write(activateDifficulty);
                bw.Write(minNumRespawns);
                bw.Write(maxNumRespawns);
                bool trig2Exists = trigger2Ref != string.Empty;
                bw.Write((byte)(trig2Exists ? 1 : 0));
                bw.Write(minRespawnTime);
                bw.Write(maxRespawnTime);
                if (version >= 10) bw.Write(staggerStart);
                if (trig2Exists) bw.WriteString(trigger2Ref, 16);
                if (version >= 11)
                {
                    bw.Write(viewRange);
                    bw.Write(hearDistance);
                    bw.Write(maxViewHeight);
                    bw.Write(unk97);
                    bw.Write(0); //padding
                }
            }
        }
    }
}
#endif