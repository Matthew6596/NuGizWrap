//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.8b65i2n22n5j
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class MiniCutSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 1, TTGame.LIJ1 => 1, TTGame.LB1 => 1, _ => 1 };

        public override string ID => "MiniCut";

        public static MiniCutSection Instance { get; private set; }

        public int version = 1;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var minicuts = FindObjectsByType<MiniCut>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int minicutCount = minicuts.Length;
            bytes.AddInt(minicutCount);

            for(int i=0; i<minicutCount; i++)
            {
                var minicut = minicuts[i];

                bytes.AddString8(minicut.name);
                bytes.AddFloat(minicut.startDelay);
                bytes.AddFloat(minicut.duration);
                bytes.AddFloat(minicut.blendInTime);
                bytes.AddFloat(minicut.blendOutTime);
                bytes.AddFloat(minicut.maxTotalDuration);

                byte partsCount = (byte)minicut.miniCutParts.Length;
                bytes.Add(partsCount);

                for(int j=0; j<partsCount; j++)
                {
                    var part = minicut.miniCutParts[j];
                    bytes.AddString8(part.name);
                    bytes.AddVector3(part.targetPosition);
                    bytes.AddFloat(part.cameraDistance);
                    short pitch = (short)part.cameraOrbitEuler.x.ToShortAng();
                    short yaw = (short)part.cameraOrbitEuler.y.ToShortAng();
                    short roll = (short)part.cameraOrbitEuler.z.ToShortAng();
                    bytes.AddShort(pitch);
                    bytes.AddShort(yaw);
                    bytes.AddShort(roll);
                    bytes.AddFloat(part.easeInTime);
                    bytes.AddFloat(part.duration);
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int minicutCount = bytes.ReadInt(ref index);

            //Clear old minicuts before creating new ones
            foreach (var cut in FindObjectsByType<MiniCut>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) cut.gameObject.DelayDestroy();

            for(int i=0; i<minicutCount; i++)
            {
                GameObject minicutObj = new(bytes.ReadString8(ref index));
                minicutObj.transform.SetParent(transform);
                var minicut = minicutObj.AddComponent<MiniCut>();

                minicut.startDelay = bytes.ReadFloat(ref index);
                minicut.duration = bytes.ReadFloat(ref index);
                minicut.blendInTime = bytes.ReadFloat(ref index);
                minicut.blendOutTime = bytes.ReadFloat(ref index);
                minicut.maxTotalDuration = bytes.ReadFloat(ref index);

                byte partsCount = bytes.ReadByte(ref index);
                minicut.miniCutParts = new MiniCut.MiniCutPart[partsCount];

                for(int j=0; j<partsCount; j++)
                {
                    MiniCut.MiniCutPart part = new()
                    {
                        name = bytes.ReadString8(ref index),
                        targetPosition = bytes.ReadVector3(ref index),
                        cameraDistance = bytes.ReadFloat(ref index),
                        cameraOrbitEuler = new(
                            ((ushort)bytes.ReadShort(ref index)).ToFloatAng(),
                            ((ushort)bytes.ReadShort(ref index)).ToFloatAng(),
                            ((ushort)bytes.ReadShort(ref index)).ToFloatAng()
                            ),
                        easeInTime = bytes.ReadFloat(ref index),
                        duration = bytes.ReadFloat(ref index),
                    };
                    minicut.miniCutParts[j] = part;
                }
            }
        }
    }
}
#endif