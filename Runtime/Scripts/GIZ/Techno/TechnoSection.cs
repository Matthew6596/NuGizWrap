//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.hx2or7y0yv4w
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class TechnoSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1,TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1=>6,TTGame.LB1 => 8, _ => 1 };

        public override string ID => "Techno";
        public static TechnoSection Instance { get; private set; }

        public int version = 6;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var technos = FindObjectsByType<Techno>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int technoCount = technos.Length;
            bytes.AddInt(technoCount);

            for(int i=0; i<technoCount; i++)
            {
                var techno = technos[i];
                bytes.AddFixedString(techno.name, 16);
                bytes.AddVector3(techno.transform.position);
                bytes.AddShort((short)techno.transform.eulerAngles.y.ToShortAng());

                if (version >= 8) bytes.AddString8(techno.controlType);
                if (version >= 2)
                {
                    bytes.Add(techno.unknown1);
                    bytes.AddString32(techno.controlledEntity);
                }
                if (version >= 3) bytes.Add(techno.unknown2);
                if (version >= 4) bytes.AddFloat(techno.cameraEmphasisAmount);
                if (version >= 5) bytes.AddInt(techno.unknown3);
                if (version >= 7) bytes.AddFloat(techno.unknown4);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int technoCount = bytes.ReadInt(ref index);

            //Clear existing shards before creating new ones
            foreach (var techno in FindObjectsByType<Techno>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) techno.gameObject.DelayDestroy();

            for(int i=0; i<technoCount; i++)
            {
                GameObject technoObj = new(bytes.ReadString(ref index, 16));
                technoObj.transform.SetParent(transform);
                technoObj.transform.position = bytes.ReadVector3(ref index);
                technoObj.transform.eulerAngles = new(0,((ushort)bytes.ReadShort(ref index)).ToFloatAng(), 0);
                var techno = technoObj.AddComponent<Techno>();

                if (version >= 8) techno.controlType = bytes.ReadString8(ref index);

                if (version >= 2)
                {
                    techno.unknown1 = bytes.ReadByte(ref index);
                    techno.controlledEntity = bytes.ReadString32(ref index);
                }

                if (version >= 3) techno.unknown2 = bytes.ReadByte(ref index);
                if (version >= 4) techno.cameraEmphasisAmount = bytes.ReadFloat(ref index);
                if (version >= 5) techno.unknown3 = bytes.ReadInt(ref index);
                if (version >= 7) techno.unknown4 = bytes.ReadFloat(ref index);
            }
        }
    }
}
#endif