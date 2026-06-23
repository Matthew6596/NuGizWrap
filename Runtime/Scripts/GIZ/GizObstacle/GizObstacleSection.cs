//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.lihvgaff1fy5
//-Matton
//===== ===== ===== ===== =====

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class GizObstacleSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 14, TTGame.LIJ1=>19, TTGame.LB1=>20, _ => 1 };

        public override string ID => "GizObstacle";

        public static GizObstacleSection Instance { get; private set; }

        public byte version = 14;

        private static Texture2D icon;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);

            Instance.SetIcon(ref icon, "Textures/GizmoIcons/ObstacleIcon");
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new() { version };

            var obstacles = FindObjectsByType<GizObstacle>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            short obstacleCount = (short)obstacles.Length;
            bytes.AddShort(obstacleCount);

            for(int i=0; i<obstacleCount; i++)
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

                bytes.AddFixedString(obs.name, 16);
                bytes.AddVector3(obs.transform.position);
                if (version >= 2) bytes.AddVector3(triggerPos);
                bytes.AddFloat(obs.unknown1);
                bytes.AddFloat(triggerRadius);
                if (version >= 3)
                {
                    bytes.AddVector3(obs.unknown3);
                    bytes.AddShort(obs.unknown4);
                }
                bytes.AddInt(obs.unknown5);
                if (version >= 12) bytes.AddInt(obs.unknown6);
                if (version == 6)
                {
                    bytes.AddShort(obs.unknown7);
                    bytes.Add(obs.unknown8);
                }
                bytes.Add(obs.unknown9);
                bytes.Add(obs.unknown10);

                if (version >= 15)
                {
                    bytes.AddFloat(obs.unknown17);
                    bytes.AddFloat(obs.unknown18);
                }
                if (version >= 17) bytes.AddFloat(obs.unknown19);
                if (version >= 18) bytes.AddFloat(obs.unknown20);

                if (version >= 7) bytes.Add(obs.unknown11);

                byte specObjVers = obs.specialObjectVersion;
                bytes.Add(specObjVers);
                byte specObjCount = (byte)obs.specialObjects.Length;
                bytes.Add(specObjCount);

                for(int j=0; j<specObjCount; j++)
                {
                    var specObj = obs.specialObjects[j];
                    bytes.AddString8(specObj.specialObject.specialObject);

                    bytes.AddFloat(specObj.unknown1);
                    bytes.AddFloat(specObj.animationTime);

                    if (specObjVers >= 2) bytes.AddInt(specObj.unknown2);
                    if (version >= 8) bytes.AddShort(specObj.unknown3);
                }

                if (version >= 4) bytes.AddFloat(obs.unknown12);
                if (version >= 5) bytes.AddFloat(obs.unknown13);
                if (version >= 8) bytes.AddFloat(obs.unknown14);
                if (version == 9) bytes.AddShort(obs.unknown15);
                if (version >= 10) bytes.AddString8(obs.unknown16);
                if (version >= 9)
                {
                    bytes.AddShort((short)obs.minStuds);
                    bytes.AddShort((short)obs.maxStuds);

                    if (obs.studsSpawn == null)
                    {
                        bytes.AddShort(0);
                        bytes.AddVector3(Vector3.zero);
                    }
                    else
                    {
                        bytes.AddShort((short)obs.studsSpawn.eulerAngles.y.ToShortAng());
                        bytes.AddVector3(obs.StudsSpawnPos);
                    }
                }
                if (version >= 11) bytes.AddFloat(obs.studsSpawnSpeed);
                if (version >= 13) bytes.AddString8(obs.unknownSfx1.sample);
                if (version >= 14) bytes.AddString8(obs.unknownSfx2.sample);
                if (version >= 16) bytes.AddString8(obs.unknownSfx3.sample);

                if (version >= 19)
                {
                    bytes.AddInt(obs.unknown21);
                    bytes.AddInt(obs.unknown22);
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadByte(ref index);
            short obsCount = bytes.ReadShort(ref index);

            //Clear GizObstacles before loading new ones
            foreach (var obs in FindObjectsByType<GizObstacle>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) obs.gameObject.DelayDestroy();

            for(int i=0; i<obsCount; i++)
            {
                string obsName = bytes.ReadString(ref index, 16);
                GameObject obsObj = new(obsName);
                obsObj.transform.SetParent(transform);
                obsObj.transform.position = bytes.ReadVector3(ref index);
                var obs = obsObj.AddComponent<GizObstacle>();

                Transform triggerTransform = new GameObject("trigger_transform").transform;
                triggerTransform.SetParent(obsObj.transform);
                var trigger = triggerTransform.gameObject.AddComponent<GizObstacleTrigger>();
                obs.triggerTransform = triggerTransform;

                if (version >= 2) triggerTransform.position = bytes.ReadVector3(ref index);
                obs.unknown1 = bytes.ReadFloat(ref index);
                trigger.radius = bytes.ReadFloat(ref index);
                if (version >= 3)
                {
                    obs.unknown3 = bytes.ReadVector3(ref index);
                    obs.unknown4 = bytes.ReadShort(ref index);
                }
                obs.unknown5 = bytes.ReadInt(ref index);
                if (version >= 12) obs.unknown6 = bytes.ReadInt(ref index);
                if (version == 6)
                {
                    obs.unknown7 = bytes.ReadShort(ref index);
                    obs.unknown8 = bytes.ReadByte(ref index);
                }
                obs.unknown9 = bytes.ReadByte(ref index);
                obs.unknown10 = bytes.ReadByte(ref index);

                if (version >= 15)
                {
                    obs.unknown17 = bytes.ReadFloat(ref index);
                    obs.unknown18 = bytes.ReadFloat(ref index);
                }
                if (version >= 17) obs.unknown19 = bytes.ReadFloat(ref index);
                if (version >= 18) obs.unknown20 = bytes.ReadFloat(ref index);

                if (version >= 7) obs.unknown11 = bytes.ReadByte(ref index);

                byte specObjVers = bytes.ReadByte(ref index);
                obs.specialObjectVersion = specObjVers;
                byte specObjCount = bytes.ReadByte(ref index);
                obs.specialObjects = new GizObstacle.SpecialObject[specObjCount];

                for(int j=0; j<specObjCount; j++)
                {
                    GizObstacle.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = bytes.ReadString8(ref index) },
                        unknown1 = bytes.ReadFloat(ref index),
                        animationTime = bytes.ReadFloat(ref index),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = bytes.ReadInt(ref index);
                    if (version >= 8) specObj.unknown3 = bytes.ReadShort(ref index);

                    obs.specialObjects[j] = specObj;
                }

                if (version >= 4) obs.unknown12 = bytes.ReadFloat(ref index);
                if (version >= 5) obs.unknown13 = bytes.ReadFloat(ref index);
                if (version >= 8) obs.unknown14 = bytes.ReadFloat(ref index);
                if (version == 9) obs.unknown15 = bytes.ReadShort(ref index);
                if (version >= 10) obs.unknown16 = bytes.ReadString8(ref index);
                if (version >= 9)
                {
                    obs.minStuds = (ushort)bytes.ReadShort(ref index);
                    obs.maxStuds = (ushort)bytes.ReadShort(ref index);

                    if(obs.studsSpawn == null)
                    {
                        GameObject spawnObj = new("studs_spawn_transform");
                        spawnObj.transform.SetParent(obsObj.transform);
                        obs.studsSpawn = spawnObj.transform;
                    }
                    obs.studsSpawn.eulerAngles = new(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0);
                    obs.studsSpawn.localPosition = bytes.ReadVector3(ref index);
                }
                if (version >= 11) obs.studsSpawnSpeed = bytes.ReadFloat(ref index);
                if (version >= 13) obs.unknownSfx1.sample = bytes.ReadString8(ref index);
                if (version >= 14) obs.unknownSfx2.sample = bytes.ReadString8(ref index);
                if (version >= 16) obs.unknownSfx3.sample = bytes.ReadString8(ref index);

                if (version >= 19)
                {
                    obs.unknown21 = bytes.ReadInt(ref index);
                    obs.unknown22 = bytes.ReadInt(ref index);
                }
            }
        }
    }
}
#endif