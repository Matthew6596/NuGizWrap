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

    public class ZipUpConfig : GizmoTypeConfig
    {
        public override string ID => "ZipUp";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 4, TTGame.LIJ1 => 6, TTGame.LB1 => 6, _ => 1 };

        public int version = 4;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int zipCount = br.ReadInt32();

            string[] existingNames = new string[zipCount];
            for (int i = 0; i < zipCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject zipObj = new(name);
                Transform zipObjTrans = zipObj.transform;
                zipObjTrans.SetParent(parent);
                var zip = zipObj.AddComponent<ZipUp>();

                var start = new GameObject("start_transform").transform;
                start.position = br.ReadVector3();
                zip.start = start;

                var axis = new GameObject("axis_transform").transform;
                axis.position = br.ReadVector3();
                zip.hook = axis;

                var end = new GameObject("end_transform").transform;
                end.position = br.ReadVector3();
                zip.end = end;

                zipObjTrans.position = (start.position + axis.position + end.position) / 3;
                start.SetParent(zipObjTrans);
                axis.SetParent(zipObjTrans);
                end.SetParent(zipObjTrans);

                zip.hook.eulerAngles = br.ReadXYEuler();

                zip.swing = br.ReadByte() != 0;
                zip.activeForPlayer = br.ReadByte() != 0;
                zip.twoWay = br.ReadByte() != 0;
                if (version >= 2) zip.hookVisible = br.ReadByte() != 0;
                if (version >= 3) zip.inactive = br.ReadByte() != 0;
                if (version >= 4) zip.targetsVisible = br.ReadByte() != 0;
                if (version >= 5) zip.unknown5 = br.ReadByte() != 0;
                if (version >= 6)
                {
                    zip.startPlatformStyle = (ZipUp.PlatformStyle)br.ReadByte();
                    zip.endPlatformStyle = (ZipUp.PlatformStyle)br.ReadByte();
                }
            }

            return zipCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var zips = FindObjectsByType<ZipUp>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int zipCount = zips.Length;

            bw.Write(zipCount);

            for (int i = 0; i < zipCount; i++)
            {
                var zip = zips[i];

                bw.WriteString(zip.name, 16);
                bw.Write(zip.start == null ? Vector3.zero : zip.start.position);
                bw.Write(zip.hook == null ? Vector3.zero : zip.hook.position);
                bw.Write(zip.end == null ? (zip.hook != null ? zip.hook.position : Vector3.zero) : zip.end.position);

                Vector3 hookEuler = zip.hook.eulerAngles;
                bw.Write(hookEuler.x.ToShortAng());
                bw.Write(hookEuler.y.ToShortAng());

                bw.Write((byte)(zip.swing ? 1 : 0));
                bw.Write((byte)(zip.activeForPlayer ? 1 : 0));
                bw.Write((byte)(zip.twoWay ? 1 : 0));
                if (version >= 2) bw.Write((byte)(zip.hookVisible ? 1 : 0));
                if (version >= 3) bw.Write((byte)(zip.inactive ? 1 : 0));
                if (version >= 4) bw.Write((byte)(zip.targetsVisible ? 1 : 0));
                if (version >= 5) bw.Write((byte)(zip.unknown5 ? 1 : 0));
                if (version >= 6)
                {
                    bw.Write((byte)zip.startPlatformStyle);
                    bw.Write((byte)zip.endPlatformStyle);
                }
            }
        }
    }
}
#endif