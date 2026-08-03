#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using GameScene;

    [CustomEditor(typeof(Ledge))]
    public class LedgeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!LedgeSection.Instance.CreateVersionEditorGUI(s => s.version, "Ledge", out int version)) return;

            serializedObject.Prop("type");
            if (version >= 2) serializedObject.Props("leftLedge", "rightLedge");
            if (version >= 3) serializedObject.Prop("interactOptions");
            if (version >= 4)
            {
                var unk4Prop = serializedObject.FindProperty("specialObject");
                EditorGUILayout.PropertyField(unk4Prop);

                if (((SpecialObjectReference)unk4Prop.boxedValue).specialObject.Length > 0) 
                    serializedObject.Props("specialObjectPos", "specialObjectAng");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif