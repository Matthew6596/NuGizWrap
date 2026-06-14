//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.c39x4gm9wkq
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class SignalSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LB1 => 7, _ => 1 };

        public override string ID => "Signal";
        public static SignalSection Instance { get; private set; }

        public int version = 7;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var signals = FindObjectsByType<Signal>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int signalCount = signals.Length;
            bytes.AddInt(signalCount);

            for (int i = 0; i < signalCount; i++)
            {
                var signal = signals[i];

                bytes.AddFixedString(signal.name, 16);
                bytes.AddVector3(signal.transform.position);
                bytes.Add((byte)signal.character);
                if (version >= 2)
                {
                    bytes.Add((byte)signal.suit);
                    bytes.AddShort((short)signal.transform.eulerAngles.y.ToShortAng());
                }
                if (version >= 4)
                {
                    bytes.AddShort(signal.unknown2);
                    bytes.AddVector3(signal.unknown3);
                }
                if (version >= 5) bytes.AddString8(signal.unknown4);

                if (version >= 7)
                {
                    byte unk5Count = (byte)signal.unknown5.Length;
                    bytes.Add(unk5Count);
                    for (int j = 0; j < unk5Count; j++) bytes.AddString8(signal.unknown5[j]);
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int signalCount = bytes.ReadInt(ref index);

            foreach (var signal in FindObjectsByType<Signal>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                signal.gameObject.DelayDestroy();

            for (int i = 0; i < signalCount; i++)
            {
                GameObject signalObj = new(bytes.ReadString(ref index, 16));
                signalObj.transform.SetParent(transform);
                signalObj.transform.position = bytes.ReadVector3(ref index);
                var signal = signalObj.AddComponent<Signal>();

                signal.character = (Signal.Character)bytes.ReadByte(ref index);
                if (version >= 2)
                {
                    signal.suit = (Signal.Suit)bytes.ReadByte(ref index);
                    signal.transform.eulerAngles = new(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0);
                }
                if (version >= 4)
                {
                    signal.unknown2 = bytes.ReadShort(ref index);
                    signal.unknown3 = bytes.ReadVector3(ref index);
                }
                if (version >= 5) signal.unknown4 = bytes.ReadString8(ref index);

                if (version >= 7)
                {
                    byte unk5Count = bytes.ReadByte(ref index);
                    signal.unknown5 = new string[unk5Count];
                    for (int j = 0; j < unk5Count; j++) signal.unknown5[j] = bytes.ReadString8(ref index);
                }
            }
        }
    }
}
#endif