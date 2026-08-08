//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.vnnuwt6ujwmg
//-Matton
//===== ===== ===== ===== =====

#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class TorpMachineSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.TCS, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 3, TTGame.LB1 => 4, _ => 1 };

        public override string ID => "Torp Machine";

        public static TorpMachineSection Instance { get; private set; }

        public int version = 3;
        public float scale;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();

            bytes.AddInt(version);

            var torps = FindObjectsByType<TorpMachine>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            var torpCount = torps.Length;

            bytes.AddInt(torpCount);
            if(version >= 3) bytes.AddFloat(scale);

            for(int i=0; i<torpCount; i++)
            {
                var torp = torps[i];
                bytes.AddString32(torp.name);
                bytes.AddVector3(torp.transform.position);
                bytes.AddShort((short)torp.transform.eulerAngles.y.ToShortAng());
                if (version >= 2) bytes.Add((byte)(torp.redOutline ? 1 : 0));
                if (version >= 4) bytes.AddShort(torp.unknown1);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int torpCount = bytes.ReadInt(ref index);
            if(version >= 3) scale = bytes.ReadFloat(ref index);

            //Destroy existing torps before adding new ones
            foreach (var torp in FindObjectsByType<TorpMachine>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) torp.gameObject.DelayDestroy();

            for(int i=0; i<torpCount; i++)
            {
                GameObject torpObj = new(bytes.ReadString32(ref index));
                torpObj.transform.SetParent(transform);
                torpObj.transform.position = bytes.ReadVector3(ref index);
                torpObj.transform.eulerAngles = new(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0);
                var torp = torpObj.AddComponent<TorpMachine>();
                if (version >= 3) torp.redOutline = bytes.ReadByte(ref index) != 0;
                if (version >= 4) torp.unknown1 = bytes.ReadShort(ref index);
            }
        }
    }
}
#endif