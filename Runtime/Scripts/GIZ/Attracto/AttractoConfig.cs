#if UNITY_EDITOR
using NuGizWrap.Helper;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    public class AttractoConfig : GizmoTypeConfig
    {
        public override string ID => "Attracto";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LB1 => 3, _ => 1 };

        public int version;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int attractoCount = br.ReadInt32();

            List<string> existingNames = new();
            for (int i = 0; i < attractoCount; i++)
            {
                string name = br.ReadString(16);
                name = ObjectNames.GetUniqueName(existingNames.ToArray(), name);
                existingNames.Add(name);
                GameObject attractoObj = new(name);

                Transform attrTrans = attractoObj.transform;
                attrTrans.SetParent(parent);
                attrTrans.position = br.ReadVector3();
                attrTrans.eulerAngles = br.ReadYEuler();
                var attracto = attractoObj.AddComponent<Attracto>();

                attracto.pieceCount = br.ReadByte();
                if (version == 2) br.ReadString8(); //unused string8 property
            }

            return attractoCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var attractos = FindObjectsByType<Attracto>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int attractoCount = attractos.Length;
            bw.Write(attractoCount);

            for (int i = 0; i < attractoCount; i++)
            {
                var attracto = attractos[i];
                bw.WriteString(attracto.name, 16);

                Transform attrTrans = attracto.transform;
                bw.Write(attrTrans.position);
                bw.Write(attrTrans.eulerAngles.y.ToShortAng());

                bw.Write(attracto.pieceCount);
                if (version == 2) bw.Write(0); //unused string8 property
            }
        }
    }
}
#endif