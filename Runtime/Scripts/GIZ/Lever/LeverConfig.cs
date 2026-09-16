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

    public class LeverConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 6, TTGame.LIJ1 => 8, TTGame.LB1 => 9, _ => 1 };

        public int version = 6;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int leverCount = br.ReadInt32();

            string[] existingNames = new string[leverCount];
            for (int i = 0; i < leverCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject leverObj = new(name);
                leverObj.transform.SetParent(parent);
                leverObj.transform.SetPositionAndRotation(br.ReadVector3(), Quaternion.Euler(br.ReadYEuler()));
                var lever = leverObj.AddComponent<Lever>();

                lever.handleColor = (Lever.HandleColor)br.ReadByte();
                if (version >= 2) lever.multiplePulls = br.ReadByte() != 0;
                if (version >= 3) lever.pullTime = br.ReadSingle();
                if (version >= 4) lever.invisible = br.ReadByte() != 0;

                if (version >= 5)
                {
                    GameObject target = new("target_transform");
                    target.transform.SetParent(leverObj.transform);
                    lever.target = target.transform;
                    target.transform.localPosition = br.ReadVector3();
                    target.transform.localScale = Vector3.one * br.ReadSingle();
                }

                if (version >= 6) lever.targetInvisible = br.ReadByte() != 0;

                if (version >= 7) lever.unknown1 = br.ReadString8();
                if (version >= 8) lever.unknown2 = br.ReadByte() != 0;
                if (version >= 9)
                {
                    lever.unknown3 = br.ReadByte();
                    lever.unknown4 = br.ReadByte();
                }
            }

            return leverCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var levers = FindObjectsByType<Lever>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            bw.Write(levers.Length);

            for (int i = 0; i < levers.Length; i++)
            {
                var lever = levers[i];

                bw.WriteString(lever.name, 16);
                bw.Write(lever.transform.position);
                bw.Write(lever.transform.eulerAngles.y.ToShortAng());

                bw.Write((byte)lever.handleColor);
                if (version >= 2) bw.Write((byte)(lever.multiplePulls ? 1 : 0));
                if (version >= 3) bw.Write(lever.pullTime);
                if (version >= 4) bw.Write((byte)(lever.invisible ? 1 : 0));

                if (version >= 5)
                {
                    if (lever.target == null)
                    {
                        //ADD DEFAULTS
                        //bytes.AddVector3(new(0, 0, 0.5f));
                        bw.Write(Vector3.zero);
                        bw.Write(1f);
                    }
                    else
                    {
                        bw.Write(lever.target.localPosition);
                        bw.Write(lever.target.localScale.x);
                    }
                }

                if (version >= 6) bw.Write((byte)(lever.targetInvisible ? 1 : 0));

                if (version >= 7) bw.WriteString8(lever.unknown1);
                if (version >= 8) bw.Write((byte)(lever.unknown2 ? 1 : 0));
                if (version >= 9)
                {
                    bw.Write(lever.unknown3);
                    bw.Write(lever.unknown4);
                }
            }
        }
    }
}
#endif