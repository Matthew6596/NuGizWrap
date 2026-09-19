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

    public class SpinnerConfig : GizmoTypeConfig
    {
        public override string ID => "Spinner";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 9, TTGame.LIJ1 => 11, TTGame.LB1 => 13, _ => 1 };

        public int version = 9;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int spinnerCount = br.ReadInt32();

            string[] existingNames = new string[spinnerCount];
            for (int i = 0; i < spinnerCount; i++)
            {
                string name = br.ReadString8();

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject spinnerObj = new(name);
                spinnerObj.transform.SetParent(parent);
                spinnerObj.transform.SetPositionAndRotation(br.ReadVector3(), Quaternion.Euler(br.ReadYEuler()));
                var spinner = spinnerObj.AddComponent<Spinner>();

                spinner.specialObject = new() { specialObject = br.ReadString8() };
                byte outputCount = br.ReadByte();
                byte flapCount = br.ReadByte();
                spinner.flapCount = flapCount;

                int unk1 = 0;
                if (version >= 3)
                {
                    unk1 = br.ReadInt32();
                    spinner.outputStickTime = br.ReadSingle();
                }
                spinner.interactionOptions = unk1;

                if (version >= 4) spinner.animSpeed = br.ReadSingle();
                if (unk1 != 0)
                {
                    if (version >= 11) spinner.unknown4 = br.ReadInt16();
                    else if (version >= 6) spinner.unknown4 = br.ReadByte();
                }

                //Support for versions <5 excluded here

                byte specObjVers = br.ReadByte();
                spinner.specialObjectVersion = specObjVers;
                byte specObjCount = br.ReadByte();
                spinner.animObjects = new Spinner.SpecialObject[specObjCount];
                for (int j = 0; j < specObjCount; j++)
                {
                    Spinner.SpecialObject specObj = new()
                    {
                        specialObject = new()
                        {
                            specialObject = br.ReadString8()
                        },
                        unknown1 = br.ReadSingle(),
                        animationTime = br.ReadSingle(),
                    };
                    if (specObjVers >= 2) specObj.unknown2 = br.ReadInt32();
                    spinner.animObjects[j] = specObj;
                }

                //Support for versions <7 excluded here

                spinner.outputStates = new float[outputCount];
                if (version >= 7)
                {
                    for (int j = 0; j < outputCount; j++) spinner.outputStates[j] = br.ReadSingle();
                }

                if (version >= 8) spinner.unknown6 = br.ReadSingle();
                if (version >= 9) spinner.unknown7 = br.ReadSingle();

                if (version >= 10) spinner.unknownSpecialObject = new() { specialObject = br.ReadString8() };
                if (version >= 12) spinner.unknown9 = br.ReadSingle();
                if (version >= 13) spinner.unknown10 = br.ReadInt32();
            }

            return spinnerCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var spinners = FindObjectsByType<Spinner>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int spinnerCount = spinners.Length;

            bw.Write(spinnerCount);

            for (int i = 0; i < spinnerCount; i++)
            {
                var spinner = spinners[i];

                bw.WriteString8(spinner.name);
                bw.Write(spinner.transform.position);
                bw.Write(spinner.transform.eulerAngles.y.ToShortAng());
                bw.WriteString8(spinner.specialObject.specialObject);
                byte outputCount = (byte)spinner.outputStates.Length;
                bw.Write(outputCount);

                byte flapCount = spinner.flapCount;
                if (version >= 2) bw.Write(flapCount);

                int unk1 = spinner.interactionOptions;
                if (version >= 3)
                {
                    bw.Write(unk1);
                    bw.Write(spinner.outputStickTime);
                }
                if (version >= 4) bw.Write(spinner.animSpeed);
                if (unk1 != 0)
                {
                    if (version >= 11) bw.Write(spinner.unknown4);
                    else if (version >= 6) bw.Write((byte)spinner.unknown4);
                }

                //Support for versions <5 excluded here

                byte specObjVers = spinner.specialObjectVersion;
                byte specObjCount = (byte)spinner.animObjects.Length;
                bw.Write(specObjVers);
                bw.Write(specObjCount);
                for (int j = 0; j < specObjCount; j++)
                {
                    var specObj = spinner.animObjects[j];
                    bw.WriteString8(specObj.specialObject.specialObject);
                    bw.Write(specObj.unknown1);
                    bw.Write(specObj.animationTime);
                    if (specObjVers >= 2) bw.Write(specObj.unknown2);
                }

                //Support for versions <7 excluded here

                if (version >= 7)
                {
                    for (int j = 0; j < outputCount; j++) bw.Write(spinner.outputStates[j]);
                }

                if (version >= 8) bw.Write(spinner.unknown6);
                if (version >= 9) bw.Write(spinner.unknown7);

                if (version >= 10) bw.WriteString8(spinner.unknownSpecialObject.specialObject);
                if (version >= 12) bw.Write(spinner.unknown9);
                if (version >= 13) bw.Write(spinner.unknown10);
            }
        }
    }
}
#endif