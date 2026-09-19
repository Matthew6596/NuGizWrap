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

    public class SecurityDoorConfig : GizmoTypeConfig
    {
        public override string ID => "SecurityDoor";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 4, TTGame.LB1 => 4, _ => 1 };

        public int version = 4;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int doorCount = br.ReadInt32();

            string[] existingNames = new string[doorCount];
            for (int i = 0; i < doorCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject doorObj = new(name);
                doorObj.transform.SetParent(parent);
                doorObj.transform.position = br.ReadVector3();
                doorObj.transform.eulerAngles = br.ReadYEuler();
                var door = doorObj.AddComponent<SecurityDoor>();

                if (version >= 2) door.type = br.ReadString8();
                if (version >= 3) door.specialObject = new() { specialObject = br.ReadString8() };
            }

            return doorCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var doors = FindObjectsByType<SecurityDoor>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int doorCount = doors.Length;
            bw.Write(doorCount);

            for (int i = 0; i < doorCount; i++)
            {
                var door = doors[i];
                bw.WriteString(door.name, 16);
                bw.Write(door.transform.position);
                bw.Write(door.transform.eulerAngles.y.ToShortAng());
                if (version >= 2) bw.WriteString8(door.type);
                if (version >= 3) bw.WriteString8(door.specialObject.specialObject);
            }
        }
    }
}
#endif