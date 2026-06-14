//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.l9lqrtser1ok
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
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

                bytes.Add(ledge.unknown1);

                if (version >= 2)
                {
                    bytes.AddShort(ledge.unknown2);
                    bytes.AddShort(ledge.unknown3);
                }

                if (version >= 3) bytes.Add((byte)ledge.type);

                string unk4 = ledge.unknown4;
                if (version >= 4) bytes.AddString8(unk4);
                if (unk4.Length > 0)
                {
                    bytes.AddVector3(ledge.unknown4Pos);
                    bytes.AddShort(ledge.unknown4Ang);
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

            for(int i=0; i<ledgeCount; i++)
            {
                GameObject ledgeObj = new(bytes.ReadString(ref index, 8));
                ledgeObj.transform.SetParent(transform);
                ledgeObj.transform.position = bytes.ReadVector3(ref index);
                ledgeObj.transform.eulerAngles = new(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0);

                var ledge = ledgeObj.AddComponent<Ledge>();

                ledge.unknown1 = bytes.ReadByte(ref index);

                if (version >= 2)
                {
                    ledge.unknown2 = bytes.ReadShort(ref index);
                    ledge.unknown3 = bytes.ReadShort(ref index);
                }

                if (version >= 3)
                {
                    byte type = bytes.ReadByte(ref index);
                    //if (!Enum.IsDefined(typeof(Ledge.Type), (int)type)) Debug.Log($"Loading Unknown Ledge Type!: {type}, {(char)type}");
                    //ledge.type = (Ledge.Type)type;
                    ledge.type = type;
                }

                if (version >= 4) ledge.unknown4 = bytes.ReadString8(ref index);
                if (ledge.unknown4.Length > 0)
                {
                    ledge.unknown4Pos = bytes.ReadVector3(ref index);
                    ledge.unknown4Ang = bytes.ReadShort(ref index);
                }
            }
        }
    }
}
#endif