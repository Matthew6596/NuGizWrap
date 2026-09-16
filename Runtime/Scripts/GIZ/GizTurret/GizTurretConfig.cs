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

    public class GizTurretConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 7, TTGame.LIJ1 => 7, TTGame.LB1 => 7, _ => 1 };

        public byte version = 7;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadByte();
            short turretCount = br.ReadInt16();

            string[] existingNames = new string[turretCount];
            for (int i = 0; i < turretCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject turretObj = new(name);
                turretObj.transform.SetParent(parent);
                var turret = turretObj.AddComponent<GizTurret>();

                byte specObjVers = br.ReadByte();
                byte specObjCount = br.ReadByte();
                turret.specialObjectVersion = specObjVers;
                turret.specialObjects = new GizTurret.SpecialObject[specObjCount];

                for (int j = 0; j < specObjCount; j++)
                {
                    GizTurret.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = br.ReadString8() },
                        unknown1 = br.ReadSingle(),
                        animationTime = br.ReadSingle(),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = br.ReadInt32();
                    if (version >= 3) specObj.unknown3 = br.ReadInt16();

                    turret.specialObjects[j] = specObj;
                }

                turret.transform.position = br.ReadVector3();
                turret.unknown2 = br.ReadVector3();
                turret.unknown3 = br.ReadVector3();
                turret.unknown4 = br.ReadVector3();

                turret.unknown5 = br.ReadInt32();
                turret.unknown6 = br.ReadInt32();
                turret.unknown7 = br.ReadInt32();
                turret.unknown8 = br.ReadInt32();
                turret.unknown9 = br.ReadInt32();
                turret.unknown10 = br.ReadInt32();
                if (version >= 2) turret.unknown11 = br.ReadInt32();

                byte unk12Count = br.ReadByte();
                turret.unknown12 = new Vector3[unk12Count];
                for (int j = 0; j < unk12Count; j++) turret.unknown12[j] = br.ReadVector3();

                turret.unknown13 = br.ReadSingle();
                turret.shootRange = br.ReadSingle();
                turret.unknown15 = br.ReadSingle();
                turret.fireRate = br.ReadSingle();
                turret.yRotationSpeed = br.ReadSingle();
                turret.xRotationSpeed = br.ReadSingle();

                turret.studsValue = br.ReadUInt16();

                GameObject studsSpawn = new("studs_spawn_transform");
                studsSpawn.transform.SetParent(turret.transform);
                studsSpawn.transform.eulerAngles = br.ReadXYEuler();
                studsSpawn.transform.localPosition = br.ReadVector3();
                turret.studsSpawn = studsSpawn.transform;

                if (version >= 6) turret.studsSpawnSpeed = br.ReadSingle();

                turret.unknown19 = br.ReadByte();

                if (version >= 4)
                {
                    turret.unknown20 = br.ReadByte();
                    turret.unknown21 = br.ReadInt16();
                }

                turret.boltType = br.ReadString8();
                turret.unknownSfx1 = new() { sample = br.ReadString8() };
                turret.unknownSfx2 = new() { sample = br.ReadString8() };
                if (version >= 7) turret.unknownSfx3 = new() { sample = br.ReadString8() };
                turret.blowup = new() { blowupName = br.ReadString8() };

                turret.unknown22 = br.ReadInt16();
            }

            return turretCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var turrets = FindObjectsByType<GizTurret>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int turretCount = turrets.Length;

            bw.Write((short)turretCount);

            for (int i = 0; i < turretCount; i++)
            {
                var turret = turrets[i];

                bw.WriteString(turret.name, 16);
                byte specObjVers = turret.specialObjectVersion;
                byte specObjCount = (byte)turret.specialObjects.Length;
                bw.Write(specObjVers);
                bw.Write(specObjCount);

                for (int j = 0; j < specObjCount; j++)
                {
                    var specObj = turret.specialObjects[j];
                    bw.WriteString8(specObj.specialObject.specialObject);
                    bw.Write(specObj.unknown1);
                    bw.Write(specObj.animationTime);
                    if (specObjVers >= 2) bw.Write(specObj.unknown2);
                    if (version >= 3) bw.Write(specObj.unknown3);
                }

                bw.Write(turret.transform.position);
                bw.Write(turret.unknown2);
                bw.Write(turret.unknown3);
                bw.Write(turret.unknown4);

                bw.Write(turret.unknown5);
                bw.Write(turret.unknown6);
                bw.Write(turret.unknown7);
                bw.Write(turret.unknown8);
                bw.Write(turret.unknown9);
                bw.Write(turret.unknown10);
                if (version >= 2) bw.Write(turret.unknown11);

                byte unk12Count = (byte)turret.unknown12.Length;
                bw.Write(unk12Count);
                for (int j = 0; j < unk12Count; j++) bw.Write(turret.unknown12[j]);

                bw.Write(turret.unknown13);
                bw.Write(turret.shootRange);
                bw.Write(turret.unknown15);
                bw.Write(turret.fireRate);
                bw.Write(turret.yRotationSpeed);
                bw.Write(turret.xRotationSpeed);

                bw.Write(turret.studsValue);

                if (turret.studsSpawn == null)
                {
                    bw.Write((short)0);
                    bw.Write((short)0);
                    bw.Write(Vector3.zero);
                }
                else
                {
                    Vector3 studsEuler = turret.studsSpawn.eulerAngles;
                    bw.Write(studsEuler.x.ToShortAng());
                    bw.Write(studsEuler.y.ToShortAng());
                    bw.Write(turret.studsSpawn.position - turret.transform.position);
                }

                if (version >= 6) bw.Write(turret.studsSpawnSpeed);

                bw.Write(turret.unknown19);

                if (version >= 4)
                {
                    bw.Write(turret.unknown20);
                    bw.Write(turret.unknown21);
                }

                bw.WriteString8(turret.boltType);
                bw.WriteString8(turret.unknownSfx1.sample);
                bw.WriteString8(turret.unknownSfx2.sample);
                if (version >= 7) bw.WriteString8(turret.unknownSfx3.sample);
                bw.WriteString8(turret.blowup.GetBlowup());

                bw.Write(turret.unknown22);
            }
        }
    }
}
#endif