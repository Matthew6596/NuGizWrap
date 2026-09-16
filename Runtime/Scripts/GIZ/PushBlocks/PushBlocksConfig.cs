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

    public class PushBlocksConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 5, TTGame.LIJ1 => 8, TTGame.LB1 => 8, _ => 1 };

        public int version = 5;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int blockCount = br.ReadInt32();

            string[] existingNames = new string[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                string name = br.ReadString8();

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject blockObj = new(name);
                blockObj.transform.SetParent(parent);
                var block = blockObj.AddComponent<PushBlocks>();

                if (version >= 8) block.specialObject = new() { specialObject = br.ReadString8() };

                block.snapRange = br.ReadSingle();
                block.pushLocation = br.ReadByte() != 0;
                block.unknown1 = br.ReadByte() != 0;
                block.lockZ = br.ReadByte() != 0;
                block.lockX = br.ReadByte() != 0;
                if (version >= 4)
                {
                    block.unknown2 = br.ReadByte() != 0;
                    block.unknown3 = br.ReadByte() != 0;
                }
                if (version >= 5)
                {
                    block.unknown4 = br.ReadByte() != 0;
                    block.noSlip = br.ReadByte() != 0;
                }

                if (version >= 3)
                {
                    byte linkCount = br.ReadByte();
                    block.linkObjects = new GameScene.SpecialObjectReference[linkCount];
                    for (int j = 0; j < linkCount; j++) block.linkObjects[j] = new() { specialObject = br.ReadString8() };
                }

                if (version >= 6)
                {
                    block.unknown5 = br.ReadByte() != 0;
                    block.unknown6 = br.ReadSingle();
                }

                if (version >= 7)
                {
                    block.unknown7 = br.ReadByte() != 0;
                    block.unknown8 = br.ReadByte() != 0;
                    block.unknown9 = br.ReadByte() != 0;
                    block.unknown10 = br.ReadByte() != 0;
                }
            }

            return blockCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var blocks = FindObjectsByType<PushBlocks>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int blockCount = blocks.Length;

            bw.Write(blockCount);

            for (int i = 0; i < blockCount; i++)
            {
                var block = blocks[i];

                bw.WriteString8(block.name);
                if (version >= 8) bw.WriteString8(block.specialObject.specialObject);
                bw.Write(block.snapRange);

                bw.Write((byte)(block.pushLocation ? 1 : 0));
                bw.Write((byte)(block.unknown1 ? 1 : 0));
                bw.Write((byte)(block.lockZ ? 1 : 0));
                bw.Write((byte)(block.lockX ? 1 : 0));
                if (version >= 4)
                {
                    bw.Write((byte)(block.unknown2 ? 1 : 0));
                    bw.Write((byte)(block.unknown3 ? 1 : 0));
                }
                if (version >= 5)
                {
                    bw.Write((byte)(block.unknown4 ? 1 : 0));
                    bw.Write((byte)(block.noSlip ? 1 : 0));
                }

                if (version >= 3)
                {
                    byte linkCount = (byte)block.linkObjects.Length;
                    bw.Write(linkCount);
                    for (int j = 0; j < linkCount; j++) bw.WriteString8(block.linkObjects[j].specialObject);
                }

                if (version >= 6)
                {
                    bw.Write((byte)(block.unknown5 ? 1 : 0));
                    bw.Write(block.unknown6);
                }

                if (version >= 7)
                {
                    bw.Write((byte)(block.unknown7 ? 1 : 0));
                    bw.Write((byte)(block.unknown8 ? 1 : 0));
                    bw.Write((byte)(block.unknown9 ? 1 : 0));
                    bw.Write((byte)(block.unknown10 ? 1 : 0));
                }
            }
        }
    }
}
#endif