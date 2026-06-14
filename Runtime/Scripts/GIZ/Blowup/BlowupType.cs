#if UNITY_EDITOR
using UnityEngine;
using System;

namespace TTModdingKit.Gizmos
{
    using GameScene;
    using Helper;

    public class BlowupType : Gizmo
    {
        public SpecialObjectReference specialObject;
        public string parRef1, parRef2;
        public string ptlRef1, ptlRef2, ptlRef3;
        public string unkRef1, unkRef2;
        public string unkRef3, unkRef4;
        public int unknown1, unknown2;
        public byte unknown3;
        public float unknown4;
        public string decal;
        public float unknown5, unknown6;
        public byte unknown7, unknown8;
        public bool nextData;
        public SubDataSet subDataSet;
        public string emitObj1, emitObj2, emitObj3, emitObj4;
        public byte unknown9;
        public float unknown10, unknown11;
        public string shadow, swap;
        public float unknown12, unknown13;
        public string unknown14, unknown15;

        private static Texture2D icon;

        private void OnValidate()
        {
            this.SetIcon(ref icon, "Textures/GizmoIcons/BlowupTypeIcon");
        }

        [Serializable]
        public struct SubDataSet
        {
            public Vector3 unk1;
            public float unk2, unk3, unk4, unk5, unk6;
            public short unk7;
            public byte unk8, unk9;
        }
    }
}
#endif