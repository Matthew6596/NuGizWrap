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

    public class TightRopeConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LB1 => 4, _ => 1 };

        public int version = 4;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int ropeCount = br.ReadInt32();

            string[] existingNames = new string[ropeCount];
            for (int i = 0; i < ropeCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject ropeObj = new(name);
                ropeObj.transform.SetParent(parent);
                var rope = ropeObj.AddComponent<TightRope>();

                rope.startKnob = new GameObject("start_knob").AddComponent<TightRopeKnob>();
                rope.startKnob.transform.SetParent(ropeObj.transform);
                rope.endKnob = new GameObject("end_knob").AddComponent<TightRopeKnob>();
                rope.endKnob.transform.SetParent(ropeObj.transform);

                Transform startKnobTransform = rope.startKnob.transform, endKnobTransform = rope.endKnob.transform;
                startKnobTransform.position = br.ReadVector3();
                endKnobTransform.position = br.ReadVector3();
                if (version >= 4)
                {
                    rope.unknown3 = br.ReadVector3();
                    rope.unknown4 = br.ReadVector3();
                }
                if (version >= 2)
                {

                    startKnobTransform.eulerAngles = br.ReadXYEuler();
                    endKnobTransform.eulerAngles = br.ReadXYEuler();

                    rope.startKnob.pinFacingSideways = br.ReadByte() != 0;
                    rope.endKnob.pinFacingSideways = br.ReadByte() != 0;
                }

                if (version >= 3) rope.alwaysShowStartKnob = br.ReadByte() != 0;
            }

            return ropeCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var ropes = FindObjectsByType<TightRope>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int ropeCount = ropes.Length;
            bw.Write(ropeCount);

            for (int i = 0; i < ropeCount; i++)
            {
                var rope = ropes[i];
                bw.WriteString(rope.name, 16);

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

                bw.Write(startKnobPos);
                bw.Write(endKnobPos);
                if (version >= 4)
                {
                    bw.Write(rope.unknown3);
                    bw.Write(rope.unknown4);
                }

                if (version >= 2)
                {
                    bw.Write(startXAng.ToShortAng());
                    bw.Write(startYAng.ToShortAng());
                    bw.Write(endXAng.ToShortAng());
                    bw.Write(endYAng.ToShortAng());

                    bw.Write((byte)(startPinSide ? 1 : 0));
                    bw.Write((byte)(endPinSide ? 1 : 0));
                }

                if (version >= 3) bw.Write((byte)(rope.alwaysShowStartKnob ? 1 : 0));

            }
        }
    }
}
#endif