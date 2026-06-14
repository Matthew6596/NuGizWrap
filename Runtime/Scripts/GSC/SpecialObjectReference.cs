#if UNITY_EDITOR
using System;
using UnityEngine;

namespace TTModdingKit.GameScene
{
    [Serializable]
    public struct SpecialObjectReference
    {
        public bool referenceInScene;
        public string specialObject;
        public SpecialObject objectReference;
    }
}
#endif