//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.r8uqyeu23ff
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class TightRopeSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LB1 => 4, _ => 1 };

        public override string ID => "TightRope";
        public static TightRopeSection Instance { get; private set; }

        public int version = 4;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var ropes = FindObjectsByType<TightRope>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int ropeCount = ropes.Length;
            bytes.AddInt(ropeCount);

            for(int i=0; i<ropeCount; i++)
            {
                var rope = ropes[i];
                bytes.AddFixedString(rope.name, 16);

                bytes.AddVector3(rope.unknown1);
                bytes.AddVector3(rope.unknown2);
                if (version >= 4)
                {
                    bytes.AddVector3(rope.unknown3);
                    bytes.AddVector3(rope.unknown4);
                }

                if (version >= 2)
                {
                    bytes.AddShort(rope.unknown5);
                    bytes.AddShort(rope.unknown6);
                    bytes.AddShort(rope.unknown7);
                    bytes.AddShort(rope.unknown8);

                    bytes.Add(rope.unknown9);
                    bytes.Add(rope.unknown10);
                }

                if (version >= 3) bytes.Add(rope.unknown11);

            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int ropeCount = bytes.ReadInt(ref index);

            //Clear existing tightropes before creating new ones
            foreach (var ropes in FindObjectsByType<TightRope>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) ropes.gameObject.DelayDestroy();

            for(int i=0; i<ropeCount; i++)
            {
                GameObject ropeObj = new(bytes.ReadString(ref index, 16));
                ropeObj.transform.SetParent(transform);
                var rope = ropeObj.AddComponent<TightRope>();

                rope.unknown1 = bytes.ReadVector3(ref index);
                rope.unknown2 = bytes.ReadVector3(ref index);
                if (version >= 4)
                {
                    rope.unknown3 = bytes.ReadVector3(ref index);
                    rope.unknown4 = bytes.ReadVector3(ref index);
                }

                if (version >= 2)
                {
                    rope.unknown5 = bytes.ReadShort(ref index);
                    rope.unknown6 = bytes.ReadShort(ref index);
                    rope.unknown7 = bytes.ReadShort(ref index);
                    rope.unknown8 = bytes.ReadShort(ref index);

                    rope.unknown9 = bytes.ReadByte(ref index);
                    rope.unknown10 = bytes.ReadByte(ref index);
                }

                if (version >= 3) rope.unknown11 = bytes.ReadByte(ref index);
            }
        }
    }
}
#endif