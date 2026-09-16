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

    public class PlugConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => TTUnityProject.Prefs.gizmo.allowAllRegisteredGizmos ? game.CompareGames(TTGame.TCS, TTGame.LIJ1, TTGame.LB1) : game.CompareGames(TTGame.LIJ1, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 2, TTGame.LIJ1 => 5, TTGame.LB1 => 6, _ => 1 };

        public int version = 5;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int plugCount = br.ReadInt32();

            string[] existingNames = new string[plugCount];
            for (int i = 0; i < plugCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject plugObj = new(name);
                plugObj.transform.SetParent(parent);
                plugObj.transform.position = br.ReadVector3();
                float pitch = br.ReadUInt16().ToFloatAng();
                float yaw = br.ReadUInt16().ToFloatAng();
                var plug = plugObj.AddComponent<Plug>();
                plug.validBlowups = (version < 3) ? br.ReadByte() : br.ReadInt16();

                if (version >= 5) plug.unknown2 = br.ReadInt16();

                float zAng = 0;
                if (version >= 2) zAng = br.ReadUInt16().ToFloatAng();
                plugObj.transform.eulerAngles = new(pitch, yaw, zAng);

                if (version >= 4) plug.blowupObjectVisible = br.ReadByte() != 0;
                if (version >= 6) plug.unknown5 = br.ReadSingle();
            }

            return plugCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var plugs = FindObjectsByType<Plug>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int plugCount = plugs.Length;
            bw.Write(plugCount);

            for (int i = 0; i < plugCount; i++)
            {
                var plug = plugs[i];
                bw.WriteString(plug.name, 16);
                bw.Write(plug.transform.position);
                Vector3 euler = plug.transform.eulerAngles;
                bw.Write(euler.x.ToShortAng());
                bw.Write(euler.y.ToShortAng());

                if (version < 3) bw.Write((byte)plug.validBlowups);
                else bw.Write(plug.validBlowups);

                if (version >= 5) bw.Write(plug.unknown2);
                if (version >= 2) bw.Write((short)euler.z.ToShortAng());
                if (version >= 4) bw.Write((byte)(plug.blowupObjectVisible ? 1 : 0));
                if (version >= 6) bw.Write(plug.unknown5);
            }
        }
    }
}
#endif