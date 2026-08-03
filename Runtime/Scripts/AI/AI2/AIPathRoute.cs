#if UNITY_EDITOR
using System.IO;
using TTModdingKit.Helper;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace TTModdingKit.AI
{
    public class AIPathRoute : MonoBehaviour
    {
        public string unk33, unk34;
        public string[] unk31;
        public string unk32;
        public string[] specialRouteCharacters;

        private void OnValidate()
        {
            if (unk31 != null && unk31.Length > 255)
            {
                unk31 = unk31.Take(255).ToArray();
                EditorUtility.DisplayDialog("Max Unk31", "AIPathRoute can only have a maximum of 255 Unk31.", "OK");
            }

            if (specialRouteCharacters != null && specialRouteCharacters.Length > 255)
            {
                specialRouteCharacters = specialRouteCharacters.Take(255).ToArray();
                EditorUtility.DisplayDialog("Max Special Route Characters", "AIPathRoute can only have a maximum of 255 special route characters.", "OK");
            }
        }

        public void FromBytes(BinaryReader br, int pointCount)
        {
            name = br.ReadString8();
            if (name.Length != 0)
            {
                byte unk31Count = br.ReadByte();
                byte unk32Length = br.ReadByte();
                br.ReadBytes(2); //padding

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

        public void ToBytes(BinaryWriter bw, int pointCount)
        {
            bw.WriteString8(name);
            if (name.Length != 0)
            {
                byte unk31Count = (byte)unk31.Length;
                byte unk32Length = (byte)(unk32.Length + 1);
                bw.Write(unk31Count);
                bw.Write((byte)(unk32Length));
                bw.Write((short)0); //padding

                if (pointCount != 0)
                {
                    bw.WriteString(unk33, pointCount);
                    bw.WriteString(unk34, unk31Count);

                    for (int i = 0; i < unk31Count; i++) bw.WriteString(unk31[i], unk31Count);
                }

                bw.WriteString(unk32, unk32Length);
            }

            byte specRouteCharCount = (byte)specialRouteCharacters.Length;
            bw.Write(specRouteCharCount);
            for (int i = 0; i < specRouteCharCount; i++) bw.WriteString8(specialRouteCharacters[i]);
        }
    }
}
#endif