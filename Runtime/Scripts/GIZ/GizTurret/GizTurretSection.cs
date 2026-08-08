//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.z49rmjts1z7t
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace TTModdingKit.Gizmos 
{
    using Helper;
    using System.Linq;

    public class GizTurretSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 7, TTGame.LIJ1=>7, TTGame.LB1=>7, _ => 1 };

        public override string ID => "GizTurret";

        public static GizTurretSection Instance { get; private set; }

        public byte version = 7;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new() { version };

            var turrets = FindObjectsByType<GizTurret>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int turretCount = turrets.Length;

            bytes.AddShort((short)turretCount);

            for(int i=0; i<turretCount; i++)
            {
                var turret = turrets[i];

                bytes.AddFixedString(turret.name, 16);
                byte specObjVers = turret.specialObjectVersion;
                byte specObjCount = (byte)turret.specialObjects.Length;
                bytes.Add(specObjVers);
                bytes.Add(specObjCount);

                for(int j=0; j<specObjCount; j++)
                {
                    var specObj = turret.specialObjects[j];
                    bytes.AddString8(specObj.specialObject.specialObject);
                    bytes.AddFloat(specObj.unknown1);
                    bytes.AddFloat(specObj.animationTime);
                    if (specObjVers >= 2) bytes.AddInt(specObj.unknown2);
                    if (version >= 3) bytes.AddShort(specObj.unknown3);
                }

                bytes.AddVector3(turret.transform.position);
                bytes.AddVector3(turret.unknown2);
                bytes.AddVector3(turret.unknown3);
                bytes.AddVector3(turret.unknown4);

                bytes.AddInt(turret.unknown5);
                bytes.AddInt(turret.unknown6);
                bytes.AddInt(turret.unknown7);
                bytes.AddInt(turret.unknown8);
                bytes.AddInt(turret.unknown9);
                bytes.AddInt(turret.unknown10);
                if (version >= 2) bytes.AddInt(turret.unknown11);

                byte unk12Count = (byte)turret.unknown12.Length;
                bytes.Add(unk12Count);
                for (int j = 0; j < unk12Count; j++) bytes.AddVector3(turret.unknown12[j]);

                bytes.AddFloat(turret.unknown13);
                bytes.AddFloat(turret.shootRange);
                bytes.AddFloat(turret.unknown15);
                bytes.AddFloat(turret.fireRate);
                bytes.AddFloat(turret.yRotationSpeed);
                bytes.AddFloat(turret.xRotationSpeed);

                bytes.AddShort((short)turret.studsValue);

                if (turret.studsSpawn == null)
                {
                    bytes.AddShort(0);
                    bytes.AddShort(0);
                    bytes.AddVector3(Vector3.zero);
                }
                else
                {
                    Vector3 studsEuler = turret.studsSpawn.eulerAngles;
                    bytes.AddShort((short)studsEuler.x.ToShortAng());
                    bytes.AddShort((short)studsEuler.y.ToShortAng());
                    bytes.AddVector3(turret.studsSpawn.position - turret.transform.position);
                }

                if (version >= 6) bytes.AddFloat(turret.studsSpawnSpeed);

                bytes.Add(turret.unknown19);

                if (version >= 4)
                {
                    bytes.Add(turret.unknown20);
                    bytes.AddShort(turret.unknown21);
                }

                bytes.AddString8(turret.boltType);
                bytes.AddString8(turret.unknownSfx1.sample);
                bytes.AddString8(turret.unknownSfx2.sample);
                if (version >= 7) bytes.AddString8(turret.unknownSfx3.sample);
                bytes.AddString8(turret.blowup.GetBlowup());

                bytes.AddShort(turret.unknown22);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadByte(ref index);
            short turretCount = bytes.ReadShort(ref index);

            //Clear existing turrets before adding new ones
            foreach (var t in FindObjectsByType<GizTurret>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) t.gameObject.DelayDestroy();

            for (int i = 0; i < turretCount; i++)
            {
                GameObject turretObj = new(bytes.ReadString(ref index, 16));
                turretObj.transform.SetParent(transform);
                var turret = turretObj.AddComponent<GizTurret>();

                byte specObjVers = bytes.ReadByte(ref index);
                byte specObjCount = bytes.ReadByte(ref index);
                turret.specialObjectVersion = specObjVers;
                turret.specialObjects = new GizTurret.SpecialObject[specObjCount];

                for (int j = 0; j < specObjCount; j++)
                {
                    GizTurret.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = bytes.ReadString8(ref index) },
                        unknown1 = bytes.ReadFloat(ref index),
                        animationTime = bytes.ReadFloat(ref index),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = bytes.ReadInt(ref index);
                    if (version >= 3) specObj.unknown3 = bytes.ReadShort(ref index);

                    turret.specialObjects[j] = specObj;
                }

                turret.transform.position = bytes.ReadVector3(ref index);
                turret.unknown2 = bytes.ReadVector3(ref index);
                turret.unknown3 = bytes.ReadVector3(ref index);
                turret.unknown4 = bytes.ReadVector3(ref index);

                turret.unknown5 = bytes.ReadInt(ref index);
                turret.unknown6 = bytes.ReadInt(ref index);
                turret.unknown7 = bytes.ReadInt(ref index);
                turret.unknown8 = bytes.ReadInt(ref index);
                turret.unknown9 = bytes.ReadInt(ref index);
                turret.unknown10 = bytes.ReadInt(ref index);
                if (version >= 2) turret.unknown11 = bytes.ReadInt(ref index);

                byte unk12Count = bytes.ReadByte(ref index);
                turret.unknown12 = new Vector3[unk12Count];
                for (int j = 0; j < unk12Count; j++) turret.unknown12[j] = bytes.ReadVector3(ref index);

                turret.unknown13 = bytes.ReadFloat(ref index);
                turret.shootRange = bytes.ReadFloat(ref index);
                turret.unknown15 = bytes.ReadFloat(ref index);
                turret.fireRate = bytes.ReadFloat(ref index);
                turret.yRotationSpeed = bytes.ReadFloat(ref index);
                turret.xRotationSpeed = bytes.ReadFloat(ref index);

                turret.studsValue = (ushort)bytes.ReadShort(ref index);

                GameObject studsSpawn = new("studs_spawn_transform");
                studsSpawn.transform.SetParent(turret.transform);
                studsSpawn.transform.eulerAngles = bytes.ReadXYEuler(ref index);
                studsSpawn.transform.localPosition = bytes.ReadVector3(ref index);
                turret.studsSpawn = studsSpawn.transform;

                if (version >= 6) turret.studsSpawnSpeed = bytes.ReadFloat(ref index);

                turret.unknown19 = bytes.ReadByte(ref index);

                if (version >= 4)
                {
                    turret.unknown20 = bytes.ReadByte(ref index);
                    turret.unknown21 = bytes.ReadShort(ref index);
                }

                turret.boltType = bytes.ReadString8(ref index);
                turret.unknownSfx1 = new() { sample = bytes.ReadString8(ref index) };
                turret.unknownSfx2 = new() { sample = bytes.ReadString8(ref index) };
                if (version >= 7) turret.unknownSfx3 = new() { sample = bytes.ReadString8(ref index) };
                turret.blowup = new() { blowupName =  bytes.ReadString8(ref index) };

                turret.unknown22 = bytes.ReadShort(ref index);
            }
        }
    }
}
#endif