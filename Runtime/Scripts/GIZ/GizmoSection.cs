#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TTModdingKit.Gizmos
{
    using Helper;
    public abstract class GizmoSection : MonoBehaviour, IGameCompatible
    {
        public abstract bool IsGameCompatible(TTGame game);
        public abstract int GetMaxVersion(TTGame game);

        public abstract string ID { get; }

        /// <summary>
        /// Converts the gizmo section to bytes, excluding ID and length
        /// </summary>
        /// <returns></returns>
        public abstract byte[] ToBytes();

        /// <summary>
        /// Loads a gizmo section from bytes, exluding ID and length
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        public abstract void FromBytes(byte[] bytes, ref int index);

        protected T DoSingleton<T>(T instance) where T : GizmoSection
        {
            if (instance == null) return this as T;
            else if (this != instance)
            {
                EditorUtility.DisplayDialog($"Cannot Create {ID}Section", $"There can only be one instance of {ID}Section, and there is already one on GameObject '{instance.gameObject.name}'", "OK");
                this.DelayDestroy();
            }
            return instance;
        }
    }
}
#endif