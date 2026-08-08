#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    [CreateAssetMenu(fileName = "Things", menuName = "TT Modding/Things")]
    public class Things : ScriptableObject
    {
        public static Things CurrentThings { get=>TTUnityProject.Instance.globalThings; } //TEMP

        public string[] objectNames = new string[0];
        public GameObject[] objects = new GameObject[0];

        private void OnValidate()
        {
            int nameLen = objectNames.Length;
            int objsLen = objects.Length;
            if (nameLen != objsLen)
            {
                if (nameLen > objsLen)
                {
                    GameObject[] newObjs = new GameObject[nameLen];
                    Array.Copy(objects, newObjs, objsLen);
                    objects = newObjs;
                }
                else
                {
                    objects = objects.Take(nameLen).ToArray();
                }
            }
        }

        public GameObject GetObject(string name)
        {
            int ind = Array.IndexOf(objectNames, name);
            return ind == -1 ? null : objects[ind];
        }
    }
}
#endif