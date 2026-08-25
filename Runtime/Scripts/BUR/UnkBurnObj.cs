#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.Lighting
{
    using Helper;

    public class UnkBurnObj : MonoBehaviour
    {
        public float unk1, unk2, unk3, unk4, unk5;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Load(BinaryReader br)
        {
            transform.position = br.ReadVector3();
            unk1 = br.ReadSingle();
            unk2 = br.ReadSingle();
            unk3 = br.ReadSingle();
            unk4 = br.ReadSingle();
            unk5 = br.ReadSingle();
        }
    }
}
#endif