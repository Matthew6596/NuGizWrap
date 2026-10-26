#if UNITY_EDITOR
using System;
using UnityEngine;

namespace NuGizWrap.Audio
{
    [Serializable]
    public struct AudioClipReference
    {
        public string clipPath;
        public AudioClip clip;
        public bool referenceAsset;
    }
}
#endif