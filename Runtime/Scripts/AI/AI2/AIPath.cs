#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using NuGizWrap.Helper;
using UnityEditor;
using UnityEngine;

namespace NuGizWrap.AI
{
    public class AIPath : MonoBehaviour
    {
        public AIPathConnection[] connections;
        public AIPathPoint[] points;
        public AIPathRoute[] routes;

        public byte unk3;
        public Unk116[] unk116;
        public Unk36[] unk36;

        private void OnValidate()
        {
            if (points != null && points.Length > 255)
            {
                points = points.Take(255).ToArray();
                EditorUtility.DisplayDialog("Max Points", "AIPath can only have a maximum of 255 points.", "OK");
            }

            if (connections != null && connections.Length > short.MaxValue)
            {
                connections = connections.Take(short.MaxValue).ToArray();
                EditorUtility.DisplayDialog("Max Connections", $"AIPath can only have a maximum of {short.MaxValue} connections.", "OK");
            }

            if (routes != null && routes.Length > 255)
            {
                routes = routes.Take(255).ToArray();
                EditorUtility.DisplayDialog("Max Routes", "AIPath can only have a maximum of 255 routes.", "OK");
            }

            if (unk36 != null && unk36.Length > 255)
            {
                unk36 = unk36.Take(255).ToArray();
                EditorUtility.DisplayDialog("Max Unk36", "AIPath can only have a maximum of 255 Unk36.", "OK");
            }
        }

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
                routes = new AIPathRoute[routeCount];
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

        public void ToBytes(BinaryWriter bw, int version)
        {
            bw.WriteString(name, 16);

            byte pointCount = (byte)points.Length;
            bw.Write(pointCount);

            bw.Write(unk3);

            var connectionsCount = version == 1 ? (byte)connections.Length : (short)connections.Length;
            if (version == 1) bw.Write((byte)connectionsCount);
            else bw.Write((short)connectionsCount);

            for(int i=0; i<connectionsCount; i++) connections[i].ToBytes(bw, version);

            if (version == 1) bw.Write((byte)0); //padding

            for (int i = 0; i < pointCount; i++) points[i].ToBytes(bw, version);
            for (int i = 0; i < pointCount; i++) bw.Write(unk116[i].unk116);

            if (version >= 5)
            {
                byte routeCount = (byte)routes.Length;
                bw.Write(routeCount);
                for (int i = 0; i < routeCount; i++) routes[i].ToBytes(bw, pointCount);
            }

            if (version >= 19)
            {
                byte unk36Count = (byte)unk36.Length;
                bw.Write(unk36Count);
                for (int i = 0; i < unk36Count; i++)
                {
                    bw.Write(unk36[i].unk37);
                    bw.Write(unk36[i].unk38);
                }
            }
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