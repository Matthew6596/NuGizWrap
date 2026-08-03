//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.rn49ko9vx6qu
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class AttractoSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LB1 => 3, _ => 1 };

        public override string ID => "Attracto";
        public static AttractoSection Instance { get; private set; }

        public int version = 3;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var attractos = FindObjectsByType<Attracto>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int attractoCount = attractos.Length;
            bytes.AddInt(attractoCount);

            for(int i=0; i<attractoCount; i++)
            {
                var attracto = attractos[i];
                bytes.AddFixedString(attracto.name, 16);
                bytes.AddVector3(attracto.transform.position);
                bytes.AddShort((short)attracto.transform.eulerAngles.y.ToShortAng());

                bytes.Add(attracto.pieceCount);
                if (version == 2) bytes.Add(0); //unused string8 property
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int attractoCount = bytes.ReadInt(ref index);

            //Clear existing shards before creating new ones
            foreach (var attracto in FindObjectsByType<Attracto>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))attracto.gameObject.DelayDestroy();

            for(int i=0; i<attractoCount; i++)
            {
                GameObject attractoObj = new(bytes.ReadString(ref index, 16));
                attractoObj.transform.SetParent(transform);
                attractoObj.transform.position = bytes.ReadVector3(ref index);
                attractoObj.transform.eulerAngles = bytes.ReadYEuler(ref index);
                var attracto = attractoObj.AddComponent<Attracto>();

                attracto.pieceCount = bytes.ReadByte(ref index);
                if (version == 2) bytes.ReadString8(ref index); //unused string8 property
            }
        }
    }
}
#endif