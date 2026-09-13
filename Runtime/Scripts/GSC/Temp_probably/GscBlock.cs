#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    public abstract class GscBlock : MonoBehaviour
    {
        public abstract void Load(BinaryReader br);
        public abstract void Save(BinaryWriter bw);

        /// <summary>
        /// A second pass save method for calculating and writing all pointer offsets in this block.
        /// You can use the GetPtrAddress() function in the blocks where the pointers' values reside.
        /// </summary>
        public virtual void PostSave(BinaryWriter bw) { }

        /// <summary>
        /// Get the absolute address of a value to be pointed at given the key.
        /// </summary>
        /// <param name="key">The key for the pointer value, to be implemented by the inheriting class of GscBlock.</param>
        /// <returns>The absolute address of the pointer's value based on the given key.</returns>
        public virtual long GetPtrAddress(object key) => -1;
    }
}
#endif