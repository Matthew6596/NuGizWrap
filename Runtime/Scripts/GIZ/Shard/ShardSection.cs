//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.pelmmimizw2h
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class ShardSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LB1 => 2, _ => 1 };

        public override string ID => "Shard";
        public static ShardSection Instance { get; private set; }

        public int version = 2;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var shards = FindObjectsByType<Shard>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int shardCount = shards.Length;
            bytes.AddInt(shardCount);

            for(int i=0; i<shardCount; i++)
            {
                var shard = shards[i];
                bytes.AddFixedString(shard.name, 16);
                bytes.AddVector3(shard.transform.position);

                if (version >= 2)
                {
                    bytes.AddShort(shard.unknown1);
                    bytes.AddShort(shard.unknown2);
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int shardCount = bytes.ReadInt(ref index);

            //Clear existing shards before creating new ones
            foreach (var shard in FindObjectsByType<Shard>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) shard.gameObject.DelayDestroy();

            for(int i=0; i<shardCount; i++)
            {
                GameObject shardObj = new(bytes.ReadString(ref index, 16));
                shardObj.transform.SetParent(transform);
                shardObj.transform.position = bytes.ReadVector3(ref index);
                var shard = shardObj.AddComponent<Shard>();

                if (version >= 2)
                {
                    shard.unknown1 = bytes.ReadShort(ref index);
                    shard.unknown2 = bytes.ReadShort(ref index);
                }
            }
        }
    }
}
#endif