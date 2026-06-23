#if UNITY_EDITOR
using System.IO;
using TTModdingKit.Helper;
using UnityEngine;

namespace TTModdingKit.AI
{
    public class AIPathRoute : MonoBehaviour
    {
        public string unk33, unk34;
        public string[] unk31;
        public string unk32;
        public string[] specialRouteCharacters;

        public void FromBytes(BinaryReader br, int pointCount)
        {
            name = br.ReadString8();
            if (name.Length != 0)
            {
                byte unk31Count = br.ReadByte();
                byte unk32Length = br.ReadByte();
                br.ReadBytes(2);

                if (pointCount != 0)
                {
                    unk33 = br.ReadString(pointCount);
                    unk34 = br.ReadString(unk31Count);

                    unk31 = new string[unk31Count];
                    for (int k = 0; k < unk31Count; k++) unk31[k] = br.ReadString(unk31Count);
                }

                unk32 = br.ReadString(unk32Length);
            }
            name = name.Trim();

            byte specialRouteCharsCount = br.ReadByte();
            specialRouteCharacters = new string[specialRouteCharsCount];
            for (int k = 0; k < specialRouteCharsCount; k++) specialRouteCharacters[k] = br.ReadString8();
        }

        public void ToBytes(BinaryWriter bw)
        {

        }
    }
}
#endif