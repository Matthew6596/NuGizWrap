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

    public class GrappleConfig : GizmoTypeConfig
    {
        public override string ID => "Grapple";

        public override bool IsGameCompatible(TTGame game) => TTUnityProject.Prefs.gizmo.allowAllRegisteredGizmos ? game.CompareGames(TTGame.TCS, TTGame.LIJ1, TTGame.LB1) : game.CompareGames(TTGame.LIJ1, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 7, TTGame.LIJ1 => 11, TTGame.LB1 => 11, _ => 1 };

        public int version = 11;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int grappleCount = br.ReadInt32();

            string[] existingNames = new string[grappleCount];
            for (int i = 0; i < grappleCount; i++)
            {
                string name = br.ReadString(16);

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject grappleObj = new(name);
                grappleObj.transform.SetParent(parent);
                grappleObj.transform.position = br.ReadVector3();
                var grapple = grappleObj.AddComponent<Grapple>();
                Vector3 grappleEuler = Vector3.zero;

                if (version < 2) br.ReadInt16(); //padding
                grappleEuler.y = br.ReadUInt16().ToFloatAng();
                if (version >= 3) grapple.unknown3 = br.ReadSingle();
                if (version >= 4)
                {
                    grapple.swingingRope = br.ReadByte() != 0;
                    grapple.length = br.ReadSingle();
                }
                if (version >= 5) grappleEuler.x = br.ReadUInt16().ToFloatAng();
                if (version >= 6) grapple.noFreeMovement = br.ReadByte() != 0;
                if (version >= 7) grapple.specialObject = new() { specialObject = br.ReadString8() };
                if (version >= 8) grapple.visible = br.ReadByte() != 0;
                if (version >= 9)
                {
                    if (grapple.swingingRope) grapple.ropeType = (Grapple.RopeGrappleType)br.ReadByte();
                    else grapple.grappleType = (Grapple.ZipGrappleType)br.ReadByte();
                }
                if (version >= 10) grapple.blowup.SetBlowup(br.ReadString(16));
                if (version >= 11) grapple.ropeBrightness = br.ReadByte() / 255f;

                grapple.transform.eulerAngles = grappleEuler;
            }

            return grappleCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var grapples = FindObjectsByType<Grapple>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int grappleCount = grapples.Length;
            bw.Write(grappleCount);

            for (int i = 0; i < grappleCount; i++)
            {
                var grapple = grapples[i];
                bw.WriteString(grapple.name, 16);
                bw.Write(grapple.transform.position);
                Vector3 grappleEuler = grapple.transform.eulerAngles;

                if (version < 2) bw.Write((short)0); //padding
                bw.Write(grappleEuler.y.ToShortAng());
                if (version >= 3) bw.Write(grapple.unknown3);
                if (version >= 4)
                {
                    bw.Write((byte)(grapple.swingingRope ? 1 : 0));
                    bw.Write(grapple.length);
                }
                if (version >= 5) bw.Write(grappleEuler.x.ToShortAng());
                if (version >= 6) bw.Write((byte)(grapple.noFreeMovement ? 1 : 0));
                if (version >= 7) bw.WriteString8(grapple.specialObject.specialObject);
                if (version >= 8) bw.Write((byte)(grapple.visible ? 1 : 0));
                if (version >= 9) bw.Write(grapple.swingingRope ? (byte)grapple.ropeType : (byte)grapple.grappleType);
                if (version >= 10) bw.WriteString(grapple.blowup.GetBlowup(), 16);
                if (version >= 11) bw.Write((byte)(grapple.ropeBrightness * 255));
            }
        }
    }
}
#endif