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

    public class LedgeConfig : GizmoTypeConfig
    {
        public override string ID => "Ledge";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 5, TTGame.LB1 => 5, _ => 1 };

        public int version = 5;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int ledgeCount = br.ReadInt32();

            string[] existingNames = new string[ledgeCount];
            List<(Ledge, short, short)> ledgeConnections = new();

            for (int i = 0; i < ledgeCount; i++)
            {
                string name = br.ReadString(8);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject ledgeObj = new(name);
                ledgeObj.transform.SetParent(parent);
                ledgeObj.transform.position = br.ReadVector3();
                ledgeObj.transform.eulerAngles = br.ReadYEuler();
                var ledge = ledgeObj.AddComponent<Ledge>();

                byte typeByte = br.ReadByte();
                ledge.type = Enum.IsDefined(typeof(Ledge.Type), (int)typeByte) ? (Ledge.Type)typeByte : Ledge.Type.Two;

                //Add connection indicies to list to connect after all ledges are created.
                if (version >= 2) ledgeConnections.Add((ledge, br.ReadInt16(), br.ReadInt16()));

                if (version >= 3) ledge.interactOptions = br.ReadByte();

                string specObj = "";
                if (version >= 4)
                {
                    specObj = br.ReadString8();
                    ledge.specialObject = new() { specialObject = specObj };
                }
                if (specObj.Length > 0)
                {
                    ledge.specialObjectPos = br.ReadVector3();
                    ledge.specialObjectAng = br.ReadInt16();
                }
            }

            if (version >= 2)
            {
                Ledge GetLedge(short ind) => ind >= 0 && ind < ledgeCount ? ledgeConnections[ind].Item1 : null;

                //Connect ledges based on read left/right index
                for (int i = 0; i < ledgeCount; i++)
                {
                    var ledgeItems = ledgeConnections[i];
                    var ledge = ledgeItems.Item1;
                    ledge.leftLedge = GetLedge(ledgeItems.Item2);
                    ledge.rightLedge = GetLedge(ledgeItems.Item3);
                }
            }

            return ledgeCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var ledges = FindObjectsByType<Ledge>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int ledgeCount = ledges.Length;
            bw.Write(ledgeCount);

            for (int i = 0; i < ledgeCount; i++)
            {
                var ledge = ledges[i];
                bw.WriteString(ledge.name, 8);
                bw.Write(ledge.transform.position);
                bw.Write(ledge.transform.eulerAngles.y.ToShortAng());

                bw.Write((byte)ledge.type);

                if (version >= 2)
                {
                    bw.Write((short)Array.IndexOf(ledges, ledge.leftLedge));
                    bw.Write((short)Array.IndexOf(ledges, ledge.rightLedge));
                }

                if (version >= 3) bw.Write(ledge.interactOptions);

                string specObj = ledge.specialObject.specialObject;
                if (version >= 4)
                {
                    bw.WriteString8(specObj);
                    if (specObj.Length > 0)
                    {
                        bw.Write(ledge.specialObjectPos);
                        bw.Write(ledge.specialObjectAng);
                    }
                }
            }
        }
    }
}
#endif