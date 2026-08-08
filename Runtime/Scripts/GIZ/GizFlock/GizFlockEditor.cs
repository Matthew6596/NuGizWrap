#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(GizFlock))]
    public class GizFlockEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GizFlockSection.Instance.CreateVersionEditorGUI(s => s.version, "GizFlock", out int version)) return;

            serializedObject.Props("creature", "creatureCount", "interactionOptions", "unknown3", "unknown4", "unknown5", "unknown6", "unknown7", "unknown8", "unknown9", "unknown10", "unknown11", "unknown12", "unknown13", "unknown14", "unknown15", "unknown16", "unknown17", "unknown18", "unknown19", "unknown20", "unknown21", "unknown22", "unknown23", "unknown24", "unknown25");

            if (version >= 1)
            {
                var unk26Prop = serializedObject.FindProperty("unknown26");
                EditorGUILayout.PropertyField(unk26Prop);

                serializedObject.Props("unknown27", "unknown28");

                byte unk26 = (byte)unk26Prop.intValue;
                if (unk26 == 2 || unk26 == 3) serializedObject.Prop("unknown29");
                else if (unk26 == 4 || unk26 == 5) serializedObject.Prop("unknown30");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif