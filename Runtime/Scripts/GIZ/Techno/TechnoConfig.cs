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

    public class TechnoConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => TTUnityProject.Prefs.gizmo.allowAllRegisteredGizmos ? game.CompareGames(TTGame.TCS, TTGame.LIJ1, TTGame.LB1) : game.CompareGames(TTGame.LIJ1, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 5, TTGame.LIJ1 => 6, TTGame.LB1 => 8, _ => 1 };

        public int version = 6;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int technoCount = br.ReadInt32();

            string[] existingNames = new string[technoCount];
            for (int i = 0; i < technoCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject technoObj = new(name);
                technoObj.transform.SetParent(parent);
                technoObj.transform.position = br.ReadVector3();
                technoObj.transform.eulerAngles = br.ReadYEuler();
                var techno = technoObj.AddComponent<Techno>();

                if (version >= 8) techno.controlType = br.ReadString8();

                if (version >= 2)
                {
                    techno.unknown1 = br.ReadByte();
                    techno.controlledEntity = br.ReadString32();
                }

                if (version >= 3) techno.unknown2 = br.ReadByte();
                if (version >= 4) techno.cameraEmphasisAmount = br.ReadSingle();
                if (version >= 5) techno.unknown3 = br.ReadInt32();
                if (version >= 7) techno.unknown4 = br.ReadSingle();
            }

            return technoCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var technos = FindObjectsByType<Techno>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int technoCount = technos.Length;
            bw.Write(technoCount);

            for (int i = 0; i < technoCount; i++)
            {
                var techno = technos[i];
                bw.WriteString(techno.name, 16);
                bw.Write(techno.transform.position);
                bw.Write(techno.transform.eulerAngles.y.ToShortAng());

                if (version >= 8) bw.WriteString8(techno.controlType);
                if (version >= 2)
                {
                    bw.Write(techno.unknown1);
                    bw.WriteString32(techno.controlledEntity);
                }
                if (version >= 3) bw.Write(techno.unknown2);
                if (version >= 4) bw.Write(techno.cameraEmphasisAmount);
                if (version >= 5) bw.Write(techno.unknown3);
                if (version >= 7) bw.Write(techno.unknown4);
            }
        }
    }
}
#endif