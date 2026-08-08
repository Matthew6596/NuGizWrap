//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.khpqnkqqfga2
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class HatMachineSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.TCS);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 5, _ => 1 };

        public override string ID => "HatMachine";

        public static HatMachineSection Instance { get; private set; }

        public int version = 5;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var hatmachines = FindObjectsByType<HatMachine>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int hatmachineCount = hatmachines.Length;

            bytes.AddInt(hatmachineCount);

            for(int i=0; i<hatmachineCount; i++)
            {
                var hatmachine = hatmachines[i];

                bytes.AddString32(hatmachine.name);
                bytes.AddVector3(hatmachine.transform.position);
                bytes.AddShort((short)hatmachine.transform.eulerAngles.y.ToShortAng());
                bytes.Add((byte)hatmachine.type);
                if (version >= 3) bytes.Add((byte)hatmachine.handleColor);

                if (version >= 4)
                {
                    if(hatmachine.target == null)
                    {
                        bytes.AddVector3(Vector3.zero);
                        bytes.AddFloat(1);
                    }
                    else
                    {
                        bytes.AddVector3(hatmachine.target.position - hatmachine.transform.position);
                        bytes.AddFloat(hatmachine.target.localScale.x);
                    }
                }

                if (version >= 5) bytes.Add((byte)(hatmachine.targetInvisible ? 1 : 0));
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int hatmachineCount = bytes.ReadInt(ref index);

            //Clear existing hatmachines before adding new ones
            foreach (var hat in FindObjectsByType<HatMachine>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) hat.gameObject.DelayDestroy();

            for(int i=0; i<hatmachineCount; i++)
            {
                GameObject hatObj = new(bytes.ReadString32(ref index));
                hatObj.transform.SetParent(transform);
                hatObj.transform.SetPositionAndRotation(bytes.ReadVector3(ref index),
                    Quaternion.Euler(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0));
                var hatmachine = hatObj.AddComponent<HatMachine>();

                hatmachine.type = (HatMachine.Type)bytes.ReadByte(ref index);
                if (version >= 3) hatmachine.handleColor = (Lever.HandleColor)bytes.ReadByte(ref index);

                if (version >= 4)
                {
                    Transform target = new GameObject("target_transform").transform;
                    target.SetParent(hatObj.transform);
                    target.localPosition = bytes.ReadVector3(ref index);
                    target.localScale = bytes.ReadFloat(ref index) * Vector3.one;
                    hatmachine.target = target;
                }

                if (version >= 5) hatmachine.targetInvisible = bytes.ReadByte(ref index) != 0;
            }
        }
    }
}
#endif