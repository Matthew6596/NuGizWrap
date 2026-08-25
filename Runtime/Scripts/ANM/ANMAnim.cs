#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace NuGizWrap.Animations
{
    using Helper;

    public class ANMAnim : MonoBehaviour
    {
        public string specialObject;
        public int unk3, unk4;
        public float unk5, unk6;
        public Unk1[] unk1s;
        public Unk2[] unk2s;
        public float unk7, unk8, unk9;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Load(BinaryReader br, int version)
        {
            specialObject = br.ReadString(20);
            name = $"anim_{specialObject}";
            unk1s = new Unk1[br.ReadInt32()];
            unk2s = new Unk2[(version >= 2) ? br.ReadInt32() : 0];
            unk3 = br.ReadInt32();
            unk4 = br.ReadInt32();
            unk5 = br.ReadSingle();
            unk6 = br.ReadSingle();

            for(int i=0; i<unk1s.Length; i++)
            {
                string unk1Name = br.ReadString(16);
                int unk1Unk1 = version == 5 ? (int)br.ReadSingle() : br.ReadInt32();
                Unk1 unk1 = new()
                {
                    name = unk1Name,
                    unk1 = unk1Unk1,
                    unk2 = br.ReadInt32(),
                    unk3 = br.ReadVector3(),
                };
                if (version >= 3)
                {
                    unk1.unk4 = br.ReadInt16();
                    unk1.unk5 = br.ReadInt16();
                }
                unk1s[i] = unk1;
            }

            for(int i=0; i<unk2s.Length; i++)
            {
                unk2s[i] = new()
                {
                    name = br.ReadString(16),
                    unk1 = br.ReadInt32(),
                    unk2 = br.ReadSingle(),
                    unk3 = br.ReadVector3(),
                };
            }

            if (version >= 4)
            {
                unk7 = br.ReadSingle();
                unk8 = br.ReadSingle();
                unk9 = br.ReadSingle();
            }
        }

        [Serializable]
        public struct Unk1
        {
            public string name;
            public int unk1, unk2;
            public Vector3 unk3;
            public short unk4, unk5;
        }

        [Serializable]
        public struct Unk2
        {
            public string name;
            public int unk1;
            public float unk2;
            public Vector3 unk3;
        }
    }
}
#endif