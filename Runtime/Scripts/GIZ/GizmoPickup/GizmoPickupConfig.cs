#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    [CreateAssetMenu(fileName = "GizmoPickupConfig", menuName = "Gizmos/GizmoPickup Config")]
    public class GizmoPickupConfig : GizmoTypeConfig
    {
        public override string ID => "GizmoPickup";

        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 7, TTGame.LIJ1 => 7, TTGame.LB1 => 7, _ => 1 };

        public int version = 5;
        public int unknown1 = 1;
        public float drawDistance = 15;
        public float scale = 1;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int pupCount = br.ReadInt32();

            if (version >= 3) unknown1 = br.ReadInt32();
            if (version >= 5)
            {
                drawDistance = br.ReadSingle();
                scale = br.ReadSingle();
            }

            List<string> existingPupNames = new();
            //Load new pickups
            for (int i = 0; i < pupCount; i++)
            {
                string pupName = br.ReadString(8);
                if (pupName.Trim() == "") pupName = $"pup";
                pupName = ObjectNames.GetUniqueName(existingPupNames.ToArray(), pupName);
                existingPupNames.Add(pupName);

                GameObject pupObj = new(pupName);
                pupObj.transform.SetParent(parent);
                pupObj.transform.position = br.ReadVector3();
                var pup = pupObj.AddComponent<GizmoPickup>();

                byte type = br.ReadByte();
                if (!Enum.IsDefined(typeof(GizmoPickup.Type), (int)type)) Debug.Log("Loading Unknown Pickup Type!: " + (char)type);
                pup.type = (GizmoPickup.Type)type;

                if (version >= 2) pup.spawnType = (GizmoPickup.SpawnType)br.ReadByte();
                if (version >= 4) pup.spawnGroup = br.ReadByte();
            }

            return pupCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var pickups = FindObjectsByType<GizmoPickup>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            bw.Write(pickups.Length);

            if (version >= 3) bw.Write(unknown1);
            if (version >= 5)
            {
                bw.Write(drawDistance);
                bw.Write(scale);
            }

            foreach (var pup in pickups)
            {
                bw.WriteString(pup.name, 8);
                bw.Write(pup.transform.position);
                bw.Write((byte)pup.type);

                if (version >= 2) bw.Write((byte)pup.spawnType);
                if (version >= 4) bw.Write(pup.spawnGroup);
            }
        }
    }
}
#endif