//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.9ppjxoll1xur
//-Matton
//===== ===== ===== ===== =====

#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NuGizWrap.Gizmos
{
    using Helper;

    public class TubeSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 2, TTGame.LIJ1 => 3, TTGame.LB1 => 5, _ => 1 };

        public override string ID => "Tube";

        public static TubeSection Instance { get; private set; }

        public int version = 2;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var tubes = FindObjectsByType<Tube>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int tubeCount = tubes.Length;
            bytes.AddInt(tubeCount);

            for(int i=0; i<tubeCount; i++)
            {
                var tube = tubes[i];
                bytes.AddFixedString(tube.name, 16);
                bytes.AddVector3(tube.transform.position);
                bytes.AddFloat(tube.height);
                bytes.AddFloat(tube.radius);
                if (version >= 2) bytes.Add((byte)(tube.magnetic ? 1 : 0));
                if (version >= 3) bytes.AddString8(tube.specialObject.specialObject);
                if (version >= 4) bytes.Add((byte)(tube.glideOnly ? 1 : 0));
                if (version >= 5)
                {
                    bytes.Add((byte)(tube.horizontal ? 1 : 0));
                    float ang = Mathf.Repeat(tube.transform.eulerAngles.y, 360f);
                    bytes.AddFloat(ang);
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int tubeCount = bytes.ReadInt(ref index);

            //Clear tubes before adding new ones
            foreach (var tube in FindObjectsByType<Tube>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) tube.gameObject.DelayDestroy();

            for (int i = 0; i < tubeCount; i++)
            {
                GameObject tubeObj = new(bytes.ReadString(ref index, 16));
                tubeObj.transform.SetParent(transform);
                tubeObj.transform.position = bytes.ReadVector3(ref index);
                var tube = tubeObj.AddComponent<Tube>();

                tube.height = bytes.ReadFloat(ref index);
                tube.radius = bytes.ReadFloat(ref index);
                if (version >= 2) tube.magnetic = bytes.ReadByte(ref index) != 0;
                if (version >= 3) tube.specialObject = new() { specialObject = bytes.ReadString8(ref index) };
                if (version >= 4) tube.glideOnly = bytes.ReadByte(ref index) != 0;
                if (version >= 5)
                {
                    tube.horizontal = bytes.ReadByte(ref index) != 0;
                    tube.transform.eulerAngles = new(0, bytes.ReadFloat(ref index), 0);
                }
            }
        }
    }
}
#endif