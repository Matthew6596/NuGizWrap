//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.r8uqyeu23ff
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class TightRopeSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LB1 => 4, _ => 1 };

        public override string ID => "TightRope";
        public static TightRopeSection Instance { get; private set; }

        public int version = 4;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var ropes = FindObjectsByType<TightRope>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int ropeCount = ropes.Length;
            bytes.AddInt(ropeCount);

            for(int i=0; i<ropeCount; i++)
            {
                var rope = ropes[i];
                bytes.AddFixedString(rope.name, 16);

                //Preparing/getting knob variables
                Vector3 startKnobPos = Vector3.zero, endKnobPos = Vector3.zero;
                float startXAng = 0, startYAng = 0, endXAng = 0, endYAng = 0;
                bool startPinSide = false, endPinSide = false;
                if (rope.startKnob != null)
                {
                    Transform startTransform = rope.startKnob.transform;
                    startKnobPos = startTransform.position;
                    startXAng = startTransform.eulerAngles.x;
                    startYAng = startTransform.eulerAngles.y;
                    startPinSide = rope.startKnob.pinFacingSideways;
                }
                if (rope.endKnob != null)
                {
                    Transform endTransform = rope.startKnob.transform;
                    endKnobPos = endTransform.position;
                    endXAng = endTransform.eulerAngles.x;
                    endYAng = endTransform.eulerAngles.y;
                    endPinSide = rope.endKnob.pinFacingSideways;
                }

                bytes.AddVector3(startKnobPos);
                bytes.AddVector3(endKnobPos);
                if (version >= 4)
                {
                    bytes.AddVector3(rope.unknown3);
                    bytes.AddVector3(rope.unknown4);
                }

                if (version >= 2)
                {
                    bytes.AddShort((short)startXAng.ToShortAng());
                    bytes.AddShort((short)startYAng.ToShortAng());
                    bytes.AddShort((short)endXAng.ToShortAng());
                    bytes.AddShort((short)endYAng.ToShortAng());

                    bytes.Add((byte)(startPinSide ? 1 : 0));
                    bytes.Add((byte)(endPinSide ? 1 : 0));
                }

                if (version >= 3) bytes.Add((byte)(rope.alwaysShowStartKnob ? 1 : 0));

            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int ropeCount = bytes.ReadInt(ref index);

            //Clear existing tightropes before creating new ones
            foreach (var ropes in FindObjectsByType<TightRope>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) ropes.gameObject.DelayDestroy();

            for(int i=0; i<ropeCount; i++)
            {
                GameObject ropeObj = new(bytes.ReadString(ref index, 16));
                ropeObj.transform.SetParent(transform);
                var rope = ropeObj.AddComponent<TightRope>();

                rope.startKnob = new GameObject("start_knob").AddComponent<TightRopeKnob>();
                rope.startKnob.transform.SetParent(ropeObj.transform);
                rope.endKnob = new GameObject("end_knob").AddComponent<TightRopeKnob>();
                rope.endKnob.transform.SetParent(ropeObj.transform);

                Transform startKnobTransform = rope.startKnob.transform, endKnobTransform = rope.endKnob.transform;
                startKnobTransform.position = bytes.ReadVector3(ref index);
                endKnobTransform.position = bytes.ReadVector3(ref index);
                if (version >= 4)
                {
                    rope.unknown3 = bytes.ReadVector3(ref index);
                    rope.unknown4 = bytes.ReadVector3(ref index);
                }

                if (version >= 2)
                {
                    
                    startKnobTransform.eulerAngles = bytes.ReadXYEuler(ref index);
                    endKnobTransform.eulerAngles = bytes.ReadXYEuler(ref index);

                    rope.startKnob.pinFacingSideways = bytes.ReadByte(ref index) != 0;
                    rope.endKnob.pinFacingSideways = bytes.ReadByte(ref index) != 0;
                }

                if (version >= 3) rope.alwaysShowStartKnob = bytes.ReadByte(ref index) != 0;
            }
        }
    }
}
#endif