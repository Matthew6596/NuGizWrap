#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Puzzle))]
    public class PuzzleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!PuzzleSection.Instance.CreateVersionEditorGUI(s => s.version, "Puzzle", out int version)) return;

            serializedObject.Props("unknown1", "unknown2");
            if (version >= 3) serializedObject.Prop("characterFacingPosition");
            if (version >= 4) serializedObject.Prop("targetPosition");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif