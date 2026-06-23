#if UNITY_EDITOR
using System.IO;
using TTModdingKit.Helper;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.AI
{
    public class Locator : MonoBehaviour
    {
        public byte unk55, unk56;
        public short unk57;
        public float unk58, unk59;
        public int unk60;

        public void FromBytes(BinaryReader br, int version)
        {
            name = br.ReadString(16).Trim();
            transform.position = br.ReadVector3();
            transform.eulerAngles = new(0, ((ushort)br.ReadInt16()).ToFloatAng(), 0);
            unk55 = br.ReadByte();
            unk56 = br.ReadByte();
            unk57 = br.ReadInt16();
            unk58 = br.ReadSingle();
            unk59 = br.ReadSingle();
            if (version >= 15)
            {
                unk60 = br.ReadInt32();
            }
        }

        public void ToBytes(BinaryWriter bw)
        {

        }

        private void OnDrawGizmos()
        {
            Giz.color = TTUnityProject.Prefs.ai2.locatorColor;
            Giz.DrawSphere(transform.position, 0.1f);
        }
    }
}
#endif