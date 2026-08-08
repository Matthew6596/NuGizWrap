//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.o5510grn0x5z
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class PlugSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => TTUnityProject.Prefs.gizmo.allowAllRegisteredGizmos ? game.CompareGames(TTGame.TCS, TTGame.LIJ1, TTGame.LB1) : game.CompareGames(TTGame.LIJ1,TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 5, TTGame.LB1 => 6, _ => 1 };

        public override string ID => "Plug";
        public static PlugSection Instance { get; private set; }

        public int version = 5;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var plugs = FindObjectsByType<Plug>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int plugCount = plugs.Length;
            bytes.AddInt(plugCount);

            for(int i=0; i<plugCount; i++)
            {
                var plug = plugs[i];
                bytes.AddFixedString(plug.name, 16);
                bytes.AddVector3(plug.transform.position);
                Vector3 euler = plug.transform.eulerAngles;
                bytes.AddShort((short)euler.x.ToShortAng());
                bytes.AddShort((short)euler.y.ToShortAng());

                if (version < 3) bytes.Add((byte)plug.unknown1);
                else bytes.AddShort(plug.unknown1);

                if (version >= 5) bytes.AddShort(plug.unknown2);
                if (version >= 2) bytes.AddShort(plug.unknown3);
                if (version >= 4) bytes.Add(plug.unknown4);
                if (version >= 6) bytes.AddFloat(plug.unknown5);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int plugCount = bytes.ReadInt(ref index);

            //Clear existing plugs before creating new ones
            foreach (var plug in FindObjectsByType<Plug>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) plug.gameObject.DelayDestroy();

            for(int i=0; i<plugCount; i++)
            {
                GameObject plugObj = new(bytes.ReadString(ref index, 16));
                plugObj.transform.SetParent(transform);
                plugObj.transform.position = bytes.ReadVector3(ref index);
                float pitch = ((ushort)bytes.ReadShort(ref index)).ToFloatAng();
                float yaw = ((ushort)bytes.ReadShort(ref index)).ToFloatAng();
                plugObj.transform.eulerAngles = new(pitch, yaw, 0);
                var plug = plugObj.AddComponent<Plug>();

                plug.unknown1 = (version < 3) ? bytes.ReadByte(ref index) : bytes.ReadShort(ref index);

                if (version >= 5) plug.unknown2 = bytes.ReadShort(ref index);
                if (version >= 2) plug.unknown3 = bytes.ReadShort(ref index);
                if (version >= 4) plug.unknown4 = bytes.ReadByte(ref index);
                if (version >= 6) plug.unknown5 = bytes.ReadFloat(ref index);
            }
        }
    }
}
#endif