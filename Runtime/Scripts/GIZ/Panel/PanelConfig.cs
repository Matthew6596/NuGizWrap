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

    public class PanelConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.TCS, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 8, TTGame.LB1 => 8, _ => 1 };

        public int version = 8;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int panelCount = br.ReadInt32();

            string[] existingNames = new string[panelCount];
            for (int i = 0; i < panelCount; i++)
            {
                string name = br.ReadString32();

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject panelObj = new(name);
                panelObj.transform.SetParent(parent);
                panelObj.transform.SetPositionAndRotation(br.ReadVector3(), Quaternion.Euler(br.ReadYEuler()));
                var panel = panelObj.AddComponent<Panel>();

                panel.type = (Panel.Type)br.ReadByte();
                if (version >= 3) panel.invisible = br.ReadByte() != 0;

                if (version >= 4)
                {
                    Transform target = new GameObject("target_transform").transform;
                    target.SetParent(panelObj.transform);
                    target.localPosition = br.ReadVector3();
                    target.localScale = br.ReadSingle() * Vector3.one;
                    panel.target = target;
                }

                if (version >= 5) panel.targetInvisible = br.ReadByte() != 0;

                if (version >= 6)
                {
                    panel.alternativeFace = br.ReadByte() != 0;
                    panel.alternativeBody = br.ReadByte() != 0;
                }

                if (version >= 7) panel.unknown1 = br.ReadByte() != 0;
                if (version >= 8) panel.unknown2 = br.ReadByte() != 0;
            }

            return panelCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var panels = FindObjectsByType<Panel>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int panelCount = panels.Length;

            bw.Write(panelCount);

            for (int i = 0; i < panelCount; i++)
            {
                var panel = panels[i];

                bw.WriteString32(panel.name);
                bw.Write(panel.transform.position);
                bw.Write(panel.transform.eulerAngles.y.ToShortAng());

                bw.Write((byte)panel.type);
                if (version >= 3) bw.Write((byte)(panel.invisible ? 1 : 0));

                if (version >= 4)
                {
                    if (panel.target == null)
                    {
                        bw.Write(Vector3.zero);
                        bw.Write(1f);
                    }
                    else
                    {
                        bw.Write(panel.target.position - panel.transform.position);
                        bw.Write(panel.target.localScale.x);
                    }
                }

                if (version >= 5) bw.Write((byte)(panel.targetInvisible ? 1 : 0));
                if (version >= 6)
                {
                    bw.Write((byte)(panel.alternativeFace ? 1 : 0));
                    bw.Write((byte)(panel.alternativeBody ? 1 : 0));
                }

                if (version >= 7) bw.Write((byte)(panel.unknown1 ? 1 : 0));
                if (version >= 8) bw.Write((byte)(panel.unknown2 ? 1 : 0));

            }
        }
    }
}
#endif