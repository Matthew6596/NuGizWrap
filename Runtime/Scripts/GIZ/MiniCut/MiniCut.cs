#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class MiniCut : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Played", "Playing" };

        public float startDelay, duration, blendInTime, blendOutTime, maxTotalDuration;
        public MiniCutPart[] miniCutParts;

        private void OnValidate()
        {
            miniCutParts ??= new MiniCutPart[0];
            if (miniCutParts.Length > 255)
            {
                EditorUtility.DisplayDialog("Too many MiniCut Parts", "Maximum of 255 minicut parts allowed on a single MiniCut.", "OK");
                miniCutParts = miniCutParts.Take(255).ToArray();
            }
        }

        [Serializable]
        public struct MiniCutPart
        {
            public string name;
            public Vector3 targetPosition;
            public float cameraDistance;
            public Vector3 cameraOrbitEuler;
            public float easeInTime, duration;
        }

        public static float GizmoScale = 0.25f, GizmoAlpha = 0.5f;
        public static Color GizmoColor = Color.white;
        private void OnDrawGizmos()
        {
            if (miniCutParts == null) return;
            foreach (var stage in miniCutParts)
            {
                Color col = GizmoColor;
                col.a = GizmoAlpha;
                Giz.color = col;
                Giz.DrawSphere(stage.targetPosition, GizmoScale); //temp, change to cam pos + ray
            }
        }
    }
}
#endif