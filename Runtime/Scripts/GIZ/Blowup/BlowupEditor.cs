#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Blowup))]
    public class BlowupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!BlowupSection.Instance.CreateVersionEditorGUI(s => s.version, "Blowup", out int version)) return;

            serializedObject.Props("type", "unknown1", "unknown2", "unknown3");
            if (version >= 2 && version <= 19) serializedObject.Prop("unknown4a");
            if (version >= 20) serializedObject.Prop("interactionOptions");
            if (version == 28) serializedObject.Prop("unknown4b");
            if (version >= 30 && version < 34) serializedObject.Prop("unknown5");

            if (version >= 34) serializedObject.Props("unknown33", "unknown34");
            if (version >= 41) serializedObject.Props("unknown35", "unknown36", "unknown37");

            if (version >= 2) serializedObject.Props("studsValue", "unknown6", "unknown7");
            if (version >= 4) serializedObject.Prop("damage");
            if (version >= 6) serializedObject.Prop("range");
            if (version >= 8) serializedObject.Props("unknown8", "unknown9");
            if (version >= 9) serializedObject.Props("unknown10", "unknown11", "unknown12", "unknown13", "unknown14", "unknown15");
            if (version >= 10) serializedObject.Prop("unknown16");
            if (version >= 11) serializedObject.Props("unknown17", "unknown18", "unknown19");
            if (version >= 12) serializedObject.Prop("unknown20");
            if (version >= 13) serializedObject.Props("unknown21", "unknown22");
            if (version >= 19) serializedObject.Props("unknown23", "unknown24", "unknown25", "unknown26", "unknown27", "unknown28", "unknown29");
            if (version >= 21) serializedObject.Prop("unknown30");
            if (version >= 23) serializedObject.Prop("unknown31");
            if (version >= 31) serializedObject.Prop("unknown32");

            if (version >= 32) serializedObject.Prop("unknown38");
            if (version >= 36) serializedObject.Prop("unknown39");
            //if (version >= 36) EditorGUILayout.HelpBox("Property unknown39 is currently unsupported. Exporting/reading will result in incorrect/corrupt data.", MessageType.Warning);
            if (version >= 37) serializedObject.Prop("unknown40");
            if (version >= 38) serializedObject.Prop("unknown41");
            if (version >= 40) serializedObject.Prop("unknown42");
            if (version >= 41) serializedObject.Prop("unknown43");
            if (version >= 44) serializedObject.Prop("unknown44");
            //if (version >= 44) EditorGUILayout.HelpBox("Property unknown44 is currently unsupported. Exporting/reading will result in incorrect/corrupt data.", MessageType.Warning);
            if (version >= 45) serializedObject.Prop("unknown45");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif