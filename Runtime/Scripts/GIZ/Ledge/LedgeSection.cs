//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.l9lqrtser1ok
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System;
    using System.Linq;

    public class LedgeSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 5, TTGame.LB1 => 5, _ => 1 };

        public override string ID => "Ledge";
        public static LedgeSection Instance { get; private set; }

        public int version = 5;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var ledges = FindObjectsByType<Ledge>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int ledgeCount = ledges.Length;
            bytes.AddInt(ledgeCount);

            for(int i=0; i<ledgeCount; i++)
            {
                var ledge = ledges[i];
                bytes.AddFixedString(ledge.name, 8);
                bytes.AddVector3(ledge.transform.position);
                bytes.AddShort((short)ledge.transform.eulerAngles.y.ToShortAng());

                bytes.Add((byte)ledge.type);

                if (version >= 2)
                {
                    bytes.AddShort((short)Array.IndexOf(ledges, ledge.leftLedge));
                    bytes.AddShort((short)Array.IndexOf(ledges, ledge.rightLedge));
                }

                if (version >= 3) bytes.Add(ledge.interactOptions);

                string specObj = ledge.specialObject.specialObject;
                if (version >= 4)
                {
                    bytes.AddString8(specObj);
                    if (specObj.Length > 0)
                    {
                        bytes.AddVector3(ledge.specialObjectPos);
                        bytes.AddShort(ledge.specialObjectAng);
                    }
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int ledgeCount = bytes.ReadInt(ref index);

            //Clear existing ledges before creating new ones
            foreach (var ledge in FindObjectsByType<Ledge>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) ledge.gameObject.DelayDestroy();

            List<(Ledge, short, short)> ledgeConnections = new();

            for (int i = 0; i < ledgeCount; i++)
            {
                GameObject ledgeObj = new(bytes.ReadString(ref index, 8));
                ledgeObj.transform.SetParent(transform);
                ledgeObj.transform.position = bytes.ReadVector3(ref index);
                ledgeObj.transform.eulerAngles = new(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0);

                var ledge = ledgeObj.AddComponent<Ledge>();

                byte typeByte = bytes.ReadByte(ref index);
                ledge.type = Enum.IsDefined(typeof(Ledge.Type), (int)typeByte) ? (Ledge.Type)typeByte : Ledge.Type.Two;

                //Add connection indicies to list to connect after all ledges are created.
                if (version >= 2) ledgeConnections.Add((ledge, bytes.ReadShort(ref index), bytes.ReadShort(ref index)));

                if (version >= 3) ledge.interactOptions = bytes.ReadByte(ref index);

                string specObj = "";
                if (version >= 4)
                {
                    specObj = bytes.ReadString8(ref index);
                    ledge.specialObject = new() { specialObject = specObj };
                }
                if (specObj.Length > 0)
                {
                    ledge.specialObjectPos = bytes.ReadVector3(ref index);
                    ledge.specialObjectAng = bytes.ReadShort(ref index);
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
        }
    }
}
#endif