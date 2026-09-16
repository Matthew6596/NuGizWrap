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

    public class GizForceConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.TCS);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 16, _ => 1 };

        public int version;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadByte();
            short forceCount = br.ReadInt16();

            string[] existingNames = new string[forceCount];
            for (int i = 0; i < forceCount; i++)
            {
                string forceName = br.ReadString(16);

                forceName = ObjectNames.GetUniqueName(existingNames, forceName);
                existingNames[i] = forceName;

                GameObject forceObj = new(forceName);
                forceObj.transform.SetParent(parent);
                forceObj.transform.position = br.ReadVector3();
                var force = forceObj.AddComponent<GizForce>();

                if (version == 1) force.unknown1 = br.ReadVector3();
                force.returnTime = br.ReadSingle();
                force.shakeTime = br.ReadSingle();
                force.range = br.ReadSingle();
                if (version == 1)
                {
                    force.unknown2 = br.ReadVector3();
                    force.unknown3 = br.ReadInt16();
                }
                force.interactionOptions = (GizForce.InteractionOptions)br.ReadInt32();
                force.togglable = br.ReadByte() != 0;
                if (version >= 11) force.unknown4 = br.ReadByte();
                force.unknown5 = br.ReadByte();
                if (version == 1) force.unknown6 = br.ReadByte();

                byte specObjVers = br.ReadByte();
                force.specialObjectVersion = specObjVers;
                byte specObjCount = br.ReadByte();
                force.specialObjects = new GizForce.SpecialObject[specObjCount];

                for (int j = 0; j < specObjCount; j++)
                {
                    GizForce.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = br.ReadString8() },
                        unknown1 = br.ReadSingle(),
                        animationTime = br.ReadSingle(),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = br.ReadInt32();
                    if (version >= 9) specObj.unknown3 = br.ReadInt16();

                    force.specialObjects[j] = specObj;
                }

                force.forceSpeed = br.ReadSingle();
                force.returnSpeed = br.ReadSingle();

                if (version >= 6) force.autoForce = br.ReadSingle();
                if (version >= 7) force.effectScale = br.ReadSingle();
                if (version >= 3) force.unknown7 = br.ReadSingle();
                if (version == 4) force.unknown8 = br.ReadInt16();

                if (version >= 5) force.blowupType.SetBlowupType(br.ReadString8());

                if (version >= 4)
                {
                    force.minStuds = br.ReadUInt16();
                    force.maxStuds = br.ReadUInt16();

                    GameObject spawnObj = new("studs_spawn_transform");
                    spawnObj.transform.SetParent(forceObj.transform);
                    force.studsSpawn = spawnObj.transform;

                    force.studsSpawn.eulerAngles = br.ReadYEuler();
                    force.studsSpawn.localPosition = br.ReadVector3();
                }

                if (version >= 10) force.studsSpawnSpeed = br.ReadSingle();

                if (version >= 15)
                {
                    force.processSound.sample = br.ReadString8();
                    force.completeSound.sample = br.ReadString8();
                    force.returnSound.sample = br.ReadString8();
                }
            }

            return forceCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write((byte)version);

            var forces = FindObjectsByType<GizForce>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            short forceCount = (short)forces.Length;
            bw.Write(forceCount);

            for (int i = 0; i < forceCount; i++)
            {
                var force = forces[i];

                bw.WriteString(force.name, 16);
                bw.Write(force.transform.position);
                if (version == 1) bw.Write(force.unknown1);
                bw.Write(force.returnTime);
                bw.Write(force.shakeTime);
                bw.Write(force.range);
                if (version == 1)
                {
                    bw.Write(force.unknown2);
                    bw.Write(force.unknown3);
                }
                bw.Write((int)force.interactionOptions);
                bw.Write((byte)(force.togglable ? 0xff : 0)); //probably supposed to be index value not bool idk
                if (version >= 11) bw.Write(force.unknown4);
                bw.Write(force.unknown5);
                if (version == 1) bw.Write(force.unknown6);

                byte specObjVers = force.specialObjectVersion;
                bw.Write(specObjVers);
                byte specObjCount = (byte)force.specialObjects.Length;
                bw.Write(specObjCount);

                for (int j = 0; j < specObjCount; j++)
                {
                    var specObj = force.specialObjects[j];
                    bw.WriteString8(specObj.specialObject.specialObject);
                    bw.Write(specObj.unknown1);
                    bw.Write(specObj.animationTime);
                    if (specObjVers >= 2) bw.Write(specObj.unknown2);
                    if (version >= 9) bw.Write(specObj.unknown3);
                }

                bw.Write(force.forceSpeed);
                bw.Write(force.returnSpeed);

                if (version >= 6) bw.Write(force.autoForce);
                if (version >= 7) bw.Write(force.effectScale);
                if (version >= 3) bw.Write(force.unknown7);
                if (version == 4) bw.Write(force.unknown8);

                if (version >= 5) bw.WriteString8(force.blowupType.GetBlowupType());

                if (version >= 4)
                {
                    bw.Write(force.minStuds);
                    bw.Write(force.maxStuds);

                    if (force.studsSpawn == null)
                    {
                        bw.Write((short)0);
                        bw.Write(Vector3.zero);
                    }
                    else
                    {
                        bw.Write(force.studsSpawn.eulerAngles.y.ToShortAng());
                        bw.Write(force.studsSpawn.position - force.transform.position);
                    }
                }

                if (version >= 10) bw.Write(force.studsSpawnSpeed);

                if (version >= 15)
                {
                    bw.WriteString8(force.processSound.sample);
                    bw.WriteString8(force.completeSound.sample);
                    bw.WriteString8(force.returnSound.sample);
                }
            }
        }
    }
}
#endif