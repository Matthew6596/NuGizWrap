//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.oa61eeebjknu
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class TeleportSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 2, _ => 2 };

        public override string ID => "Teleport";
        public static TeleportSection Instance { get; private set; }

        public int version = 2;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var tps = FindObjectsByType<Teleport>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int tpCount = tps.Length;
            bytes.AddInt(tpCount);

            for(int i=0; i<tpCount; i++)
            {
                var tp = tps[i];
                bytes.AddString8(tp.name);

                var h1 = tp.hatch1;
                var h2 = tp.hatch2;
                var h1T = h1.transform;
                var h2T = h2.transform;

                string h1SpecObj = "Flap_01";
                float flap1Y = 0.25f;
                short h1Ang = 0;
                Vector3 h1Pos = tp.unknown4;
                if(h1 != null)
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

                bytes.AddString8(tp.hatchBaseSpecialObject);
                bytes.AddString8(h1SpecObj);
                bytes.AddString8(h2SpecObj);

                bytes.AddVector3(tp.unknown4);
                bytes.AddVector3(tp.unknown5);

                bytes.AddFloat(tp.unknown6);
                bytes.AddFloat(tp.unknown7);
                bytes.AddFloat(flap1Y);
                bytes.AddFloat(flap2Y);
                bytes.AddFloat(tp.unknown10);
                bytes.AddFloat(tp.unknown11);

                bytes.AddShort(h1Ang);
                bytes.AddShort(h2Ang);
                bytes.AddShort(tp.unknown14);

                bytes.AddVector3(h1Pos);
                bytes.AddVector3(h2Pos);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            //version = bytes.ReadInt(ref index);
            version = 2; //hardcoded in LIJ1
            bytes.ReadInt(ref index);

            int tpCount = bytes.ReadInt(ref index);

            //Clear existing shards before creating new ones
            foreach (var tp in FindObjectsByType<Teleport>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) tp.gameObject.DelayDestroy();

            for(int i=0; i<tpCount; i++)
            {
                GameObject tpObj = new(bytes.ReadString8(ref index));
                tpObj.transform.SetParent(transform);
                var tp = tpObj.AddComponent<Teleport>();

                var h1 = new GameObject("hatch_1").AddComponent<TeleportHatch>();
                h1.transform.SetParent(tp.transform);
                tp.hatch1 = h1;

                var h2 = new GameObject("hatch_2").AddComponent<TeleportHatch>();
                h2.transform.SetParent(tp.transform);
                tp.hatch2 = h2;

                tp.hatchBaseSpecialObject = bytes.ReadString8(ref index);
                h1.flapSpecialObject = bytes.ReadString8(ref index);
                h2.flapSpecialObject = bytes.ReadString8(ref index);

                tp.unknown4 = bytes.ReadVector3(ref index);
                tp.unknown5 = bytes.ReadVector3(ref index);

                tp.unknown6 = bytes.ReadFloat(ref index);
                tp.unknown7 = bytes.ReadFloat(ref index);
                h1.flapYOffset = bytes.ReadFloat(ref index);
                h2.flapYOffset = bytes.ReadFloat(ref index);
                tp.unknown10 = bytes.ReadFloat(ref index);
                tp.unknown11 = bytes.ReadFloat(ref index);

                h1.transform.eulerAngles = bytes.ReadYEuler(ref index);
                h2.transform.eulerAngles = bytes.ReadYEuler(ref index);
                tp.unknown14 = bytes.ReadShort(ref index);

                h1.transform.position = bytes.ReadVector3(ref index);
                h2.transform.position = bytes.ReadVector3(ref index);
            }
        }
    }
}
#endif