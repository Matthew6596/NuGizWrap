//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.46t16ppabddc
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class GrappleSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => TTUnityProject.Prefs.gizmo.allowAllRegisteredGizmos ? game.CompareGames(TTGame.TCS, TTGame.LIJ1, TTGame.LB1) : game.CompareGames(TTGame.LIJ1,TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 11, TTGame.LB1 => 11, _ => 1 };

        public override string ID => "Grapple";
        public static GrappleSection Instance { get; private set; }

        public int version = 11;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var grapples = FindObjectsByType<Grapple>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int grappleCount = grapples.Length;
            bytes.AddInt(grappleCount);

            for(int i=0; i<grappleCount; i++)
            {
                var grapple = grapples[i];
                bytes.AddFixedString(grapple.name, 16);
                bytes.AddVector3(grapple.transform.position);
                Vector3 grappleEuler = grapple.transform.eulerAngles;

                if (version < 2) bytes.AddShort(0); //padding
                bytes.AddShort((short)grappleEuler.y.ToShortAng());
                if (version >= 3) bytes.AddFloat(grapple.unknown3);
                if (version >= 4)
                {
                    bytes.Add((byte)(grapple.swingingRope ? 1 : 0));
                    bytes.AddFloat(grapple.length);
                }
                if (version >= 5) bytes.AddShort((short)grappleEuler.x.ToShortAng());
                if (version >= 6) bytes.Add((byte)(grapple.noFreeMovement ? 1 : 0));
                if (version >= 7) bytes.AddString8(grapple.specialObject.specialObject);
                if (version >= 8) bytes.Add((byte)(grapple.visible ? 1 : 0));
                if (version >= 9) bytes.Add(grapple.swingingRope ? (byte)grapple.ropeType : (byte)grapple.grappleType);
                if (version >= 10) bytes.AddFixedString(grapple.blowup.GetBlowup(), 16);
                if (version >= 11) bytes.Add((byte)(grapple.ropeBrightness * 255));
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int grappleCount = bytes.ReadInt(ref index);

            //Clear existing shards before creating new ones
            foreach (var grapple in FindObjectsByType<Grapple>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) 
                grapple.gameObject.DelayDestroy();

            for(int i=0; i<grappleCount; i++)
            {
                GameObject grappleObj = new(bytes.ReadString(ref index, 16));
                grappleObj.transform.SetParent(transform);
                grappleObj.transform.position = bytes.ReadVector3(ref index);
                var grapple = grappleObj.AddComponent<Grapple>();
                Vector3 grappleEuler = Vector3.zero;

                if (version < 2) bytes.ReadShort(ref index); //padding
                grappleEuler.y = bytes.ReadShort(ref index);
                if (version >= 3) grapple.unknown3 = bytes.ReadFloat(ref index);
                if (version >= 4)
                {
                    grapple.swingingRope = bytes.ReadByte(ref index) != 0;
                    grapple.length = bytes.ReadFloat(ref index);
                }
                if (version >= 5) grappleEuler.x = bytes.ReadShort(ref index);
                if (version >= 6) grapple.noFreeMovement = bytes.ReadByte(ref index) != 0;
                if (version >= 7) grapple.specialObject = new() { specialObject = bytes.ReadString8(ref index) };
                if (version >= 8) grapple.visible = bytes.ReadByte(ref index) != 0;
                if (version >= 9)
                {
                    if (grapple.swingingRope) grapple.ropeType = (Grapple.RopeGrappleType) bytes.ReadByte(ref index);
                    else grapple.grappleType = (Grapple.ZipGrappleType) bytes.ReadByte(ref index);
                }
                if (version >= 10) grapple.blowup.SetBlowup(bytes.ReadString(ref index, 16));
                if (version >= 11) grapple.ropeBrightness = bytes.ReadByte(ref index) / 255f;

                grapple.transform.eulerAngles = grappleEuler;
            }
        }
    }
}
#endif