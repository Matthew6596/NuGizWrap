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

    public class TubeConfig : GizmoTypeConfig
    {
        public override string ID => "Tube";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 2, TTGame.LIJ1 => 3, TTGame.LB1 => 5, _ => 1 };

        public int version = 2;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int tubeCount = br.ReadInt32();

            string[] existingNames = new string[tubeCount];
            for (int i = 0; i < tubeCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject tubeObj = new(name);
                tubeObj.transform.SetParent(parent);
                tubeObj.transform.position = br.ReadVector3();
                var tube = tubeObj.AddComponent<Tube>();

                tube.height = br.ReadSingle();
                tube.radius = br.ReadSingle();
                if (version >= 2) tube.magnetic = br.ReadByte() != 0;
                if (version >= 3) tube.specialObject = new() { specialObject = br.ReadString8() };
                if (version >= 4) tube.glideOnly = br.ReadByte() != 0;
                if (version >= 5)
                {
                    tube.horizontal = br.ReadByte() != 0;
                    tube.transform.eulerAngles = new(0, br.ReadSingle(), 0);
                }
            }

            return tubeCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var tubes = FindObjectsByType<Tube>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int tubeCount = tubes.Length;
            bw.Write(tubeCount);

            for (int i = 0; i < tubeCount; i++)
            {
                var tube = tubes[i];
                bw.WriteString(tube.name, 16);
                bw.Write(tube.transform.position);
                bw.Write(tube.height);
                bw.Write(tube.radius);
                if (version >= 2) bw.Write((byte)(tube.magnetic ? 1 : 0));
                if (version >= 3) bw.WriteString8(tube.specialObject.specialObject);
                if (version >= 4) bw.Write((byte)(tube.glideOnly ? 1 : 0));
                if (version >= 5)
                {
                    bw.Write((byte)(tube.horizontal ? 1 : 0));
                    float ang = Mathf.Repeat(tube.transform.eulerAngles.y, 360f);
                    bw.Write(ang);
                }
            }
        }
    }
}
#endif