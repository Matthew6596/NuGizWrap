//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.p9ura3i9iwpx
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class LeverSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 6, TTGame.LIJ1 => 8, TTGame.LB1 => 9, _ => 1 };

        public override string ID => "Lever";

        public static LeverSection Instance { get; private set; }

        public int version = 6;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var levers = FindObjectsByType<Lever>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            bytes.AddInt(levers.Length);

            for(int i=0; i<levers.Length; i++)
            {
                var lever = levers[i];

                bytes.AddFixedString(lever.name, 16);
                bytes.AddVector3(lever.transform.position);
                bytes.AddShort((short)lever.transform.eulerAngles.y.ToShortAng());

                bytes.Add((byte)lever.handleColor);
                if (version >= 2) bytes.Add((byte)(lever.multiplePulls ? 1 : 0));
                if (version >= 3) bytes.AddFloat(lever.pullTime);
                if (version >= 4) bytes.Add((byte)(lever.invisible ? 1 : 0));

                if (version >= 5)
                {
                    if (lever.target == null)
                    {
                        //ADD DEFAULTS
                        //bytes.AddVector3(new(0, 0, 0.5f));
                        bytes.AddVector3(Vector3.zero);
                        bytes.AddFloat(1);
                    }
                    else
                    {
                        bytes.AddVector3(lever.target.localPosition);
                        bytes.AddFloat(lever.target.localScale.x);
                    }
                }

                if (version >= 6) bytes.Add((byte)(lever.targetInvisible ? 1 : 0));

                if (version >= 7) bytes.AddString8(lever.unknown1);
                if (version >= 8) bytes.Add((byte)(lever.unknown2 ? 1 : 0));
                if (version >= 9)
                {
                    bytes.Add(lever.unknown3);
                    bytes.Add(lever.unknown4);
                }
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int leverCount = bytes.ReadInt(ref index);

            //Clear existing levers before adding new ones
            foreach (var lever in FindObjectsByType<Lever>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) lever.gameObject.DelayDestroy();

            for(int i=0; i<leverCount; i++)
            {
                GameObject leverObj = new(bytes.ReadString(ref index, 16));
                leverObj.transform.SetParent(transform);
                leverObj.transform.SetPositionAndRotation(bytes.ReadVector3(ref index),
                    Quaternion.Euler(0, ((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0));
                var lever = leverObj.AddComponent<Lever>();

                lever.handleColor = (Lever.HandleColor)bytes.ReadByte(ref index);
                if (version >= 2) lever.multiplePulls = bytes.ReadByte(ref index) != 0;
                if (version >= 3) lever.pullTime = bytes.ReadFloat(ref index);
                if (version >= 4) lever.invisible = bytes.ReadByte(ref index) != 0;
                
                if (version >= 5)
                {
                    GameObject target = new("target_transform");
                    target.transform.SetParent(leverObj.transform);
                    lever.target = target.transform;
                    target.transform.localPosition = bytes.ReadVector3(ref index);
                    target.transform.localScale = Vector3.one * bytes.ReadFloat(ref index);
                }

                if (version >= 6) lever.targetInvisible = bytes.ReadByte(ref index) != 0;

                if (version >= 7) lever.unknown1 = bytes.ReadString8(ref index);
                if (version >= 8) lever.unknown2 = bytes.ReadByte(ref index) != 0;
                if (version >= 9)
                {
                    lever.unknown3 = bytes.ReadByte(ref index);
                    lever.unknown4 = bytes.ReadByte(ref index);
                }
            }
        }
    }
}
#endif