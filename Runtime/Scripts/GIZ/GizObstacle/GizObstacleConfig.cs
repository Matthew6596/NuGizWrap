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

    public class GizObstacleConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 14, TTGame.LIJ1 => 19, TTGame.LB1 => 20, _ => 1 };

        public int version;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadByte();
            short obsCount = br.ReadInt16();

            string[] existingNames = new string[obsCount];
            for (int i = 0; i < obsCount; i++)
            {
                string obsName = br.ReadString(16);

                obsName = ObjectNames.GetUniqueName(existingNames, obsName);
                existingNames[i] = obsName;

                GameObject obsObj = new(obsName);
                obsObj.transform.SetParent(parent);
                obsObj.transform.position = br.ReadVector3();
                var obs = obsObj.AddComponent<GizObstacle>();

                Transform triggerTransform = new GameObject("trigger_transform").transform;
                triggerTransform.SetParent(obsObj.transform);
                var trigger = triggerTransform.gameObject.AddComponent<GizObstacleTrigger>();
                obs.triggerTransform = triggerTransform;

                if (version >= 2) triggerTransform.position = br.ReadVector3();
                obs.unknown1 = br.ReadSingle();
                trigger.radius = br.ReadSingle();
                if (version >= 3)
                {
                    obs.unknown3 = br.ReadVector3();
                    obs.unknown4 = br.ReadInt16();
                }
                obs.unknown5 = br.ReadInt32();
                if (version >= 12) obs.unknown6 = br.ReadInt32();
                if (version == 6) br.ReadBytes(3); //padding
                obs.animBehaviour = (GizObstacle.AnimBehaviour)br.ReadByte();
                obs.type = (GizObstacle.Type)br.ReadByte();

                if (version >= 15)
                {
                    obs.unknown17 = br.ReadSingle();
                    obs.unknown18 = br.ReadSingle();
                }
                if (version >= 17) obs.unknown19 = br.ReadSingle();
                if (version >= 18) obs.unknown20 = br.ReadSingle();

                if (version >= 7) obs.unknown11 = br.ReadByte();

                byte specObjVers = br.ReadByte();
                obs.specialObjectVersion = specObjVers;
                byte specObjCount = br.ReadByte();
                obs.specialObjects = new GizObstacle.SpecialObject[specObjCount];

                for (int j = 0; j < specObjCount; j++)
                {
                    GizObstacle.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = br.ReadString8() },
                        unknown1 = br.ReadSingle(),
                        animationTime = br.ReadSingle(),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = br.ReadInt32();
                    if (version >= 8) specObj.unknown3 = br.ReadInt16();

                    obs.specialObjects[j] = specObj;
                }

                if (version >= 4) obs.unknown12 = br.ReadSingle();
                if (version >= 5) obs.unknown13 = br.ReadSingle();
                if (version >= 8) obs.unknown14 = br.ReadSingle();
                if (version == 9)
                {
                    short blowupId = br.ReadInt16();
                    Debug.LogWarning($"Cannot load blowup via nametable ID ({blowupId}), blowup on GizObstacle '{name}' will be null");
                }
                if (version >= 10) obs.blowupType.SetBlowupType(br.ReadString8());
                if (version >= 9)
                {
                    obs.studsValue = br.ReadUInt16();

                    if (obs.studsSpawn == null)
                    {
                        GameObject spawnObj = new("studs_spawn_transform");
                        spawnObj.transform.SetParent(obsObj.transform);
                        obs.studsSpawn = spawnObj.transform;
                    }
                    obs.studsSpawn.eulerAngles = br.ReadXYEuler();
                    obs.studsSpawn.localPosition = br.ReadVector3();
                }
                if (version >= 11) obs.studsSpawnSpeed = br.ReadSingle();
                if (version >= 13) obs.unknownSfx1.sample = br.ReadString8();
                if (version >= 14) obs.unknownSfx2.sample = br.ReadString8();
                if (version >= 16) obs.unknown23 = br.ReadString8();

                if (version >= 19)
                {
                    obs.unknown21 = br.ReadInt32();
                    obs.unknown22 = br.ReadInt32();
                }
            }

            return obsCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write((byte)version);

            var obstacles = FindObjectsByType<GizObstacle>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            short obstacleCount = (short)obstacles.Length;
            bw.Write(obstacleCount);

            for (int i = 0; i < obstacleCount; i++)
            {
                var obs = obstacles[i];

                Vector3 triggerPos = obs.transform.position;
                float triggerRadius = 0.5f;
                var triggerTransform = obs.triggerTransform;
                if (triggerTransform != null && triggerTransform.TryGetComponent<GizObstacleTrigger>(out var trigger))
                {
                    triggerPos = triggerTransform.position;
                    triggerRadius = trigger.radius;
                }

                bw.WriteString(obs.name, 16);
                bw.Write(obs.transform.position);
                if (version >= 2) bw.Write(triggerPos);
                bw.Write(obs.unknown1);
                bw.Write(triggerRadius);
                if (version >= 3)
                {
                    bw.Write(obs.unknown3);
                    bw.Write(obs.unknown4);
                }
                bw.Write(obs.unknown5);
                if (version >= 12) bw.Write(obs.unknown6);
                if (version == 6)
                {
                    bw.Write((short)0); //padding
                    bw.Write((byte)0);
                }
                bw.Write((byte)obs.animBehaviour);
                bw.Write((byte)obs.type);

                if (version >= 15)
                {
                    bw.Write(obs.unknown17);
                    bw.Write(obs.unknown18);
                }
                if (version >= 17) bw.Write(obs.unknown19);
                if (version >= 18) bw.Write(obs.unknown20);

                if (version >= 7) bw.Write(obs.unknown11);

                byte specObjVers = obs.specialObjectVersion;
                bw.Write(specObjVers);
                byte specObjCount = (byte)obs.specialObjects.Length;
                bw.Write(specObjCount);

                for (int j = 0; j < specObjCount; j++)
                {
                    var specObj = obs.specialObjects[j];
                    bw.WriteString8(specObj.specialObject.specialObject);

                    bw.Write(specObj.unknown1);
                    bw.Write(specObj.animationTime);

                    if (specObjVers >= 2) bw.Write(specObj.unknown2);
                    if (version >= 8) bw.Write(specObj.unknown3);
                }

                if (version >= 4) bw.Write(obs.unknown12);
                if (version >= 5) bw.Write(obs.unknown13);
                if (version >= 8) bw.Write(obs.unknown14);
                if (version == 9)
                {
                    bw.Write((short)0);
                    Debug.LogWarning($"Cannot export blowup by nametable ID, blowup on GizObstacle '{name}' will be exported as 0");
                }
                if (version >= 10) bw.WriteString8(obs.blowupType.GetBlowupType());
                if (version >= 9)
                {
                    bw.Write(obs.studsValue);

                    if (obs.studsSpawn == null)
                    {
                        bw.Write((short)0);
                        bw.Write((short)0);
                        bw.Write(Vector3.zero);
                    }
                    else
                    {
                        Vector3 euler = obs.studsSpawn.eulerAngles;
                        bw.Write(euler.x.ToShortAng());
                        bw.Write(euler.y.ToShortAng());
                        bw.Write(obs.StudsSpawnPos);
                    }
                }
                if (version >= 11) bw.Write(obs.studsSpawnSpeed);
                if (version >= 13) bw.WriteString8(obs.unknownSfx1.sample);
                if (version >= 14) bw.WriteString8(obs.unknownSfx2.sample);
                if (version >= 16) bw.WriteString8(obs.unknown23);

                if (version >= 19)
                {
                    bw.Write(obs.unknown21);
                    bw.Write(obs.unknown22);
                }
            }
        }
    }
}
#endif