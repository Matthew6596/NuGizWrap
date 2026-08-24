#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NuGizWrap.GizFlow
{
    using Gizmos;

    public class GizRandom : Gizmo
    {
        public Output[] outputs;

        public override string[] GetOutputNames(TTGame game)
        {
            outputs ??= new Output[0];

            List<string> outStrs = new();
            for(int i=0; i<outputs.Length; i++)
            {
                var o = outputs[i];
                outStrs.Add(string.IsNullOrEmpty(o.name) ? $"Output #{i}" : o.name);
            }

            return outStrs.ToArray();
        }

        [Serializable]
        public struct Output
        {
            public string name;
            public int chance;
        }

        public bool IsValid()
        {
            int sum = 0;
            foreach(var o in outputs) sum += o.chance;
            return sum == 100;
        }
    }
}
#endif