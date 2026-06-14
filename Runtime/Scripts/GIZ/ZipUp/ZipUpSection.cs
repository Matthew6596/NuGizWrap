//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.bvrr0sa8nkyk
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class ZipUpSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 4, TTGame.LIJ1 => 6, TTGame.LB1 => 6, _ => 1 };

        public override string ID => "ZipUp";

        public static ZipUpSection Instance { get; private set; }

        public int version = 4;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var zips = FindObjectsByType<ZipUp>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int zipCount = zips.Length;

            bytes.AddInt(zipCount);

            for(int i=0; i<zipCount; i++)
            {
                var zip = zips[i];

                bytes.AddFixedString(zip.name, 16);
                bytes.AddVector3(zip.start == null ? Vector3.zero : zip.start.position);
                bytes.AddVector3(zip.axis == null ? Vector3.zero : zip.axis.position);
                bytes.AddVector3(zip.end == null ? (zip.axis != null ? zip.axis.position : Vector3.zero) : zip.end.position);

                bytes.AddShort(zip.unknown1);
                bytes.AddShort(zip.unknown2);

                bytes.Add((byte)(zip.swing ? 1 : 0));
                bytes.Add((byte)(zip.unknown3 ? 1 : 0));
                bytes.Add((byte)(zip.twoWay ? 1 : 0));
                if (version >= 2) bytes.Add((byte)(zip.invisible ? 1 : 0));
                if (version >= 3) bytes.Add((byte)(zip.unknown4 ? 1 : 0));
                if (version >= 4) bytes.Add((byte)(zip.targetsInvisible ? 1 : 0));
                if (version >= 5) bytes.Add((byte)(zip.unknown5 ? 1 : 0));
                if (version >= 6)
                {
                    bytes.Add(zip.unknown6);
                    bytes.Add(zip.unknown7);
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int zipCount = bytes.ReadInt(ref index);

            //Clear existing zipups before creating new ones
            foreach (var zip in FindObjectsByType<ZipUp>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) zip.gameObject.DelayDestroy();

            for(int i=0; i<zipCount; i++)
            {
                GameObject zipObj = new(bytes.ReadString(ref index, 16));
                Transform zipObjTrans = zipObj.transform;
                zipObjTrans.SetParent(transform);
                var zip = zipObj.AddComponent<ZipUp>();

                var start = new GameObject("start_transform").transform;
                start.position = bytes.ReadVector3(ref index);
                zip.start = start;

                var axis = new GameObject("axis_transform").transform;
                axis.position = bytes.ReadVector3(ref index);
                zip.axis = axis;

                var end = new GameObject("end_transform").transform;
                end.position = bytes.ReadVector3(ref index);
                zip.end = end;

                zipObjTrans.position = (start.position + axis.position + end.position)/3;
                start.SetParent(zipObjTrans);
                axis.SetParent(zipObjTrans);
                end.SetParent(zipObjTrans);

                zip.unknown1 = bytes.ReadShort(ref index);
                zip.unknown2 = bytes.ReadShort(ref index);

                zip.swing = bytes.ReadByte(ref index) != 0;
                zip.unknown3 = bytes.ReadByte(ref index) != 0;
                zip.twoWay = bytes.ReadByte(ref index) != 0;
                if (version >= 2) zip.invisible = bytes.ReadByte(ref index) != 0;
                if (version >= 3) zip.unknown4 = bytes.ReadByte(ref index) != 0;
                if (version >= 4) zip.targetsInvisible = bytes.ReadByte(ref index) != 0;
                if (version >= 5) zip.unknown5 = bytes.ReadByte(ref index) != 0;
                if (version >= 6)
                {
                    zip.unknown6 = bytes.ReadByte(ref index);
                    zip.unknown7 = bytes.ReadByte(ref index);
                }
            }
        }
    }
}
#endif