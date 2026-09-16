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
    using UnityEngine.XR;

    public class SignalConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LB1 => 7, _ => 1 };

        public int version = 7;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int signalCount = br.ReadInt32();

            string[] existingNames = new string[signalCount];
            for (int i = 0; i < signalCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject signalObj = new(name);
                signalObj.transform.SetParent(parent);
                signalObj.transform.position = br.ReadVector3();
                var signal = signalObj.AddComponent<Signal>();

                signal.character = (Signal.Character)br.ReadByte();
                if (version >= 2)
                {
                    signal.suit = (Signal.Suit)br.ReadByte();
                    signal.transform.eulerAngles = br.ReadYEuler();
                }
                if (version >= 4)
                {
                    signal.unknown2 = br.ReadInt16();
                    signal.unknown3 = br.ReadVector3();
                }
                if (version >= 5) signal.unknown4 = br.ReadString8();

                if (version >= 7)
                {
                    byte unk5Count = br.ReadByte();
                    signal.unknown5 = new string[unk5Count];
                    for (int j = 0; j < unk5Count; j++) signal.unknown5[j] = br.ReadString8();
                }
            }

            return signalCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var signals = FindObjectsByType<Signal>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int signalCount = signals.Length;
            bw.Write(signalCount);

            for (int i = 0; i < signalCount; i++)
            {
                var signal = signals[i];

                bw.WriteString(signal.name, 16);
                bw.Write(signal.transform.position);
                bw.Write((byte)signal.character);
                if (version >= 2)
                {
                    bw.Write((byte)signal.suit);
                    bw.Write(signal.transform.eulerAngles.y.ToShortAng());
                }
                if (version >= 4)
                {
                    bw.Write(signal.unknown2);
                    bw.Write(signal.unknown3);
                }
                if (version >= 5) bw.WriteString8(signal.unknown4);

                if (version >= 7)
                {
                    byte unk5Count = (byte)signal.unknown5.Length;
                    bw.Write(unk5Count);
                    for (int j = 0; j < unk5Count; j++) bw.WriteString8(signal.unknown5[j]);
                }
            }
        }
    }
}
#endif