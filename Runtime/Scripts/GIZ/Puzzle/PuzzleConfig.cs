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

    public class PuzzleConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.LIJ1 => 4, _ => 1 };

        public int version = 4;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadInt32();
            int puzzleCount = br.ReadInt32();

            string[] existingNames = new string[puzzleCount];
            for (int i = 0; i < puzzleCount; i++)
            {
                string name = br.ReadString8();

                name = ObjectNames.GetUniqueName(existingNames, name);
                existingNames[i] = name;

                GameObject puzzleObj = new(name);
                puzzleObj.transform.SetParent(parent);
                puzzleObj.transform.position = br.ReadVector3();
                var puzzle = puzzleObj.AddComponent<Puzzle>();

                puzzle.unknown1 = br.ReadSingle();
                puzzle.unknown2 = br.ReadByte() != 0;

                puzzleObj.transform.eulerAngles = br.ReadXYEuler();

                if (version >= 3) puzzle.characterFacingPosition = br.ReadVector3();
                if (version >= 4) puzzle.targetPosition = br.ReadVector3();
            }

            return puzzleCount;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(version);

            var puzzles = FindObjectsByType<Puzzle>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            int puzzleCount = puzzles.Length;
            bw.Write(puzzleCount);

            for (int i = 0; i < puzzleCount; i++)
            {
                var puzzle = puzzles[i];
                bw.WriteString8(puzzle.name);
                bw.Write(puzzle.transform.position);

                bw.Write(puzzle.unknown1);
                bw.Write((byte)(puzzle.unknown2 ? 1 : 0));

                Vector3 euler = puzzle.transform.eulerAngles;
                bw.Write(euler.x.ToShortAng());
                bw.Write(euler.y.ToShortAng());

                if (version >= 3) bw.Write(puzzle.characterFacingPosition);
                if (version >= 4) bw.Write(puzzle.targetPosition);
            }
        }
    }
}
#endif