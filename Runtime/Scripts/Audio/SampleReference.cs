#if UNITY_EDITOR
using UnityEngine;
using System;

namespace NuGizWrap.Audio
{
    [Serializable]
    public struct SampleReference
    {
        public bool referenceInProject;
        public string sample;
    }
}
#endif