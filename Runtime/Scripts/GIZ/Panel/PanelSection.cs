//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.3sc0ufxr8kc6
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class PanelSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.TCS, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 8, TTGame.LB1=>8, _ => 1 };

        public override string ID => "Panel";

        public static PanelSection Instance { get; private set; }

        public int version = 8;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var panels = FindObjectsByType<Panel>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int panelCount = panels.Length;

            bytes.AddInt(panelCount);

            for(int i=0; i<panelCount; i++)
            {
                var panel = panels[i];

                bytes.AddString32(panel.name);
                bytes.AddVector3(panel.transform.position);
                bytes.AddShort((short)panel.transform.eulerAngles.y.ToShortAng());

                bytes.Add((byte)panel.type);
                if (version >= 3) bytes.Add((byte)(panel.invisible ? 1 : 0));

                if (version >= 4)
                {
                    if (panel.target == null)
                    {
                        bytes.AddVector3(Vector3.zero);
                        bytes.AddFloat(1);
                    }
                    else
                    {
                        bytes.AddVector3(panel.target.position - panel.transform.position);
                        bytes.AddFloat(panel.target.localScale.x);
                    }
                }

                if (version >= 5) bytes.Add((byte)(panel.targetInvisible ? 1 : 0));
                if (version >= 6)
                {
                    bytes.Add((byte)(panel.alternativeFace ? 1 : 0));
                    bytes.Add((byte)(panel.alternativeBody ? 1 : 0));
                }

                if (version >= 7) bytes.Add((byte)(panel.unknown1 ? 1 : 0));
                if (version >= 8) bytes.Add((byte)(panel.unknown2 ? 1 : 0));

            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int panelCount = bytes.ReadInt(ref index);

            //Clear existing panels before adding new ones
            foreach (var panel in FindObjectsByType<Panel>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) panel.gameObject.DelayDestroy();

            for(int i=0; i<panelCount; i++)
            {
                GameObject panelObj = new(bytes.ReadString32(ref index));
                panelObj.transform.SetParent(transform);
                panelObj.transform.SetPositionAndRotation(bytes.ReadVector3(ref index),
                    Quaternion.Euler(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0));
                var panel = panelObj.AddComponent<Panel>();

                panel.type = (Panel.Type)bytes.ReadByte(ref index);
                if (version >= 3) panel.invisible = bytes.ReadByte(ref index) != 0;

                if (version >= 4)
                {
                    Transform target = new GameObject("target_transform").transform;
                    target.SetParent(panelObj.transform);
                    target.localPosition = bytes.ReadVector3(ref index);
                    target.localScale = bytes.ReadFloat(ref index) * Vector3.one;
                    panel.target = target;
                }

                if (version >= 5) panel.targetInvisible = bytes.ReadByte(ref index) != 0;

                if (version >= 6)
                {
                    panel.alternativeFace = bytes.ReadByte(ref index) != 0;
                    panel.alternativeBody = bytes.ReadByte(ref index) != 0;
                }

                if (version >= 7) panel.unknown1 = bytes.ReadByte(ref index) != 0;
                if (version >= 8) panel.unknown2 = bytes.ReadByte(ref index) != 0;
            }
        }
    }
}
#endif