#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(MiniCut.MiniCutPart))]
    public class MiniCutPartEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            serializedObject.Props("name", "targetPosition", "cameraDistance", "cameraOrbitEuler", "unknown8", "unknown9");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif