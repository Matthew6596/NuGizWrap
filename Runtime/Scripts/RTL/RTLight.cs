using System;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Lighting
{
    public class RTLight : MonoBehaviour
    {
        //Bit flags? idk
        public enum Option
        {
            DisableFlag=0, CastShadow=1, HasSpecular=2,
            Unknown1=3, UnknownCloudCity=32
        }

        public enum Type
        {
            Invalid=0, Ambient=1, Point=2, PointFlicker=3,
            Directional=4, CameraDirection=5, PointBlend=6, AntiLight=7,
            Jonflicker=8, Cnt=9
        }

        public Color color, highColor, flickerColor;
        public float falloff;
        public float flickerHighTime, flickerLowTime;
        public float flickerRandomHighTime, flickerRandomLowTime;
        public float flickerTimer;
        public Option option;
        public Type type;

        public short unk1, unk2, unk3;
        public int unk4;
        public short unk5, unk6;
        public int unk7;
        public float multiplier;
        public int unk8;

        public byte[] unk9;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDrawGizmosSelected()
        {
            Giz.color = new(color.r, color.g, color.b, 0.1f);
            Vector3 pos = transform.position;
            Giz.DrawSphere(pos, transform.localScale.x);
            Giz.DrawWireSphere(pos, falloff);
        }
    }
}