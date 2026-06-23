#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class GizFlock : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Finished", "Started" };

        public string creature;
        public short creatureCount;
        public int interactionOptions;
        public float unknown3, unknown4, unknown5, unknown6;
        public short unknown7;
        public float unknown8, unknown9, unknown10, unknown11, unknown12, unknown13, unknown14, unknown15, unknown16;
        public Vector3 unknown17;
        public float unknown18, unknown19;
        public string unknown20;
        public float unknown21, unknown22, unknown23, unknown24;
        public Unk25[] unknown25;
        public byte unknown26, unknown27;
        public Vector3 unknown28;
        public float unknown29;
        public Vector3 unknown30;

        private void OnValidate()
        {
            unknown25 ??= new Unk25[0];
            if (unknown25.Length > 65535)
            {
                EditorUtility.DisplayDialog("Too many [unknown25]", "Maximum of 65535 [unknown25] allowed on a single GizFlock.", "OK");
                unknown25 = unknown25.Take(65535).ToArray();
            }
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

        [Serializable]
        public struct Unk25
        {
            public byte unk1, unk2;
            public string unk3;
            public Vector3 unk4;
            public float unk5;
            public Vector3 unk6;
        }
    }
}
#endif