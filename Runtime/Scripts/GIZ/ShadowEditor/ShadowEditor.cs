#if UNITY_EDITOR
using System;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class ShadowEditor : Gizmo
    {
        public enum Preset { Custom=0, NormalLevel=1, VehicleLevel=2 }

        public float opacity;
        public float unknown2, unknown3;
        public float unknown4, unknown5;
        public float renderDistance;
        public float blur, unknown10, unknown11;
        public float quality;
        public Preset preset;
        public float unknown14;

        private void OnValidate()
        {
            if (preset != Preset.Custom) RefreshPreset();
        }

        public static float GizmoScale = 0.1f, GizmoAlpha = 0.75f;
        public static Color GizmoColor = Color.black;
        private void OnDrawGizmos()
        {
            Color col = GizmoColor;
            col.a = GizmoAlpha;
            Giz.color = col;
            Giz.DrawSphere(transform.position, GizmoScale);
            Giz.DrawRay(transform.position, transform.forward);
        }

        public void RefreshPreset()
        {
            switch (preset)
            {
                case Preset.Custom: break;
                case Preset.NormalLevel:
                    unknown2 = 2;
                    unknown3 = 0.5f;
                    unknown4 = 0.0001f;
                    unknown5 = 0.0005f;
                    renderDistance = 7;
                    unknown14 = 5;
                    blur = 1.2f;
                    unknown10 = 0.48f;
                    unknown11 = 0;
                    quality = -0.5f;
                    opacity = 0.4f;
                    break;
                case Preset.VehicleLevel:
                    unknown2 = 2;
                    unknown3 = 0.1f;
                    unknown4 = 0.0005f;
                    unknown5 = 0.01f;
                    renderDistance = 25;
                    unknown14 = 15;
                    blur = 0.5f;
                    unknown10 = 0;
                    unknown11 = 0.5f;
                    quality = -15;
                    opacity = 0.4f;
                    break;
                default: preset = Preset.Custom; break;
            }
        }
    }
}
#endif