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

    public class MiniCutConfig : GizmoTypeConfig
    {
        public override string ID => "MiniCut";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 1, TTGame.LIJ1 => 1, TTGame.LB1 => 1, _ => 1 };

        public int version = 1;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int minicutCount = br.ReadInt32();

            string[] existingNames = new string[minicutCount];
            for (int i = 0; i < minicutCount; i++)
            {
                string name = br.ReadString8();

                name = ObjectNames.GetUniqueName(existingNames, name.Trim() == "" ? "minicut" : name);
                existingNames[i] = name;

                GameObject minicutObj = new(name);
                minicutObj.transform.SetParent(parent);
                var minicut = minicutObj.AddComponent<MiniCut>();

                minicut.startDelay = br.ReadSingle();
                minicut.duration = br.ReadSingle();
                minicut.blendInTime = br.ReadSingle();
                minicut.blendOutTime = br.ReadSingle();
                minicut.maxTotalDuration = br.ReadSingle();

                byte partsCount = br.ReadByte();
                minicut.miniCutParts = new MiniCut.MiniCutPart[partsCount];

                for (int j = 0; j < partsCount; j++)
                {
                    MiniCut.MiniCutPart part = new()
                    {
                        name = br.ReadString8(),
                        targetPosition = br.ReadVector3(),
                        cameraDistance = br.ReadSingle(),
                        cameraOrbitEuler = new(
                            br.ReadUInt16().ToFloatAng(),
                            br.ReadUInt16().ToFloatAng(),
                            br.ReadUInt16().ToFloatAng()
                            ),
                        easeInTime = br.ReadSingle(),
                        duration = br.ReadSingle(),
                    };
                    minicut.miniCutParts[j] = part;
                }
            }

            return minicutCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var minicuts = FindObjectsByType<MiniCut>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int minicutCount = minicuts.Length;
            bw.Write(minicutCount);

            for (int i = 0; i < minicutCount; i++)
            {
                var minicut = minicuts[i];

                bw.WriteString8(minicut.name);
                bw.Write(minicut.startDelay);
                bw.Write(minicut.duration);
                bw.Write(minicut.blendInTime);
                bw.Write(minicut.blendOutTime);
                bw.Write(minicut.maxTotalDuration);

                byte partsCount = (byte)minicut.miniCutParts.Length;
                bw.Write(partsCount);

                for (int j = 0; j < partsCount; j++)
                {
                    var part = minicut.miniCutParts[j];
                    bw.WriteString8(part.name);
                    bw.Write(part.targetPosition);
                    bw.Write(part.cameraDistance);
                    short pitch = (short)part.cameraOrbitEuler.x.ToShortAng();
                    short yaw = (short)part.cameraOrbitEuler.y.ToShortAng();
                    short roll = (short)part.cameraOrbitEuler.z.ToShortAng();
                    bw.Write(pitch);
                    bw.Write(yaw);
                    bw.Write(roll);
                    bw.Write(part.easeInTime);
                    bw.Write(part.duration);
                }
            }
        }
    }
}
#endif