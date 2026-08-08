//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.qwdbbb68ysum
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class GizDigSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 19, _ => 1 };

        public override string ID => "GizDig";
        public static GizDigSection Instance { get; private set; }

        public byte version = 19;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new() { version };

            var digs = FindObjectsByType<GizDig>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            short digCount = (short)digs.Length;
            bytes.AddShort(digCount);

            for (int i = 0; i < digCount; i++)
            {
                var dig = digs[i];

                bytes.AddFixedString(dig.name, 16);
                bytes.AddVector3(dig.transform.position);
                if (version >= 17) bytes.AddVector3(dig.unknown1);
                bytes.AddFloat(dig.unknown2);
                bytes.AddInt((int)dig.interactionOptions);

                byte specObjVers = dig.specialObjectVersion;
                bytes.Add(specObjVers);
                byte specObjCount = (byte)dig.specialObjects.Length;
                bytes.Add(specObjCount);

                for (int j = 0; j < specObjCount; j++)
                {
                    var specObj = dig.specialObjects[j];
                    bytes.AddString8(specObj.specialObject.specialObject);
                    bytes.AddFloat(specObj.unknown1);
                    bytes.AddFloat(specObj.animationTime);
                    if (specObjVers >= 2) bytes.AddInt(specObj.unknown2);
                    bytes.AddShort(specObj.unknown3);
                }

                bytes.AddFloat(dig.animSpeed);
                bytes.AddFloat(dig.animAdvanceAmount);

                bytes.AddString8(dig.blowup.GetBlowup());

                bytes.AddShort((short)dig.studsValue);

                if (dig.studsSpawn == null)
                {
                    bytes.AddShort(0);
                    bytes.AddShort(0);
                    bytes.AddVector3(Vector3.zero);
                }
                else
                {
                    Vector3 euler = dig.studsSpawn.eulerAngles;
                    bytes.AddShort((short)euler.x.ToShortAng());
                    bytes.AddShort((short)euler.y.ToShortAng());
                    bytes.AddVector3(dig.studsSpawn.position - dig.transform.position);
                }

                bytes.AddFloat(dig.studsSpawnSpeed);

                bytes.AddString8(dig.unknownSfx.sample);

                bytes.AddShort(dig.numSteps);
                bytes.AddShort(dig.unknown7);
                if (version >= 18) bytes.AddShort((short)dig.tool);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadByte(ref index);
            short digCount = bytes.ReadShort(ref index);

            foreach (var dig in FindObjectsByType<GizDig>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                dig.gameObject.DelayDestroy();

            for (int i = 0; i < digCount; i++)
            {
                string digName = bytes.ReadString(ref index, 16);
                GameObject digObj = new(digName);
                digObj.transform.SetParent(transform);
                digObj.transform.position = bytes.ReadVector3(ref index);
                var dig = digObj.AddComponent<GizDig>();

                if (version >= 17) dig.unknown1 = bytes.ReadVector3(ref index);
                dig.unknown2 = bytes.ReadFloat(ref index);
                dig.interactionOptions = (GizDig.InteractionOptions)bytes.ReadInt(ref index);

                byte specObjVers = bytes.ReadByte(ref index);
                dig.specialObjectVersion = specObjVers;
                byte specObjCount = bytes.ReadByte(ref index);
                dig.specialObjects = new GizDig.SpecialObject[specObjCount];

                for (int j = 0; j < specObjCount; j++)
                {
                    GizDig.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = bytes.ReadString8(ref index) },
                        unknown1 = bytes.ReadFloat(ref index),
                        animationTime = bytes.ReadFloat(ref index),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = bytes.ReadInt(ref index);
                    specObj.unknown3 = bytes.ReadShort(ref index);

                    dig.specialObjects[j] = specObj;
                }

                dig.animSpeed = bytes.ReadFloat(ref index);
                dig.animAdvanceAmount = bytes.ReadFloat(ref index);

                dig.blowup = new() { blowupName = bytes.ReadString8(ref index) };

                dig.studsValue = (ushort)bytes.ReadShort(ref index);

                GameObject spawnObj = new("studs_spawn_transform");
                spawnObj.transform.SetParent(digObj.transform);
                dig.studsSpawn = spawnObj.transform;

                dig.studsSpawn.eulerAngles = bytes.ReadXYEuler(ref index);
                dig.studsSpawn.localPosition = bytes.ReadVector3(ref index);

                dig.studsSpawnSpeed = bytes.ReadFloat(ref index);

                dig.unknownSfx = new() { sample = bytes.ReadString8(ref index) };

                dig.numSteps = bytes.ReadShort(ref index);
                dig.unknown7 = bytes.ReadShort(ref index);
                if (version >= 18) dig.tool = (GizDig.Tool)bytes.ReadShort(ref index);
            }
        }
    }
}
#endif