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

    public class ShardConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LB1 => 2, _ => 1 };

        public int version = 2;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int shardCount = br.ReadInt32();

            string[] existingNames = new string[shardCount];
            for (int i = 0; i < shardCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject shardObj = new(name);
                shardObj.transform.SetParent(parent);
                shardObj.transform.position = br.ReadVector3();
                var shard = shardObj.AddComponent<Shard>();

                if (version >= 2) shard.transform.eulerAngles = br.ReadXZEuler();
            }

            return shardCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var shards = FindObjectsByType<Shard>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int shardCount = shards.Length;
            bw.Write(shardCount);

            for (int i = 0; i < shardCount; i++)
            {
                var shard = shards[i];
                bw.WriteString(shard.name, 16);
                bw.Write(shard.transform.position);

                if (version >= 2)
                {
                    Vector3 euler = shard.transform.eulerAngles;
                    bw.Write(euler.x.ToShortAng());
                    bw.Write(euler.z.ToShortAng());
                }
            }
        }
    }
}
#endif