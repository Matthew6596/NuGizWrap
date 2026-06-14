#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TTModdingKit.Audio
{
    public class Sample : MonoBehaviour
    {
        //Please ensure audio clip file path is set in data
        public AudioClip clip;

        [Tooltip("Sound is not loaded, played, or searchable.")]
        public bool isDisabled;
        [Tooltip("Sound entry is just a comment.")]
        public bool isComment;

        [Tooltip("The pitch of the sample. 22050 is the standard value used.")]
        public int pitch = 22050;
        public sbyte priority;
        public bool isGlobal;

        //Optional checks
        public bool panOpt, pitchRandomOpt, volumeOpt, volumeRandOpt, nearOpt, farOpt, rumbleOpt, fcatOpt;

        [Range(-1f, 1f)]
        public float pan;
        public bool loop;
        [Range(0f, 1f)]
        public float pitchRandomness;
        [Range(0, 16383)]
        public int volume;
        [Range(0f, 1f)]
        public float volumeRandomness;

        [Tooltip("Near Attenuation")]
        public float near;
        [Tooltip("Far Attenuation")]
        public float far;

        [Tooltip("buzzTime(0-5sec) rumbleStr(0-255) rumbleSustain(0-5sec) rumbleRelease(0-5sec)")]
        public Rumble rumble;
        //group
        public bool sequentialMode;
        [Tooltip("Fall off category number")]
        public int fallOffCategory;

        public string ToLine()
        {
            string line = "Sample ";
            if (isDisabled) line += "disable ";
            if (isComment) line += "comment ";
            line += $"name \"{name}\" ";

            string fname = clip.GetTTPath();
            line += $"fname \"{fname}\" ";

            return line;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parts"></param>
        /// <returns>Whether this clip is creating a group.</returns>
        public bool FromLine(List<string> parts)
        {
            string GetVal(string valName) => parts[parts.IndexOf(valName) + 1];

            string fname = GetVal("fname");
            //attempt to find audio clip
            var clip = TTUnityProject.GetAllAudioClips().Where((c) => c.GetTTPath() == fname).FirstOrDefault();
            if(clip == null)
            {
                //TTUnityProject.AbsoluteProjectAssetPath+"/Audio";
            }

            return false;
        }

        [Serializable]
        public struct Rumble
        {
            [Range(0f,5f)]
            public float buzzTime;
            public byte stength;
            [Range(0f,5f)]
            public float sustain;
            [Range(0f,5f)]
            public float release;
        }
    }
}
#endif