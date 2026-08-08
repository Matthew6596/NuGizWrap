#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using NuGizWrap.Helper;
using UnityEngine;

namespace NuGizWrap.AI
{
    public class LocatorSet : MonoBehaviour
    {
        public Locator[] locators;

        public void FromBytes(BinaryReader br, Locator[] locators)
        {
            name = br.ReadString(16).Trim();
            int locatorCount = br.ReadInt32();
            this.locators = new Locator[locatorCount];
            //byte[] locatorIndicies = br.ReadBytes(locatorCount);
            for (int i = 0; i < locatorCount; i++) this.locators[i] = locators[br.ReadByte()];
        }

        public void ToBytes(BinaryWriter bw, Locator[] locators)
        {
            bw.WriteString(name, 16);

            List<byte> bytes = new();
            int locatorCount = this.locators.Length;
            for (int i = 0; i < locatorCount; i++)
            {
                int ind = (byte)Array.IndexOf(locators, this.locators[i]);
                if (ind == -1 || this.locators[i] == null) locatorCount--;
                else bytes.Add((byte)ind);
            }

            bw.Write(locatorCount);
            bw.Write(bytes.ToArray());
        }
    }
}
#endif