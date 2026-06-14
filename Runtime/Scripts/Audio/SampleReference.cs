#if UNITY_EDITOR
using UnityEngine;
using System;

namespace TTModdingKit.Audio
{
    [Serializable]
    public struct SampleReference
    {
        public bool referenceInProject;
        public string sample;
    }
}
#endif