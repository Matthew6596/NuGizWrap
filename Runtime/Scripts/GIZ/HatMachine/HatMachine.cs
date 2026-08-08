#if UNITY_EDITOR
using System;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace NuGizWrap.Gizmos
{
    public class HatMachine : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Finished" };

        public enum Type
        {
            Random = 0, Leia = 1, Fedora = 2, TopHat=3, BaseballCap=4,
            Stormtrooper=5, BountyHunter=6, DroidPanel=7
        }

        public Type type;
        public Lever.HandleColor handleColor;
        public Transform target;
        public bool targetInvisible;

        private void OnValidate()
        {
            if(!Enum.IsDefined(typeof(Type), type)) type = Type.Stormtrooper;
            if(!Enum.IsDefined(typeof(Lever.HandleColor), handleColor)) handleColor = Lever.HandleColor.Yellow;
        }

        public static float GizmoScale = 0.25f, GizmoAlpha = 0.5f;
        public static Color GizmoColor = Color.white;
        private void OnDrawGizmos()
        {
            Color col = GizmoColor;
            col.a = GizmoAlpha;
            Giz.color = col;
            Giz.DrawSphere(transform.position, GizmoScale);
        }
    }
}
#endif