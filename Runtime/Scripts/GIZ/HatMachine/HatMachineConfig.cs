#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;

    public class HatMachineConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.TCS);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 5, _ => 1 };

        public int version = 5;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int hatmachineCount = br.ReadInt32();

            string[] existingNames = new string[hatmachineCount];
            for (int i = 0; i < hatmachineCount; i++)
            {
                string name = br.ReadString32();

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject hatObj = new(name);
                hatObj.transform.SetParent(parent);
                hatObj.transform.SetPositionAndRotation(br.ReadVector3(), Quaternion.Euler(br.ReadYEuler()));
                var hatmachine = hatObj.AddComponent<HatMachine>();

                hatmachine.type = (HatMachine.Type)br.ReadByte();
                if (version >= 3) hatmachine.handleColor = (Lever.HandleColor)br.ReadByte();

                if (version >= 4)
                {
                    Transform target = new GameObject("target_transform").transform;
                    target.SetParent(hatObj.transform);
                    target.localPosition = br.ReadVector3();
                    target.localScale = br.ReadSingle() * Vector3.one;
                    hatmachine.target = target;
                }

                if (version >= 5) hatmachine.targetInvisible = br.ReadByte() != 0;
            }

            return hatmachineCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var hatmachines = FindObjectsByType<HatMachine>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int hatmachineCount = hatmachines.Length;

            bw.Write(hatmachineCount);

            for (int i = 0; i < hatmachineCount; i++)
            {
                var hatmachine = hatmachines[i];

                bw.WriteString32(hatmachine.name);
                bw.Write(hatmachine.transform.position);
                bw.Write(hatmachine.transform.eulerAngles.y.ToShortAng());
                bw.Write((byte)hatmachine.type);
                if (version >= 3) bw.Write((byte)hatmachine.handleColor);

                if (version >= 4)
                {
                    if (hatmachine.target == null)
                    {
                        bw.Write(Vector3.zero);
                        bw.Write(1f);
                    }
                    else
                    {
                        bw.Write(hatmachine.target.position - hatmachine.transform.position);
                        bw.Write(hatmachine.target.localScale.x);
                    }
                }

                if (version >= 5) bw.Write((byte)(hatmachine.targetInvisible ? 1 : 0));
            }
        }
    }
}
#endif