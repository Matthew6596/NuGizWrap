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

    public class GizDigConfig : GizmoTypeConfig
    {
        public override string ID => "GizDig";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 19, _ => 1 };

        public int version;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadByte();
            short digCount = br.ReadInt16();

            List<string> existingNames = new();
            for (int i = 0; i < digCount; i++)
            {
                string digName = br.ReadString(16);

                digName = ObjectNames.GetUniqueName(existingNames.ToArray(), digName);
                existingNames.Add(digName);

                GameObject digObj = new(digName);
                digObj.transform.SetParent(parent);
                digObj.transform.position = br.ReadVector3();
                var dig = digObj.AddComponent<GizDig>();

                if (version >= 17) dig.unknown1 = br.ReadVector3();
                dig.unknown2 = br.ReadSingle();
                dig.interactionOptions = (GizDig.InteractionOptions)br.ReadInt32();

                byte specObjVers = br.ReadByte();
                dig.specialObjectVersion = specObjVers;
                byte specObjCount = br.ReadByte();
                dig.specialObjects = new GizDig.SpecialObject[specObjCount];

                for (int j = 0; j < specObjCount; j++)
                {
                    GizDig.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = br.ReadString8() },
                        unknown1 = br.ReadSingle(),
                        animationTime = br.ReadSingle(),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = br.ReadInt32();
                    specObj.unknown3 = br.ReadInt16();

                    dig.specialObjects[j] = specObj;
                }

                dig.animSpeed = br.ReadSingle();
                dig.animAdvanceAmount = br.ReadSingle();

                dig.blowup = new() { blowupName = br.ReadString8() };

                dig.studsValue = br.ReadUInt16();

                GameObject spawnObj = new("studs_spawn_transform");
                spawnObj.transform.SetParent(digObj.transform);
                dig.studsSpawn = spawnObj.transform;

                dig.studsSpawn.eulerAngles = br.ReadXYEuler();
                dig.studsSpawn.localPosition = br.ReadVector3();

                dig.studsSpawnSpeed = br.ReadSingle();

                dig.unknownSfx = new() { sample = br.ReadString8() };

                dig.numSteps = br.ReadInt16();
                dig.unknown7 = br.ReadInt16();
                if (version >= 18) dig.tool = (GizDig.Tool)br.ReadInt16();
            }

            return digCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write((byte)version);

            var digs = FindObjectsByType<GizDig>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            short digCount = (short)digs.Length;
            bw.Write(digCount);

            for (int i = 0; i < digCount; i++)
            {
                var dig = digs[i];

                bw.WriteString(dig.name, 16);
                bw.Write(dig.transform.position);
                if (version >= 17) bw.Write(dig.unknown1);
                bw.Write(dig.unknown2);
                bw.Write((int)dig.interactionOptions);

                byte specObjVers = dig.specialObjectVersion;
                bw.Write(specObjVers);
                byte specObjCount = (byte)dig.specialObjects.Length;
                bw.Write(specObjCount);

                for (int j = 0; j < specObjCount; j++)
                {
                    var specObj = dig.specialObjects[j];
                    bw.WriteString8(specObj.specialObject.specialObject);
                    bw.Write(specObj.unknown1);
                    bw.Write(specObj.animationTime);
                    if (specObjVers >= 2) bw.Write(specObj.unknown2);
                    bw.Write(specObj.unknown3);
                }

                bw.Write(dig.animSpeed);
                bw.Write(dig.animAdvanceAmount);

                bw.WriteString8(dig.blowup.GetBlowup());

                bw.Write(dig.studsValue);

                if (dig.studsSpawn == null)
                {
                    bw.Write((short)0);
                    bw.Write((short)0);
                    bw.Write(Vector3.zero);
                }
                else
                {
                    Vector3 euler = dig.studsSpawn.eulerAngles;
                    bw.Write(euler.x.ToShortAng());
                    bw.Write(euler.y.ToShortAng());
                    bw.Write(dig.studsSpawn.position - dig.transform.position);
                }

                bw.Write(dig.studsSpawnSpeed);

                bw.WriteString8(dig.unknownSfx.sample);

                bw.Write(dig.numSteps);
                bw.Write(dig.unknown7);
                if (version >= 18) bw.Write((short)dig.tool);
            }
        }
    }
}
#endif