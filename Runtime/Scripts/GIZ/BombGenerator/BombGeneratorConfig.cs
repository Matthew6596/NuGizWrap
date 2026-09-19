#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;

    public class BombGeneratorConfig : GizmoTypeConfig
    {
        public override string ID => "BombGenerator";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.TCS, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 1, TTGame.LB1 => 2, _ => 1 };

        public int version;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadByte();
            short bombGenCount = br.ReadInt16();

            List<string> existingNames = new();
            for (int i = 0; i < bombGenCount; i++)
            {
                string name = br.ReadString(16);
                name = ObjectNames.GetUniqueName(existingNames.ToArray(), name);
                existingNames.Add(name);

                var bombGenObj = new GameObject(name);
                bombGenObj.transform.SetParent(parent);
                bombGenObj.transform.position = br.ReadVector3();
                var bombGen = bombGenObj.AddComponent<BombGenerator>();

                bombGen.unknown1 = br.ReadInt32();
                if (version >= 2) bombGen.unknown2 = br.ReadSingle();

                byte specObjVers = br.ReadByte();
                bombGen.specialObjectVersion = specObjVers;
                byte specObjCount = br.ReadByte();
                bombGen.specialObjects = new BombGenerator.SpecialObject[specObjCount];

                for (int j = 0; j < specObjCount; j++)
                {
                    BombGenerator.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = br.ReadString8() },
                        unknown1 = br.ReadSingle(),
                        animationTime = br.ReadSingle(),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = br.ReadInt32();
                    if (version >= 2) specObj.unknown3 = br.ReadInt16();

                    bombGen.specialObjects[j] = specObj;
                }
            }

            return bombGenCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write((byte)version);

            var bombGens = FindObjectsByType<BombGenerator>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();

            bw.Write((short)bombGens.Length);

            for (int i = 0; i < bombGens.Length; i++)
            {
                var bombGen = bombGens[i];

                bw.WriteString(bombGen.name, 16);
                bw.Write(bombGen.transform.position);
                bw.Write(bombGen.unknown1);
                if (version >= 2) bw.Write(bombGen.unknown2);

                byte specObjVers = bombGen.specialObjectVersion;
                byte specObjCount = (byte)bombGen.specialObjects.Length;
                bw.Write(specObjVers);
                bw.Write(specObjCount);

                for (int j = 0; j < specObjCount; j++)
                {
                    var specObj = bombGen.specialObjects[j];
                    bw.WriteString8(specObj.specialObject.specialObject);
                    bw.Write(specObj.unknown1);
                    bw.Write(specObj.animationTime);
                    if (specObjVers >= 2) bw.Write(specObj.unknown2);
                    if (version >= 2) bw.Write(specObj.unknown3);
                }
            }
        }
    }
}
#endif