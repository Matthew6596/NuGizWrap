//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.j804i5hxkfx8
//-Matton
//===== ===== ===== ===== =====
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System.Linq;

    public class PuzzleSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 4, _ => 1 };

        public override string ID => "Puzzle";
        public static PuzzleSection Instance { get; private set; }

        public int version = 4;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddInt(version);

            var puzzles = FindObjectsByType<Puzzle>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int puzzleCount = puzzles.Length;
            bytes.AddInt(puzzleCount);

            for(int i=0; i<puzzleCount; i++)
            {
                var puzzle = puzzles[i];
                bytes.AddString8(puzzle.name);
                bytes.AddVector3(puzzle.transform.position);

                bytes.AddFloat(puzzle.unknown1);
                bytes.Add((byte)(puzzle.unknown2 ? 1 : 0));

                Vector3 euler = puzzle.transform.eulerAngles;
                bytes.AddShort((short)euler.x.ToShortAng());
                bytes.AddShort((short)euler.y.ToShortAng());

                if (version >= 3) bytes.AddVector3(puzzle.characterFacingPosition);
                if (version >= 4) bytes.AddVector3(puzzle.targetPosition);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadInt(ref index);
            int puzzleCount = bytes.ReadInt(ref index);

            //Clear existing puzzles before creating new ones
            foreach (var puz in FindObjectsByType<Puzzle>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) puz.gameObject.DelayDestroy();

            for(int i=0; i<puzzleCount; i++)
            {
                GameObject puzzleObj = new(bytes.ReadString8(ref index));
                puzzleObj.transform.SetParent(transform);
                puzzleObj.transform.position = bytes.ReadVector3(ref index);
                var puzzle = puzzleObj.AddComponent<Puzzle>();

                puzzle.unknown1 = bytes.ReadFloat(ref index);
                puzzle.unknown2 = bytes.ReadByte(ref index) != 0;

                float pitch = ((ushort)bytes.ReadShort(ref index)).ToFloatAng();
                float yaw = ((ushort)bytes.ReadShort(ref index)).ToFloatAng();
                puzzleObj.transform.eulerAngles = new(pitch, yaw, 0);

                if (version >= 3) puzzle.characterFacingPosition = bytes.ReadVector3(ref index);
                if (version >= 4) puzzle.targetPosition = bytes.ReadVector3(ref index);
            }
        }
    }
}
#endif