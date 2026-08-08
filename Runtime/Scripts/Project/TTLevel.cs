#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace NuGizWrap
{
    [CreateAssetMenu(fileName = "TTLevel", menuName = "TT Modding/Level")]
    public class TTLevel : ScriptableObject
    {
        [Tooltip("Exclude this level when exporting")]
        public bool excludeFromExport;

        // ===== Levels.txt =====
        [Flags]
        public enum Type { Default=0, Test=1, Intro=2, Midtro=4, Cutscene=8, Outro=16, Status=32, NewGame=64, LoadGame=128 }

        public SceneAsset scene;
        public Type type;
        // ----- End Levels.txt -----

        // ===== filename.txt =====
        [TextArea(10,24)]
        public string levelTxt;

        // ----- End filename.txt -----
    }
}
#endif