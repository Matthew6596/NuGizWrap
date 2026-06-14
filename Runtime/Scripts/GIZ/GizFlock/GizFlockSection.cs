//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.ns7vggj7q1zq
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class GizFlockSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1, TTGame.LB1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1=>4, TTGame.LB1 => 2, _ => 1 };

        public override string ID => "GizFlock";
        public static GizFlockSection Instance { get; private set; }

        public int version = 2;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);

            //Vanilla LIJ1 only has version 4, Vanilla LB1 only has version 2.
            //The GizFlock loading code between games seems to differ and lack proper versioning, so...
            //For TTModdingKit, version 2 == LB1's structure and version 4 == LIJ1's structure
            //Due to this, I've also removed some versioning that is meaningless (version >= 2 is always true)
            if (version != 2 && version != 4) version = TTUnityProject.Game == TTGame.LIJ1 ? 4 : 2;
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var flocks = FindObjectsByType<GizFlock>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            short flockCount = (short)flocks.Length; //technically ushort but I don't care, you do not deserve more than 30k gizflocks
            bytes.AddShort(flockCount);

            for(int i=0; i<flockCount; i++)
            {
                var flock = flocks[i];
                bytes.AddString8(flock.name);
                bytes.AddString8(flock.creature);
                bytes.AddShort(flock.creatureCount);

                bytes.AddInt(flock.interactionOptions);

                bytes.AddVector3(flock.transform.position);

                bytes.AddFloat(flock.unknown3);
                bytes.AddFloat(flock.unknown4);
                bytes.AddFloat(flock.unknown5);
                bytes.AddFloat(flock.unknown6);
                bytes.AddShort(flock.unknown7);
                bytes.AddFloat(flock.unknown8);
                bytes.AddFloat(flock.unknown9);
                bytes.AddFloat(flock.unknown10);
                bytes.AddFloat(flock.unknown11);
                bytes.AddFloat(flock.unknown12);
                bytes.AddFloat(flock.unknown13);
                bytes.AddFloat(flock.unknown14);
                bytes.AddFloat(flock.unknown15);
                bytes.AddFloat(flock.unknown16);
                bytes.AddVector3(flock.unknown17);
                bytes.AddFloat(flock.unknown18);
                bytes.AddFloat(flock.unknown19);
                bytes.AddString8(flock.unknown20);
                bytes.AddFloat(flock.unknown21);
                bytes.AddFloat(flock.unknown22);
                bytes.AddFloat(flock.unknown23);
                bytes.AddFloat(flock.unknown24);

                ushort unk25Count = (ushort)flock.unknown25.Length;
                bytes.AddShort((short)unk25Count);
                for(int j=0; j<unk25Count; j++)
                {
                    GizFlock.Unk25 unk25 = flock.unknown25[j];
                    byte unk1 = unk25.unk1;
                    bytes.Add(unk1);

                    bytes.Add(unk25.unk2);
                    bytes.AddString8(unk25.unk3);

                    bytes.AddVector3(unk25.unk4);

                    if (unk1 == 0) bytes.AddFloat(unk25.unk5);
                    else if (unk1 == 1) bytes.AddVector3(unk25.unk6);
                }
                
                byte unk26 = flock.unknown26;
                bytes.Add(unk26);
                bytes.Add(flock.unknown27);
                bytes.AddVector3(flock.unknown28);

                if (unk26 == 2 || unk26 == 3) bytes.AddFloat(flock.unknown29);
                else if (unk26 == 4 || unk26 == 5) bytes.AddVector3(flock.unknown30);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            short flockCount = bytes.ReadShort(ref index);

            //Clear existing flocks before creating new ones
            foreach (var flock in FindObjectsByType<GizFlock>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) flock.gameObject.DelayDestroy();

            for(int i=0; i<flockCount; i++)
            {
                GameObject flockObj = new(bytes.ReadString8(ref index));
                flockObj.transform.SetParent(transform);
                
                var flock = flockObj.AddComponent<GizFlock>();

                flock.creature = bytes.ReadString8(ref index);
                flock.creatureCount = bytes.ReadShort(ref index);

                flock.interactionOptions = bytes.ReadInt(ref index);

                flockObj.transform.position = bytes.ReadVector3(ref index);
                flock.unknown3 = bytes.ReadFloat(ref index);
                flock.unknown4 = bytes.ReadFloat(ref index);
                flock.unknown5 = bytes.ReadFloat(ref index);
                flock.unknown6 = bytes.ReadFloat(ref index);

                if (version == 4) //LIJ1
                {
                    flock.unknown7 = bytes.ReadShort(ref index);
                    flock.unknown8 = bytes.ReadFloat(ref index);
                    flock.unknown9 = bytes.ReadFloat(ref index);
                    flock.unknown10 = bytes.ReadFloat(ref index);
                    flock.unknown11 = bytes.ReadFloat(ref index);
                    flock.unknown12 = bytes.ReadFloat(ref index);
                    flock.unknown13 = bytes.ReadFloat(ref index);
                    flock.unknown14 = bytes.ReadFloat(ref index);
                    flock.unknown15 = bytes.ReadFloat(ref index);
                    flock.unknown16 = bytes.ReadFloat(ref index);
                    flock.unknown17 = bytes.ReadVector3(ref index);
                    flock.unknown18 = bytes.ReadFloat(ref index);
                    flock.unknown19 = bytes.ReadFloat(ref index);
                }
                else if (version == 2) //LB1
                {
                    index += 82;
                    Debug.LogWarning("GizFlock is not implemented fully yet for LB1.");
                }
                else throw new System.ArgumentException($"GizFlockSection version must be either 2 for LB1 or 4 for LIJ1, but it is {version}.");

                flock.unknown20 = bytes.ReadString8(ref index);
                flock.unknown21 = bytes.ReadFloat(ref index);
                flock.unknown22 = bytes.ReadFloat(ref index);
                flock.unknown23 = bytes.ReadFloat(ref index);
                flock.unknown24 = bytes.ReadFloat(ref index);

                ushort unk25Count = (ushort)bytes.ReadShort(ref index);
                flock.unknown25 = new GizFlock.Unk25[unk25Count];
                for (int j = 0; j < unk25Count; j++)
                {
                    GizFlock.Unk25 unk25 = new();
                    byte unk1 = bytes.ReadByte(ref index);
                    unk25.unk1 = unk1;

                    unk25.unk2 = bytes.ReadByte(ref index);
                    unk25.unk3 = bytes.ReadString8(ref index);

                    unk25.unk4 = bytes.ReadVector3(ref index);

                    if (unk1 == 0) unk25.unk5 = bytes.ReadFloat(ref index);
                    else if (unk1 == 1) unk25.unk6 = bytes.ReadVector3(ref index);

                    flock.unknown25[j] = unk25;
                }

                byte unk26 = bytes.ReadByte(ref index);
                flock.unknown26 = unk26;
                flock.unknown27 = bytes.ReadByte(ref index);
                flock.unknown28 = bytes.ReadVector3(ref index);

                if (unk26 == 2 || unk26 == 3) flock.unknown29 = bytes.ReadFloat(ref index);
                else if (unk26 == 4 || unk26 == 5) flock.unknown30 = bytes.ReadVector3(ref index);
            }
        }
    }
}
#endif