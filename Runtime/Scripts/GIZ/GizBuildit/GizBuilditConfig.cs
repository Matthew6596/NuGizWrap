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

    public class GizBuilditConfig : GizmoTypeConfig
    {
        public override string ID => "GizBuildit";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 9, TTGame.LIJ1 => 9, TTGame.LB1 => 10, _ => 1 };

        public int version;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadByte();
            short builditCount = br.ReadInt16();

            List<string> existingNames = new();
            for (int i = 0; i < builditCount; i++)
            {
                string buildName = br.ReadString(16);

                buildName = ObjectNames.GetUniqueName(existingNames.ToArray(), buildName);
                existingNames.Add(buildName);

                GameObject buildObj = new(buildName);
                buildObj.transform.SetParent(parent);
                buildObj.transform.position = br.ReadVector3();
                var buildit = buildObj.AddComponent<GizBuildit>();

                byte specObjVers = br.ReadByte();
                byte specObjCount = br.ReadByte();
                buildit.specialObjectVersion = specObjVers;
                buildit.specialObjects = new GizBuildit.SpecialObject[specObjCount];

                for (int j = 0; j < specObjCount; j++)
                {
                    GizBuildit.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = br.ReadString8() },
                        unknown1 = br.ReadSingle(),
                        animationTime = br.ReadSingle(),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = br.ReadInt32();

                    buildit.specialObjects[j] = specObj;
                }

                buildit.jumpIntensity = br.ReadSingle();
                if (version <= 6) br.ReadInt32(); //padding
                buildit.minStuds = br.ReadUInt16();
                buildit.maxStuds = br.ReadUInt16();
                buildit.unknown2 = br.ReadByte();
                buildit.unknown3 = br.ReadByte();
                if (version >= 10) buildit.unknown10 = br.ReadSingle();
                if (version >= 6) buildit.unknown4 = br.ReadSingle();
                if (version == 7)
                {
                    short blowupId = br.ReadInt16();
                    Debug.LogWarning($"Cannot load blowup via nametable ID ({blowupId}), blowup on GizBuildit '{name}' will be null");
                }
                if (version >= 8) buildit.blowupType.SetBlowupType(br.ReadString8());
                if (version >= 7)
                {
                    if (buildit.studsSpawn == null)
                    {
                        GameObject studSpawnObj = new("studs_spawn");
                        studSpawnObj.transform.SetParent(buildit.transform);
                        buildit.studsSpawn = studSpawnObj.transform;
                    }
                    buildit.studsSpawn.eulerAngles = br.ReadXYEuler();
                    buildit.studsSpawn.localPosition = br.ReadVector3();
                }
                if (version >= 9) buildit.studsSpawnSpeed = br.ReadSingle();
                if (version >= 4) buildit.unknown7 = br.ReadInt16();
                if (version >= 5)
                {
                    buildit.unknown8 = br.ReadInt16();
                    if (br.ReadByte() != 0) buildit.unknown9 = br.ReadString(16);
                }
            }

            return builditCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write((byte)version);

            var buildits = FindObjectsByType<GizBuildit>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            short builditCount = (short)buildits.Length;
            bw.Write(builditCount);

            for (int i = 0; i < builditCount; i++)
            {
                var buildit = buildits[i];
                bw.WriteString(buildit.name, 16);
                bw.Write(buildit.transform.position);

                byte specObjVers = buildit.specialObjectVersion;
                byte specObjCount = (byte)buildit.specialObjects.Length;
                bw.Write(specObjVers);
                bw.Write(specObjCount);

                for (int j = 0; j < specObjCount; j++)
                {
                    var specObj = buildit.specialObjects[j];
                    bw.WriteString8(specObj.specialObject.specialObject);
                    bw.Write(specObj.unknown1);
                    bw.Write(specObj.animationTime);
                    if (specObjVers >= 2) bw.Write(specObj.unknown2);
                }

                bw.Write(buildit.jumpIntensity);
                if (version <= 6) bw.Write(0); //padding
                bw.Write(buildit.minStuds);
                bw.Write(buildit.maxStuds);
                bw.Write(buildit.unknown2);
                bw.Write(buildit.unknown3);
                if (version >= 10) bw.Write(buildit.unknown10);
                if (version >= 6) bw.Write(buildit.unknown4);
                if (version == 7)
                {
                    bw.Write((short)0);
                    Debug.LogWarning($"Cannot export blowup by nametable ID, blowup on GizBuildit '{name}' will be exported as 0");
                }
                if (version >= 8) bw.WriteString8(buildit.blowupType.GetBlowupType());
                if (version >= 7)
                {
                    if (buildit.studsSpawn == null)
                    {
                        bw.Write((short)0);
                        bw.Write((short)0);
                        bw.Write(Vector3.zero);
                    }
                    else
                    {
                        Vector3 euler = buildit.studsSpawn.eulerAngles;
                        bw.Write(euler.x.ToShortAng());
                        bw.Write(euler.y.ToShortAng());
                        bw.Write(buildit.StudsSpawnPos);
                    }
                }
                if (version >= 9) bw.Write(buildit.studsSpawnSpeed);
                if (version >= 4) bw.Write(buildit.unknown7);
                if (version >= 5)
                {
                    bw.Write(buildit.unknown8);
                    bool hasBuildit = buildit.unknown9 != null && buildit.unknown9.Trim().Length > 0;
                    bw.Write((byte)(hasBuildit ? 1 : 0));
                    if (hasBuildit) bw.WriteString(buildit.unknown9, 16);
                }
            }
        }
    }
}
#endif