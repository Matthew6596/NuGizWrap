#if UNITY_EDITOR
using System;
using System.IO;
using TTModdingKit.Helper;
using UnityEngine;

namespace TTModdingKit.AI
{
    public class AIPath : MonoBehaviour
    {
        public AIPathConnection[] connections;
        public AIPathPoint[] points;
        public AIPathRoute[] routes;

        public byte unk3;
        public Unk116[] unk116;
        public Unk36[] unk36;

        public void FromBytes(BinaryReader br, int version)
        {
            name = br.ReadString(16).Trim();

            byte pointCount = br.ReadByte();
            points = new AIPathPoint[pointCount];

            unk3 = br.ReadByte();

            short connectionCount = version == 1 ? br.ReadByte() : br.ReadInt16();
            connections = new AIPathConnection[connectionCount];

            for (int j = 0; j < connectionCount; j++)
            {
                GameObject connObj = new("ai_path_connection");
                connObj.transform.SetParent(transform);
                connections[j] = connObj.AddComponent<AIPathConnection>();
                connections[j].FromBytes(br, version);
            }

            if (version == 1) br.ReadByte(); //padding

            for (int j = 0; j < pointCount; j++)
            {
                GameObject pointObj = new("ai_path_point");
                pointObj.transform.SetParent(transform);
                points[j] = pointObj.AddComponent<AIPathPoint>();
                points[j].FromBytes(br, version);
            }

            unk116 = new Unk116[pointCount];
            for (int j = 0; j < pointCount; j++)
            {
                unk116[j] = new Unk116() { unk116 = br.ReadBytes(pointCount) };
            }

            if (version >= 5)
            {
                byte routeCount = br.ReadByte();
                for (int j = 0; j < routeCount; j++)
                {
                    GameObject routeObj = new("ai_path_route");
                    routeObj.transform.SetParent(transform);
                    routes[j] = routeObj.AddComponent<AIPathRoute>();
                    routes[j].FromBytes(br, pointCount);
                }
            }

            if (version >= 19)
            {
                byte unk36Count = br.ReadByte();
                unk36 = new Unk36[unk36Count];
                for (int j = 0; j < unk36Count; j++)
                {
                    unk36[j] = new()
                    {
                        unk37 = br.ReadByte(),
                        unk38 = br.ReadInt16(),
                    };
                }
            }
        }

        public void ToBytes(BinaryWriter bw)
        {

        }

        [Serializable]
        public struct Unk116
        {
            public byte[] unk116;
        }

        [Serializable]
        public struct Unk36
        {
            public byte unk37;
            public short unk38;
        }
    }
}
#endif