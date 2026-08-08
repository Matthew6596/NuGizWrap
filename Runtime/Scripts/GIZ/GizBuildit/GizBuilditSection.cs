//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.fqtcanhs8tod
//-Matton
//===== ===== ===== ===== =====

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class GizBuilditSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 9, TTGame.LIJ1=>9, TTGame.LB1=>10, _ => 1 };

        public override string ID => "GizBuildit";

        public static GizBuilditSection Instance { get; private set; }

        public byte version = 9;

        private static Texture2D icon;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);

            Instance.SetIcon(ref icon, "Textures/GizmoIcons/BuilditIcon");
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new() { version };

            var buildits = FindObjectsByType<GizBuildit>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            short builditCount = (short)buildits.Length;
            bytes.AddShort(builditCount);

            for(int i=0; i<builditCount; i++)
            {
                var buildit = buildits[i];
                bytes.AddFixedString(buildit.name, 16);
                bytes.AddVector3(buildit.transform.position);

                byte specObjVers = buildit.specialObjectVersion;
                byte specObjCount = (byte)buildit.specialObjects.Length;
                bytes.Add(specObjVers);
                bytes.Add(specObjCount);

                for(int j=0; j<specObjCount; j++)
                {
                    var specObj = buildit.specialObjects[j];
                    bytes.AddString8(specObj.specialObject.specialObject);
                    bytes.AddFloat(specObj.unknown1);
                    bytes.AddFloat(specObj.animationTime);
                    if (specObjVers >= 2) bytes.AddInt(specObj.unknown2);
                }

                bytes.AddFloat(buildit.jumpIntensity);
                if (version <= 6) bytes.AddFloat(0); //padding
                bytes.AddShort((short)buildit.minStuds);
                bytes.AddShort((short)buildit.maxStuds);
                bytes.Add(buildit.unknown2);
                bytes.Add(buildit.unknown3);
                if (version >= 10) bytes.AddFloat(buildit.unknown10);
                if (version >= 6) bytes.AddFloat(buildit.unknown4);
                if (version == 7)
                {
                    bytes.AddShort(0);
                    Debug.LogWarning($"Cannot export blowup by nametable ID, blowup on GizBuildit '{name}' will be exported as 0");
                }
                if (version >= 8) bytes.AddString8(buildit.blowup.GetBlowup());
                if (version >= 7)
                {
                    if (buildit.studsSpawn == null)
                    {
                        bytes.AddShort(0);
                        bytes.AddShort(0);
                        bytes.AddVector3(Vector3.zero);
                    }
                    else
                    {
                        Vector3 euler = buildit.studsSpawn.eulerAngles;
                        bytes.AddShort((short)euler.x.ToShortAng());
                        bytes.AddShort((short)euler.y.ToShortAng());
                        bytes.AddVector3(buildit.StudsSpawnPos);
                    }
                }
                if (version >= 9) bytes.AddFloat(buildit.studsSpawnSpeed);
                if (version >= 4) bytes.AddShort(buildit.unknown7);
                if (version >= 5)
                {
                    bytes.AddShort(buildit.unknown8);
                    bool hasBuildit = buildit.unknown9 != null && buildit.unknown9.Trim().Length > 0;
                    bytes.Add((byte)(hasBuildit ? 1 : 0));
                    if(hasBuildit) bytes.AddFixedString(buildit.unknown9, 16);
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadByte(ref index);
            short builditCount = bytes.ReadShort(ref index);

            //Clear buildits before loading new ones
            foreach (var build in FindObjectsByType<GizBuildit>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) 
                build.gameObject.DelayDestroy();

            for (int i=0; i<builditCount; i++)
            {
                string buildName = bytes.ReadString(ref index, 16);
                GameObject buildObj = new(buildName);
                buildObj.transform.SetParent(transform);
                buildObj.transform.position = bytes.ReadVector3(ref index);
                var buildit = buildObj.AddComponent<GizBuildit>();

                byte specObjVers = bytes.ReadByte(ref index);
                byte specObjCount = bytes.ReadByte(ref index);
                buildit.specialObjectVersion = specObjVers;
                buildit.specialObjects = new GizBuildit.SpecialObject[specObjCount];

                for(int j=0; j<specObjCount; j++)
                {
                    GizBuildit.SpecialObject specObj = new()
                    {
                        specialObject = new() { specialObject = bytes.ReadString8(ref index) },
                        unknown1 = bytes.ReadFloat(ref index),
                        animationTime = bytes.ReadFloat(ref index),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = bytes.ReadInt(ref index);

                    buildit.specialObjects[j] = specObj;
                }

                buildit.jumpIntensity = bytes.ReadFloat(ref index);
                if (version <= 6) index += 4; //padding
                buildit.minStuds = (ushort)bytes.ReadShort(ref index);
                buildit.maxStuds = (ushort)bytes.ReadShort(ref index);
                buildit.unknown2 = bytes.ReadByte(ref index);
                buildit.unknown3 = bytes.ReadByte(ref index);
                if (version >= 10) buildit.unknown10 = bytes.ReadFloat(ref index);
                if (version >= 6) buildit.unknown4 = bytes.ReadFloat(ref index);
                if (version == 7)
                {
                    short blowupId = bytes.ReadShort(ref index);
                    Debug.LogWarning($"Cannot load blowup via nametable ID ({blowupId}), blowup on GizBuildit '{name}' will be null");
                }
                if (version >= 8) buildit.blowup.SetBlowup(bytes.ReadString8(ref index));
                if (version >= 7)
                {
                    if (buildit.studsSpawn == null)
                    {
                        GameObject studSpawnObj = new("studs_spawn");
                        studSpawnObj.transform.SetParent(buildit.transform);
                        buildit.studsSpawn = studSpawnObj.transform;
                    }
                    buildit.studsSpawn.eulerAngles = new(((ushort)bytes.ReadShort(ref index)).ToFloatAng(), ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0);
                    buildit.studsSpawn.localPosition = bytes.ReadVector3(ref index);
                }
                if (version >= 9) buildit.studsSpawnSpeed = bytes.ReadFloat(ref index);
                if (version >= 4) buildit.unknown7 = bytes.ReadShort(ref index);
                if (version >= 5)
                {
                    buildit.unknown8 = bytes.ReadShort(ref index);
                    if (bytes.ReadByte(ref index) != 0) buildit.unknown9 = bytes.ReadString(ref index, 16);
                }
            }
        }
    }
}
#endif