#if UNITY_EDITOR
using System.IO;
using TTModdingKit.Helper;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.AI
{
    public class Trigger : MonoBehaviour
    {
        public Vector3 halfSize;
        public byte unk49;

        public void FromBytes(BinaryReader br, int version)
        {
            name = br.ReadString(16).Trim();
            transform.position = br.ReadVector3();

            halfSize = br.ReadVector3(); //read as 3 floats, might not be vec3

            ushort ang = (ushort)br.ReadInt16();
            transform.eulerAngles = new(0, ang.ToFloatAng(), 0);

            unk49 = br.ReadByte();
            //byte unk49 = br.ReadByte();
            //if (version >= 20) this.unk49 = unk49;

            br.ReadByte(); //padding
        }

        public void ToBytes(BinaryWriter bw)
        {

        }

        private void OnDrawGizmos()
        {
            Giz.color = TTUnityProject.Prefs.ai2.triggerColor;
            Giz.DrawCube(transform.position, halfSize*2);
        }
    }
}
#endif