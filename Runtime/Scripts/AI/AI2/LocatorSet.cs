#if UNITY_EDITOR
using System.IO;
using TTModdingKit.Helper;
using UnityEngine;

namespace TTModdingKit.AI
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

        public void ToBytes(BinaryWriter bw)
        {

        }
    }
}
#endif