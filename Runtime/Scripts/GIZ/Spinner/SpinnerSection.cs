//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.orptf9we8ubl
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class SpinnerSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 9, TTGame.LIJ1=>11, TTGame.LB1=>13, _ => 1 };

        public override string ID => "Spinner";

        public static SpinnerSection Instance { get; private set; }

        public int version = 9;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
            if(version <= 6)
            {
                //Due to special object shenanigans (may implement later)
                EditorUtility.DisplayDialog("Spinner Version Warning", $"SpinnerSection versions less than 7 are not supported. (version: {version})", "OK");
                version = 7;
            }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var spinners = FindObjectsByType<Spinner>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int spinnerCount = spinners.Length;

            bytes.AddInt(spinnerCount);

            for (int i = 0; i < spinnerCount; i++)
            {
                var spinner = spinners[i];

                bytes.AddString8(spinner.name);
                bytes.AddVector3(spinner.transform.position);
                bytes.AddShort((short)spinner.transform.eulerAngles.y.ToShortAng());
                bytes.AddString8(spinner.specialObject.specialObject);
                byte outputCount = (byte)spinner.outputStates.Length;
                bytes.Add(outputCount);

                byte flapCount = spinner.flapCount;
                if (version >= 2) bytes.Add(flapCount);

                int unk1 = spinner.interactionOptions;
                if (version >= 3)
                {
                    bytes.AddInt(unk1);
                    bytes.AddFloat(spinner.outputStickTime);
                }
                if (version >= 4) bytes.AddFloat(spinner.animSpeed);
                if (unk1 != 0)
                {
                    if (version >= 11) bytes.AddShort(spinner.unknown4);
                    else if (version >= 6) bytes.Add((byte)spinner.unknown4);
                }

                //Support for versions <5 excluded here

                byte specObjVers = spinner.specialObjectVersion;
                byte specObjCount = (byte)spinner.animObjects.Length;
                bytes.Add(specObjVers);
                bytes.Add(specObjCount);
                for (int j = 0; j < specObjCount; j++)
                {
                    var specObj = spinner.animObjects[j];
                    bytes.AddString8(specObj.specialObject.specialObject);
                    bytes.AddFloat(specObj.unknown1);
                    bytes.AddFloat(specObj.animationTime);
                    if (specObjVers >= 2) bytes.AddInt(specObj.unknown2);
                }

                //Support for versions <7 excluded here

                if (version >= 7)
                {
                    for (int j = 0; j < outputCount; j++) bytes.AddFloat(spinner.outputStates[j]);
                }

                if(version >= 8) bytes.AddFloat(spinner.unknown6);
                if(version >= 9) bytes.AddFloat(spinner.unknown7);

                if(version >= 10) bytes.AddString8(spinner.unknownSpecialObject.specialObject);
                if(version >= 12) bytes.AddFloat(spinner.unknown9);
                if(version >= 13) bytes.AddInt(spinner.unknown10);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int spinnerCount = bytes.ReadInt(ref index);

            //Clear existing spinners before creating new ones
            foreach (var spin in FindObjectsByType<Spinner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) spin.gameObject.DelayDestroy();

            for(int i=0; i<spinnerCount; i++)
            {
                GameObject spinnerObj = new(bytes.ReadString8(ref index));
                spinnerObj.transform.SetParent(transform);
                spinnerObj.transform.SetPositionAndRotation(bytes.ReadVector3(ref index), 
                    Quaternion.Euler(0,((ushort)bytes.ReadShort(ref index)).ToFloatAng(),0));
                var spinner = spinnerObj.AddComponent<Spinner>();

                spinner.specialObject = new() { specialObject = bytes.ReadString8(ref index) };
                byte outputCount = bytes.ReadByte(ref index);
                byte flapCount = bytes.ReadByte(ref index);
                spinner.flapCount = flapCount;

                int unk1 = 0;
                if(version >= 3)
                {
                    unk1 = bytes.ReadInt(ref index);
                    spinner.outputStickTime = bytes.ReadFloat(ref index);
                }
                spinner.interactionOptions = unk1;

                if(version >= 4) spinner.animSpeed = bytes.ReadFloat(ref index);
                if (unk1 != 0)
                {
                    if (version >= 11) spinner.unknown4 = bytes.ReadShort(ref index);
                    else if (version >= 6) spinner.unknown4 = bytes.ReadByte(ref index);
                }

                //Support for versions <5 excluded here

                byte specObjVers = bytes.ReadByte(ref index);
                spinner.specialObjectVersion = specObjVers;
                byte specObjCount = bytes.ReadByte(ref index);
                spinner.animObjects = new Spinner.SpecialObject[specObjCount];
                for (int j = 0; j < specObjCount; j++)
                {
                    Spinner.SpecialObject specObj = new()
                    {
                        specialObject = new()
                        {
                            specialObject = bytes.ReadString8(ref index)
                        },
                        unknown1 = bytes.ReadFloat(ref index),
                        animationTime = bytes.ReadFloat(ref index),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = bytes.ReadInt(ref index);
                    spinner.animObjects[j] = specObj;
                }

                //Support for versions <7 excluded here

                spinner.outputStates = new float[outputCount];
                if (version >= 7)
                {
                    for(int j=0; j<outputCount; j++) spinner.outputStates[j] = bytes.ReadFloat(ref index);
                }

                if (version >= 8) spinner.unknown6 = bytes.ReadFloat(ref index);
                if (version >= 9) spinner.unknown7 = bytes.ReadFloat(ref index);

                if (version >= 10) spinner.unknownSpecialObject = new() { specialObject = bytes.ReadString8(ref index) };
                if (version >= 12) spinner.unknown9 = bytes.ReadFloat(ref index);
                if (version >= 13) spinner.unknown10 = bytes.ReadInt(ref index);

            }
        }
    }
}
#endif