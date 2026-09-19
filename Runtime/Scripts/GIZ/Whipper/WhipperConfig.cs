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

    public class WhipperConfig : GizmoTypeConfig
    {
        public override string ID => "Whipper";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 4, _ => 1 };

        public int version = 4;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int whipperCount = br.ReadInt32();

            string[] existingNames = new string[whipperCount];
            for (int i = 0; i < whipperCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject whipperObj = new(name);
                whipperObj.transform.SetParent(parent);
                whipperObj.transform.position = br.ReadVector3();
                var whipper = whipperObj.AddComponent<Whipper>();

                whipper.platformDistance = br.ReadSingle();
                whipper.transform.eulerAngles = br.ReadYEuler();

                whipper.vertical = br.ReadByte() != 0;

                //original prop is no beams, so reverse it here
                if (version >= 2) whipper.hasSupportBeams = br.ReadByte() == 0;

                if (version >= 3) whipper.unknown4 = br.ReadSingle();
                if (version >= 4) whipper.gizObstacle = new() { gizObstacle = br.ReadString(16) };
            }

            return whipperCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var whippers = FindObjectsByType<Whipper>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int whipperCount = whippers.Length;
            bw.Write(whipperCount);

            for (int i = 0; i < whipperCount; i++)
            {
                var whipper = whippers[i];
                bw.WriteString(whipper.name, 16);
                bw.Write(whipper.transform.position);

                bw.Write(whipper.vertical ? whipper.platformDistance : 0.2f);
                bw.Write(whipper.transform.eulerAngles.y.ToShortAng());

                bw.Write((byte)(whipper.vertical ? 1 : 0));

                //original prop is no beams, so reverse it here
                if (version >= 2) bw.Write((byte)(whipper.hasSupportBeams ? 0 : 1));

                if (version >= 3) bw.Write(whipper.unknown4);
                if (version >= 4) bw.WriteString(whipper.gizObstacle.gizObstacle, 16);
            }
        }
    }
}
#endif