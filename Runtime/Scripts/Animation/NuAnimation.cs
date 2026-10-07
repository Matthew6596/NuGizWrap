#if UNITY_EDITOR
using NuGizWrap.Gizmos;
using System;
using UnityEngine;

namespace NuGizWrap.Animations
{
    public class NuAnimation : MonoBehaviour
    {
        public ushort keyFrameCount;
        public ushort frameCount;
        public ushort firstFrame;
        public ushort endFrame;
        public BoneAnim[] bones;
        public KeyFrame[] KeyFrames;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlayAnim()
        {

        }

        [Serializable]
        public struct BoneAnim
        {

            public KeyFrame[] keyFrames;
            public int flags;
        }

        [Serializable]
        public struct KeyFrame
        {
            public Frame keyframe;
            public float interpolationFrame1;
            public float interpolationFrame2;
            public float interpolationFrame3;
        }

        [Serializable]
        public struct Frame
        {
            public Vector3 position;
            public Vector3 euler;
            public Vector3 scale;
        }
    }
}
#endif