//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.bvrr0sa8nkyk
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace NuGizWrap.Gizmos
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
                bytes.AddVector3(zip.hook == null ? Vector3.zero : zip.hook.position);
                bytes.AddVector3(zip.end == null ? (zip.hook != null ? zip.hook.position : Vector3.zero) : zip.end.position);

                Vector3 hookEuler = zip.hook.eulerAngles;
                bytes.AddShort((short)hookEuler.x.ToShortAng());
                bytes.AddShort((short)hookEuler.y.ToShortAng());

                bytes.Add((byte)(zip.swing ? 1 : 0));
                bytes.Add((byte)(zip.activeForPlayer ? 1 : 0));
                bytes.Add((byte)(zip.twoWay ? 1 : 0));
                if (version >= 2) bytes.Add((byte)(zip.hookVisible ? 1 : 0));
                if (version >= 3) bytes.Add((byte)(zip.inactive ? 1 : 0));
                if (version >= 4) bytes.Add((byte)(zip.targetsVisible ? 1 : 0));
                if (version >= 5) bytes.Add((byte)(zip.unknown5 ? 1 : 0));
                if (version >= 6)
                {
                    bytes.Add((byte)zip.startPlatformStyle);
                    bytes.Add((byte)zip.endPlatformStyle);
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
                zip.hook = axis;

                var end = new GameObject("end_transform").transform;
                end.position = bytes.ReadVector3(ref index);
                zip.end = end;

                zipObjTrans.position = (start.position + axis.position + end.position)/3;
                start.SetParent(zipObjTrans);
                axis.SetParent(zipObjTrans);
                end.SetParent(zipObjTrans);

                zip.hook.eulerAngles = bytes.ReadXYEuler(ref index);

                zip.swing = bytes.ReadByte(ref index) != 0;
                zip.activeForPlayer = bytes.ReadByte(ref index) != 0;
                zip.twoWay = bytes.ReadByte(ref index) != 0;
                if (version >= 2) zip.hookVisible = bytes.ReadByte(ref index) != 0;
                if (version >= 3) zip.inactive = bytes.ReadByte(ref index) != 0;
                if (version >= 4) zip.targetsVisible = bytes.ReadByte(ref index) != 0;
                if (version >= 5) zip.unknown5 = bytes.ReadByte(ref index) != 0;
                if (version >= 6)
                {
                    zip.startPlatformStyle = (ZipUp.PlatformStyle)bytes.ReadByte(ref index);
                    zip.endPlatformStyle = (ZipUp.PlatformStyle)bytes.ReadByte(ref index);
                }
            }
        }
    }
}
#endif