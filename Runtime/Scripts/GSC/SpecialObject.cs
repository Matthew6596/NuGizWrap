#if UNITY_EDITOR
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Gizmos;

    public class SpecialObject : Gizmo
    {
        public IABLBlock.IABLObject localIABLObject;
        public float unk1, unk2, unk3, unk4;

        public int visibilityFn, lodPtr, boundingBoxIndex, iablObjectPtr;
        public short windShearFactor, windSpeedFactor;
        public int unkPtr;
    }
}
#endif