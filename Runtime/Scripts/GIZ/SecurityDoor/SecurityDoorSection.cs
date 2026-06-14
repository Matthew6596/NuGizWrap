//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.aay4i14akilr
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class SecurityDoorSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1,TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 4, TTGame.LB1 => 4, _ => 1 };

        public override string ID => "SecurityDoor";
        public static SecurityDoorSection Instance { get; private set; }

        public int version = 4;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var doors = FindObjectsByType<SecurityDoor>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int doorCount = doors.Length;
            bytes.AddInt(doorCount);

            for(int i=0; i<doorCount; i++)
            {
                var door = doors[i];
                bytes.AddFixedString(door.name, 16);
                bytes.AddVector3(door.transform.position);
                bytes.AddShort((short)door.transform.eulerAngles.y.ToShortAng());
                if (version >= 2) bytes.AddString8(door.type);
                if (version >= 3) bytes.AddString8(door.unknown1);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int doorCount = bytes.ReadInt(ref index);

            //Clear existing shards before creating new ones
            foreach (var door in FindObjectsByType<SecurityDoor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) 
                door.gameObject.DelayDestroy();

            for(int i=0; i<doorCount; i++)
            {
                GameObject doorObj = new(bytes.ReadString(ref index, 16));
                doorObj.transform.SetParent(transform);
                doorObj.transform.position = bytes.ReadVector3(ref index);
                doorObj.transform.eulerAngles = new(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0);
                var door = doorObj.AddComponent<SecurityDoor>();

                if (version >= 2) door.type = bytes.ReadString8(ref index);
                if (version >= 3) door.unknown1 = bytes.ReadString8(ref index);
            }
        }
    }
}
#endif