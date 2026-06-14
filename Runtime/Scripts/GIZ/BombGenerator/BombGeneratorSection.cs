//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.yz4en9shwr1
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System;
    using System.Linq;

    public class BombGeneratorSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.TCS, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 1, TTGame.LB1 => 2, _ => 1 };

        public override string ID => "BombGenerator";

        public static BombGeneratorSection Instance { get; private set; }

        public byte version = 1;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new() { version };

            var bombGens = FindObjectsByType<BombGenerator>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();

            bytes.AddShort((short)bombGens.Length);

            for(int i=0; i<bombGens.Length; i++)
            {
                var bombGen = bombGens[i];

                bytes.AddFixedString(bombGen.name, 16);
                bytes.AddVector3(bombGen.transform.position);
                bytes.AddInt(bombGen.unknown1);
                if (version >= 2) bytes.AddFloat(bombGen.unknown2);

                byte specObjVers = bombGen.specialObjectVersion;
                byte specObjCount = (byte)bombGen.specialObjects.Length;
                bytes.Add(specObjVers);
                bytes.Add(specObjCount);

                for(int j=0; j<specObjCount; j++)
                {
                    var specObj = bombGen.specialObjects[j];
                    bytes.AddString8(specObj.specialObject.specialObject);
                    bytes.AddFloat(specObj.unknown1);
                    bytes.AddFloat(specObj.animationTime);
                    if (specObjVers >= 2) bytes.AddInt(specObj.unknown2);
                    if (version >= 2) bytes.AddShort(specObj.unknown3);
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadByte(ref index);
            short bombGenCount = bytes.ReadShort(ref index);

            //Clear existing bombgens before creating new ones
            foreach (var bombgen in FindObjectsByType<BombGenerator>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) bombgen.gameObject.DelayDestroy();

            for(int i=0; i<bombGenCount; i++)
            {
                var bombGenObj = new GameObject(bytes.ReadString(ref index, 16));
                bombGenObj.transform.SetParent(transform);
                bombGenObj.transform.position = bytes.ReadVector3(ref index);
                var bombGen = bombGenObj.AddComponent<BombGenerator>();

                bombGen.unknown1 = bytes.ReadInt(ref index);
                if (version >= 2) bombGen.unknown2 = bytes.ReadFloat(ref index);

                byte specObjVers = bytes.ReadByte(ref index);
                bombGen.specialObjectVersion = specObjVers;
                byte specObjCount = bytes.ReadByte(ref index);
                bombGen.specialObjects = new BombGenerator.SpecialObject[specObjCount];

                for(int j=0; j<specObjCount; j++)
                {
                    BombGenerator.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject=bytes.ReadString8(ref index) },
                        unknown1 = bytes.ReadFloat(ref index),
                        animationTime = bytes.ReadFloat(ref index),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = bytes.ReadInt(ref index);
                    if (version >= 2) specObj.unknown3 = bytes.ReadShort(ref index);

                    bombGen.specialObjects[j] = specObj;
                }
            }
        }
    }
}
#endif