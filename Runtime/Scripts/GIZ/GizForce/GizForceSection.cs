//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.qsrddxlyy21k
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class GizForceSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.TCS);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 16, _ => 1 };

        public override string ID => "GizForce";
        public static GizForceSection Instance { get; private set; }

        public byte version = 16;

        private static Texture2D icon;
        private void OnValidate()
        {
            Instance = DoSingleton(Instance);

            Instance.SetIcon(ref icon, "Textures/GizmoIcons/ForceIcon");
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new() { version };

            var forces = FindObjectsByType<GizForce>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            short forceCount = (short)forces.Length;
            bytes.AddShort(forceCount);

            for (int i = 0; i < forceCount; i++)
            {
                var force = forces[i];

                bytes.AddFixedString(force.name, 16);
                bytes.AddVector3(force.transform.position);
                if (version == 1) bytes.AddVector3(force.unknown1);
                bytes.AddFloat(force.returnTime);
                bytes.AddFloat(force.shakeTime);
                bytes.AddFloat(force.range);
                if (version == 1)
                {
                    bytes.AddVector3(force.unknown2);
                    bytes.AddShort(force.unknown3);
                }
                bytes.AddInt((int)force.interactionOptions);
                bytes.Add((byte)(force.togglable ? 0xff : 0)); //probably supposed to be index value not bool idk
                if (version >= 11) bytes.Add(force.unknown4);
                bytes.Add(force.unknown5);
                if (version == 1) bytes.Add(force.unknown6);

                byte specObjVers = force.specialObjectVersion;
                bytes.Add(specObjVers);
                byte specObjCount = (byte)force.specialObjects.Length;
                bytes.Add(specObjCount);

                for (int j = 0; j < specObjCount; j++)
                {
                    var specObj = force.specialObjects[j];
                    bytes.AddString8(specObj.specialObject.specialObject);
                    bytes.AddFloat(specObj.unknown1);
                    bytes.AddFloat(specObj.animationTime);
                    if (specObjVers >= 2) bytes.AddInt(specObj.unknown2);
                    if (version >= 9) bytes.AddShort(specObj.unknown3);
                }

                bytes.AddFloat(force.forceSpeed);
                bytes.AddFloat(force.returnSpeed);

                if (version >= 6) bytes.AddFloat(force.autoForce);
                if (version >= 7) bytes.AddFloat(force.effectScale);
                if (version >= 3) bytes.AddFloat(force.unknown7);
                if (version == 4) bytes.AddShort(force.unknown8);

                if (version >= 5) bytes.AddString8(force.blowup.GetBlowup());

                if (version >= 4)
                {
                    bytes.AddShort((short)force.minStuds);
                    bytes.AddShort((short)force.maxStuds);

                    if (force.studsSpawn == null)
                    {
                        bytes.AddShort(0);
                        bytes.AddVector3(Vector3.zero);
                    }
                    else
                    {
                        bytes.AddShort((short)force.studsSpawn.eulerAngles.y.ToShortAng());
                        bytes.AddVector3(force.studsSpawn.position - force.transform.position);
                    }
                }

                if (version >= 10) bytes.AddFloat(force.studsSpawnSpeed);

                if (version >= 15)
                {
                    bytes.AddString8(force.processSound.sample);
                    bytes.AddString8(force.completeSound.sample);
                    bytes.AddString8(force.returnSound.sample);
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadByte(ref index);
            short forceCount = bytes.ReadShort(ref index);

            foreach (var force in FindObjectsByType<GizForce>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                force.gameObject.DelayDestroy();

            for (int i = 0; i < forceCount; i++)
            {
                string forceName = bytes.ReadString(ref index, 16);
                GameObject forceObj = new(forceName);
                forceObj.transform.SetParent(transform);
                forceObj.transform.position = bytes.ReadVector3(ref index);
                var force = forceObj.AddComponent<GizForce>();

                if (version == 1) force.unknown1 = bytes.ReadVector3(ref index);
                force.returnTime = bytes.ReadFloat(ref index);
                force.shakeTime = bytes.ReadFloat(ref index);
                force.range = bytes.ReadFloat(ref index);
                if (version == 1)
                {
                    force.unknown2 = bytes.ReadVector3(ref index);
                    force.unknown3 = bytes.ReadShort(ref index);
                }
                force.interactionOptions = (GizForce.InteractionOptions)bytes.ReadInt(ref index);
                force.togglable = bytes.ReadByte(ref index) != 0;
                if (version >= 11) force.unknown4 = bytes.ReadByte(ref index);
                force.unknown5 = bytes.ReadByte(ref index);
                if (version == 1) force.unknown6 = bytes.ReadByte(ref index);

                byte specObjVers = bytes.ReadByte(ref index);
                force.specialObjectVersion = specObjVers;
                byte specObjCount = bytes.ReadByte(ref index);
                force.specialObjects = new GizForce.SpecialObject[specObjCount];

                for (int j = 0; j < specObjCount; j++)
                {
                    GizForce.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = bytes.ReadString8(ref index) },
                        unknown1 = bytes.ReadFloat(ref index),
                        animationTime = bytes.ReadFloat(ref index),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = bytes.ReadInt(ref index);
                    if (version >= 9) specObj.unknown3 = bytes.ReadShort(ref index);

                    force.specialObjects[j] = specObj;
                }

                force.forceSpeed = bytes.ReadFloat(ref index);
                force.returnSpeed = bytes.ReadFloat(ref index);

                if (version >= 6) force.autoForce = bytes.ReadFloat(ref index);
                if (version >= 7) force.effectScale = bytes.ReadFloat(ref index);
                if (version >= 3) force.unknown7 = bytes.ReadFloat(ref index);
                if (version == 4) force.unknown8 = bytes.ReadShort(ref index);

                if (version >= 5) force.blowup.SetBlowup(bytes.ReadString8(ref index));

                if (version >= 4)
                {
                    force.minStuds = (ushort)bytes.ReadShort(ref index);
                    force.maxStuds = (ushort)bytes.ReadShort(ref index);

                    GameObject spawnObj = new("studs_spawn_transform");
                    spawnObj.transform.SetParent(forceObj.transform);
                    force.studsSpawn = spawnObj.transform;

                    force.studsSpawn.eulerAngles = new(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0);
                    force.studsSpawn.localPosition = bytes.ReadVector3(ref index);
                }

                if (version >= 10) force.studsSpawnSpeed = bytes.ReadFloat(ref index);

                if (version >= 15)
                {
                    force.processSound.sample = bytes.ReadString8(ref index);
                    force.completeSound.sample = bytes.ReadString8(ref index);
                    force.returnSound.sample = bytes.ReadString8(ref index);
                }
            }
        }
    }
}
#endif