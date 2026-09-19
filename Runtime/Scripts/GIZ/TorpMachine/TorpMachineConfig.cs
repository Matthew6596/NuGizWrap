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

    public class TorpMachineConfig : GizmoTypeConfig
    {
        public override string ID => "Torp Machine";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.TCS, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 3, TTGame.LB1 => 4, _ => 1 };

        public int version = 3;
        public float scale;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int torpCount = br.ReadInt32();
            if (version >= 3) scale = br.ReadSingle();

            string[] existingNames = new string[torpCount];
            for (int i = 0; i < torpCount; i++)
            {
                string name = br.ReadString32();

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject torpObj = new(name);
                torpObj.transform.SetParent(parent);
                torpObj.transform.position = br.ReadVector3();
                torpObj.transform.eulerAngles = br.ReadYEuler();
                var torp = torpObj.AddComponent<TorpMachine>();
                if (version >= 3) torp.redOutline = br.ReadByte() != 0;
                if (version >= 4) torp.unknown1 = br.ReadInt16();
            }

            return torpCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var torps = FindObjectsByType<TorpMachine>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            var torpCount = torps.Length;

            bw.Write(torpCount);
            if (version >= 3) bw.Write(scale);

            for (int i = 0; i < torpCount; i++)
            {
                var torp = torps[i];
                bw.WriteString32(torp.name);
                bw.Write(torp.transform.position);
                bw.Write(torp.transform.eulerAngles.y.ToShortAng());
                if (version >= 2) bw.Write((byte)(torp.redOutline ? 1 : 0));
                if (version >= 4) bw.Write(torp.unknown1);
            }
        }
    }
}
#endif