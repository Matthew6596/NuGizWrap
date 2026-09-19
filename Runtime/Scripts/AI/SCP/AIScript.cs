#if UNITY_EDITOR
using System;
using UnityEngine;

namespace NuGizWrap.AI.Scripting
{
    public class AIScript : ScriptableObject
    {
        [TextArea]
        public string text;
    }
}
#endif