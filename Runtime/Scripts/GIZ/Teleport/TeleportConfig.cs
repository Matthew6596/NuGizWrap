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

    public class TeleportConfig : GizmoTypeConfig
    {
        public override string ID => "Teleport";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 2, _ => 2 };

        public int version = 2;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = 2;
            br.ReadInt32();
            int tpCount = br.ReadInt32();

            string[] existingNames = new string[tpCount];
            for (int i = 0; i < tpCount; i++)
            {
                string name = br.ReadString8();

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject tpObj = new(name);
                tpObj.transform.SetParent(parent);
                var tp = tpObj.AddComponent<Teleport>();

                var h1 = new GameObject("hatch_1").AddComponent<TeleportHatch>();
                h1.transform.SetParent(tp.transform);
                tp.hatch1 = h1;

                var h2 = new GameObject("hatch_2").AddComponent<TeleportHatch>();
                h2.transform.SetParent(tp.transform);
                tp.hatch2 = h2;

                tp.hatchBaseSpecialObject = br.ReadString8();
                h1.flapSpecialObject = br.ReadString8();
                h2.flapSpecialObject = br.ReadString8();

                tp.unknown4 = br.ReadVector3();
                tp.unknown5 = br.ReadVector3();

                tp.unknown6 = br.ReadSingle();
                tp.unknown7 = br.ReadSingle();
                h1.flapYOffset = br.ReadSingle();
                h2.flapYOffset = br.ReadSingle();
                tp.unknown10 = br.ReadSingle();
                tp.unknown11 = br.ReadSingle();

                h1.transform.eulerAngles = br.ReadYEuler();
                h2.transform.eulerAngles = br.ReadYEuler();
                tp.unknown14 = br.ReadInt16();

                h1.transform.position = br.ReadVector3();
                h2.transform.position = br.ReadVector3();
            }

            return tpCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var tps = FindObjectsByType<Teleport>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int tpCount = tps.Length;
            bw.Write(tpCount);

            for (int i = 0; i < tpCount; i++)
            {
                var tp = tps[i];
                bw.WriteString8(tp.name);

                var h1 = tp.hatch1;
                var h2 = tp.hatch2;
                var h1T = h1.transform;
                var h2T = h2.transform;

                string h1SpecObj = "Flap_01";
                float flap1Y = 0.25f;
                short h1Ang = 0;
                Vector3 h1Pos = tp.unknown4;
                if (h1 != null)
                {
                    h1SpecObj = h1.flapSpecialObject;
                    flap1Y = h1.flapYOffset;
                    h1Ang = (short)h1T.eulerAngles.y.ToShortAng();
                    h1Pos = h1T.position;
                }

                string h2SpecObj = "Flap_01";
                float flap2Y = 0.25f;
                short h2Ang = 0;
                Vector3 h2Pos = tp.unknown5;
                if (h2 != null)
                {
                    h2SpecObj = h2.flapSpecialObject;
                    flap2Y = h2.flapYOffset;
                    h2Ang = (short)h2T.eulerAngles.y.ToShortAng();
                    h2Pos = h2T.position;
                }

                bw.WriteString8(tp.hatchBaseSpecialObject);
                bw.WriteString8(h1SpecObj);
                bw.WriteString8(h2SpecObj);

                bw.Write(tp.unknown4);
                bw.Write(tp.unknown5);

                bw.Write(tp.unknown6);
                bw.Write(tp.unknown7);
                bw.Write(flap1Y);
                bw.Write(flap2Y);
                bw.Write(tp.unknown10);
                bw.Write(tp.unknown11);

                bw.Write(h1Ang);
                bw.Write(h2Ang);
                bw.Write(tp.unknown14);

                bw.Write(h1Pos);
                bw.Write(h2Pos);
            }
        }
    }
}
#endif