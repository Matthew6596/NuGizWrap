//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.ohio83o5kwsd
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class PushBlocksSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 5, TTGame.LIJ1=>8, TTGame.LB1=>8, _ => 1 };

        public override string ID => "PushBlocks";

        public static PushBlocksSection Instance { get; private set; }

        public int version = 5;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var blocks = FindObjectsByType<PushBlocks>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int blockCount = blocks.Length;

            bytes.AddInt(blockCount);

            for(int i=0; i<blockCount; i++)
            {
                var block = blocks[i];

                bytes.AddString8(block.name);
                if (version >= 8) bytes.AddString8(block.specialObject.specialObject);
                bytes.AddFloat(block.snapRange);

                bytes.Add((byte)(block.pushLocation ? 1 : 0));
                bytes.Add((byte)(block.unknown1 ? 1 : 0));
                bytes.Add((byte)(block.lockZ ? 1 : 0));
                bytes.Add((byte)(block.lockX ? 1 : 0));
                if (version >= 4)
                {
                    bytes.Add((byte)(block.unknown2 ? 1 : 0));
                    bytes.Add((byte)(block.unknown3 ? 1 : 0));
                }
                if (version >= 5)
                {
                    bytes.Add((byte)(block.unknown4 ? 1 : 0));
                    bytes.Add((byte)(block.noSlip ? 1 : 0));
                }

                if (version >= 3)
                {
                    byte linkCount = (byte)block.linkObjects.Length;
                    bytes.Add(linkCount);
                    for (int j = 0; j < linkCount; j++) bytes.AddString8(block.linkObjects[j].specialObject);
                }

                if (version >= 6)
                {
                    bytes.Add((byte)(block.unknown5 ? 1 : 0));
                    bytes.AddFloat(block.unknown6);
                }

                if (version >= 7)
                {
                    bytes.Add((byte)(block.unknown7 ? 1 : 0));
                    bytes.Add((byte)(block.unknown8 ? 1 : 0));
                    bytes.Add((byte)(block.unknown9 ? 1 : 0));
                    bytes.Add((byte)(block.unknown10 ? 1 : 0));
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int blockCount = bytes.ReadInt(ref index);

            //Clear existing pushblocks before creating new ones
            foreach (var block in FindObjectsByType<PushBlocks>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) 
                block.gameObject.DelayDestroy();

            for (int i = 0; i < blockCount; i++)
            {
                GameObject blockObj = new(bytes.ReadString8(ref index));
                blockObj.transform.SetParent(transform);
                var block = blockObj.AddComponent<PushBlocks>();

                if (version >= 8) block.specialObject = new() { specialObject = bytes.ReadString8(ref index) };

                block.snapRange = bytes.ReadFloat(ref index);
                block.pushLocation = bytes.ReadByte(ref index) != 0;
                block.unknown1 = bytes.ReadByte(ref index) != 0;
                block.lockZ = bytes.ReadByte(ref index) != 0;
                block.lockX = bytes.ReadByte(ref index) != 0;
                if (version >= 4)
                {
                    block.unknown2 = bytes.ReadByte(ref index) != 0;
                    block.unknown3 = bytes.ReadByte(ref index) != 0;
                }
                if (version >= 5)
                {
                    block.unknown4 = bytes.ReadByte(ref index) != 0;
                    block.noSlip = bytes.ReadByte(ref index) != 0;
                }

                if (version >= 3)
                {
                    byte linkCount = bytes.ReadByte(ref index);
                    block.linkObjects = new GameScene.SpecialObjectReference[linkCount];
                    for(int j=0; j<linkCount; j++) block.linkObjects[j] = new() { specialObject = bytes.ReadString8(ref index) };
                }

                if (version >= 6)
                {
                    block.unknown5 = bytes.ReadByte(ref index) != 0;
                    block.unknown6 = bytes.ReadFloat(ref index);
                }

                if (version >= 7)
                {
                    block.unknown7 = bytes.ReadByte(ref index) != 0;
                    block.unknown8 = bytes.ReadByte(ref index) != 0;
                    block.unknown9 = bytes.ReadByte(ref index) != 0;
                    block.unknown10 = bytes.ReadByte(ref index) != 0;
                }
            }
        }
    }
}
#endif