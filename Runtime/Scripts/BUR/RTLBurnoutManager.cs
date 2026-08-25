#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace NuGizWrap.Lighting
{
    using Helper;

    public class RTLBurnoutManager : MonoBehaviour
    {
        public int version;
        public float unk1, unk2, unk3;
        public float unk4 = 1, unk5 = 4, unk6 = 0.2f, unk7 = 0.5f, unk8 = 1.9f, unk9 = 0;
        public float unk10, unk11, unk12, unk13, unk14;
        public int unk15;
        public float unk16, unk17, unk18, unk19, unk20, unk21, unk22, unk23;

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
            version = br.ReadInt32();
            int unkBurnObjCount = br.ReadInt32();
            unk1 = br.ReadSingle();
            unk2 = br.ReadSingle();
            unk3 = br.ReadSingle();

            if(version >= 2)
            {
                unk4 = br.ReadSingle();
                unk5 = br.ReadSingle();
                unk6 = br.ReadSingle();
                unk7 = br.ReadSingle();
                unk8 = br.ReadSingle();
                unk9 = br.ReadSingle();
            }

            for(int i=0; i<unkBurnObjCount; i++)
            {
                UnkBurnObj unkBurn = new GameObject($"unk_burn_{i}").AddComponent<UnkBurnObj>();
                unkBurn.transform.SetParent(transform);
                unkBurn.Load(br);
            }

            unk10 = br.ReadSingle();
            unk11 = br.ReadSingle();
            unk12 = br.ReadSingle();
            unk13 = br.ReadSingle();
            unk14 = br.ReadSingle();
            unk15 = br.ReadInt32();
            unk16 = br.ReadSingle();
            unk17 = br.ReadSingle();
            unk18 = br.ReadSingle();
            unk19 = br.ReadSingle();
            unk20 = br.ReadSingle();
            unk21 = br.ReadSingle();
            unk22 = br.ReadSingle();
            unk23 = br.ReadSingle();
        }
    }
}
#endif