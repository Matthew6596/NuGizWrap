//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.7vjr6qn909rn
//-Matton
//===== ===== ===== ===== =====

#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class GizmoPickupSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 7, TTGame.LIJ1=>7, TTGame.LB1=>7, _ => 1 };

        public override string ID => "GizmoPickup";

        public static GizmoPickupSection Instance { get; private set; }

        public int version = 5;
        public int unknown1 = 1;
        public float drawDistance = 15;
        public float scale = 1;

        private float prevScale = 1;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);

            if (scale != prevScale)
            {
                foreach (var pup in FindObjectsByType<GizmoPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    pup.transform.localScale = new(scale, scale, scale);

                prevScale = scale;
            }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var pickups = FindObjectsByType<GizmoPickup>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            bytes.AddInt(pickups.Length);

            if(version >= 3) bytes.AddInt(unknown1);
            if (version >= 5)
            {
                bytes.AddFloat(drawDistance);
                bytes.AddFloat(scale);
            }

            foreach (var pup in pickups)
            {
                bytes.AddFixedString(pup.name, 8);
                bytes.AddVector3(pup.transform.position);
                bytes.Add((byte)pup.type);

                if (version >= 2) bytes.Add((byte)pup.spawnType);
                if (version >= 4) bytes.Add(pup.spawnGroup);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int pupCount = bytes.ReadInt(ref index);

            if (version >= 3) unknown1 = bytes.ReadInt(ref index);
            if (version >= 5)
            {
                drawDistance = bytes.ReadFloat(ref index);
                scale = bytes.ReadFloat(ref index);
            }

            //Clear existing pickups in scene before loading new ones
            foreach (var pup in FindObjectsByType<GizmoPickup>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) pup.gameObject.DelayDestroy();

            //Load new pickups
            for (int i=0; i<pupCount; i++)
            {
                string pupName = bytes.ReadString(ref index, 8);
                if (pupName.Replace("\0", "").Trim() == "") pupName = "unnamed_pickup";
                GameObject pupObj = new(pupName);
                pupObj.transform.SetParent(transform);
                pupObj.transform.position = bytes.ReadVector3(ref index);
                var pup = pupObj.AddComponent<GizmoPickup>();

                byte type = bytes.ReadByte(ref index);
                if (!Enum.IsDefined(typeof(GizmoPickup.Type), (int)type)) Debug.Log("Loading Unknown Pickup Type!: " + (char)type);
                pup.type = (GizmoPickup.Type)type;

                if (version >= 2) pup.spawnType = (GizmoPickup.SpawnType)bytes.ReadByte(ref index);
                if (version >= 4) pup.spawnGroup = bytes.ReadByte(ref index);
            }
        }
    }
}
#endif