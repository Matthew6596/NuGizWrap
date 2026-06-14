//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.30em1qdevmqw
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class WhipperSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 4, _ => 1 };

        public override string ID => "Whipper";
        public static WhipperSection Instance { get; private set; }

        public int version = 4;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var whippers = FindObjectsByType<Whipper>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int whipperCount = whippers.Length;
            bytes.AddInt(whipperCount);

            for(int i=0; i<whipperCount; i++)
            {
                var whipper = whippers[i];
                bytes.AddFixedString(whipper.name, 16);
                bytes.AddVector3(whipper.transform.position);

                bytes.AddFloat(whipper.unknown1);
                bytes.AddShort((short)whipper.transform.eulerAngles.y.ToShortAng());

                bytes.Add(whipper.unknown2);
                if (version >= 2) bytes.Add(whipper.unknown3);
                if (version >= 3) bytes.AddFloat(whipper.unknown4);
                if (version >= 4) bytes.AddFixedString(whipper.gizObstacle.gizObstacle, 16);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int whipperCount = bytes.ReadInt(ref index);

            //Clear existing whippers before creating new ones
            foreach (var whipper in FindObjectsByType<Whipper>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) 
                whipper.gameObject.DelayDestroy();

            for (int i = 0; i < whipperCount; i++)
            {
                GameObject whipperObj = new(bytes.ReadString(ref index, 16));
                whipperObj.transform.SetParent(transform);
                whipperObj.transform.position = bytes.ReadVector3(ref index);
                var whipper = whipperObj.AddComponent<Whipper>();

                whipper.unknown1 = bytes.ReadFloat(ref index);
                whipper.transform.eulerAngles = new(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0);

                whipper.unknown2 = bytes.ReadByte(ref index);
                if (version >= 2) whipper.unknown3 = bytes.ReadByte(ref index);
                if (version >= 3) whipper.unknown4 = bytes.ReadFloat(ref index);
                if (version >= 4) whipper.gizObstacle = new() { gizObstacle = bytes.ReadString(ref index, 16) };
            }
        }
    }
}
#endif