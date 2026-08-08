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
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 2, _ => 1 };

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

                bytes.AddString8(tp.unknown1);
                bytes.AddString8(tp.unknown2);
                bytes.AddString8(tp.unknown3);

                bytes.AddVector3(tp.unknown4);
                bytes.AddVector3(tp.unknown5);

                bytes.AddFloat(tp.unknown6);
                bytes.AddFloat(tp.unknown7);
                bytes.AddFloat(tp.unknown8);
                bytes.AddFloat(tp.unknown9);
                bytes.AddFloat(tp.unknown10);
                bytes.AddFloat(tp.unknown11);

                bytes.AddShort(tp.unknown12);
                bytes.AddShort(tp.unknown13);
                bytes.AddShort(tp.unknown14);

                bytes.AddVector3(tp.unknown15);
                bytes.AddVector3(tp.unknown16);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int tpCount = bytes.ReadInt(ref index);

            //Clear existing shards before creating new ones
            foreach (var tp in FindObjectsByType<Teleport>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) tp.gameObject.DelayDestroy();

            for(int i=0; i<tpCount; i++)
            {
                GameObject tpObj = new(bytes.ReadString8(ref index));
                tpObj.transform.SetParent(transform);
                var tp = tpObj.AddComponent<Teleport>();

                tp.unknown1 = bytes.ReadString8(ref index);
                tp.unknown2 = bytes.ReadString8(ref index);
                tp.unknown3 = bytes.ReadString8(ref index);

                tp.unknown4 = bytes.ReadVector3(ref index);
                tp.unknown5 = bytes.ReadVector3(ref index);

                tp.unknown6 = bytes.ReadFloat(ref index);
                tp.unknown7 = bytes.ReadFloat(ref index);
                tp.unknown8 = bytes.ReadFloat(ref index);
                tp.unknown9 = bytes.ReadFloat(ref index);
                tp.unknown10 = bytes.ReadFloat(ref index);
                tp.unknown11 = bytes.ReadFloat(ref index);

                tp.unknown12 = bytes.ReadShort(ref index);
                tp.unknown13 = bytes.ReadShort(ref index);
                tp.unknown14 = bytes.ReadShort(ref index);

                tp.unknown15 = bytes.ReadVector3(ref index);
                tp.unknown16 = bytes.ReadVector3(ref index);
            }
        }
    }
}
#endif