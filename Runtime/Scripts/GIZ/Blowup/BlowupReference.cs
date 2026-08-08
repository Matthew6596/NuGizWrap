#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    [Serializable]
    public struct BlowupReference
    {
        public bool referenceInScene;
        public Blowup blowup;
        public string blowupName;

        public readonly string GetBlowup()
        {
            if (referenceInScene)
            {
                if (blowup == null)
                {
                    Debug.LogWarning($"Null reference to blowup, returning '{blowupName}' for blowup reference");
                    return blowupName;
                }
                return blowup.name;
            }
            return blowupName;
        }

        public void SetBlowup(string name)
        {
            blowupName = name;

            BlowupReference blowRef = this;

            EditorApplication.delayCall += () => { blowRef.FetchBlowup(); };
        }

        public void SetBlowup(Blowup blowup)
        {
            this.blowup = blowup;
            blowupName = (blowup == null) ? "" : blowup.name;
        }

        public void FetchBlowup()
        {
            string blwupName = blowupName;
            blowup = GameObject.FindObjectsByType<Blowup>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(b => b.name == blwupName).FirstOrDefault();
        }
    }
}
#endif