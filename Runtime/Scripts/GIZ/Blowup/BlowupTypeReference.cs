#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TTModdingKit.Gizmos
{
    [Serializable]
    public struct BlowupTypeReference
    {
        public bool referenceInScene;
        public BlowupType blowupType;
        public string blowupTypeName;

        public readonly string GetBlowupType()
        {
            if (referenceInScene)
            {
                if (blowupType == null)
                {
                    Debug.LogWarning($"Null reference to BlowupType, returning '{blowupTypeName}' for BlowupType reference");
                    return blowupTypeName;
                }
                return blowupType.name;
            }
            return blowupTypeName;
        }

        public void SetBlowupType(string name)
        {
            blowupTypeName = name;

            BlowupTypeReference blowRef = this;

            EditorApplication.delayCall += () => { blowRef.FetchBlowupType(); };
        }

        public void SetBlowupType(BlowupType blowup)
        {
            this.blowupType = blowup;
            blowupTypeName = (blowup == null) ? "" : blowup.name;
        }

        public void FetchBlowupType()
        {
            string blwupName = blowupTypeName;
            blowupType = GameObject.FindObjectsByType<BlowupType>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(b => b.name == blwupName).FirstOrDefault();
        }
    }
}
#endif