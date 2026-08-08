#if UNITY_EDITOR
using System;
using UnityEngine;

namespace NuGizWrap
{
    [CreateAssetMenu(fileName = "TTArea", menuName = "TT Modding/Area (Chapter)")]
    public class TTArea : ScriptableObject
    {
        [Tooltip("Exclude this area when exporting")]
        public bool excludeFromExport;

        // ===== Areas.txt =====
        [Flags]
        public enum Type { Default = 0, Test = 1, Vehicle = 2, Bonus = 4, Hub = 8, OverrideThings = 16, SingleBuffer=32, EndingArea = 64, NoPickupGravity=128, NoCharacterCollision=256, NoGoldBrick=512, NoCompletionPoints=1024, NoFreeplay=2048 }

        public TTLevel[] levels;

        public string minikit, redbrick;
        public Type type;
        public int nameId;
        public Vector2Int textId = new(-1,-1);
        public float timeTrialTime;
        // ----- End Areas.txt -----

        // ===== filename.txt =====
        public Character[] characters;
        public AIMessage[] aiMessages;
        public int storyCoins;
        public int freeplayCoins;

        public string music;

        public Stream[] streaming;
        // ----- End filename.txt -----

        [Serializable]
        public struct Character
        {
            public enum Type { Player, Resident, Cutscene }

            public string name;
            public Type type;
        }

        [Serializable]
        public struct AIMessage
        {
            public string msg;
        }

        [Serializable]
        public struct Stream
        {
            public string[] levels;
        }
    }
}
#endif