#if UNITY_EDITOR
using UnityEngine;

namespace TTModdingKit
{
    [CreateAssetMenu(fileName = "TTEpisode", menuName = "TT Modding/Episode")]
    public class TTEpisode : ScriptableObject
    {
        [Tooltip("Exclude this episode when exporting")]
        public bool excludeFromExport;

        public TTArea[] areas;
        public int nameId, textId;
    }
}
#endif