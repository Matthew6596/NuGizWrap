#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(BlowupType))]
    public class BlowupTypeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!BlowupSection.Instance.CreateVersionEditorGUI(s => s.version, "Blowup", out int version)) return;

            serializedObject.Prop("specialObject");
            if (version >= 17) serializedObject.Props("parRef1", "parRef2");
            if (version >= 4) serializedObject.Props("ptlRef1", "ptlRef2", "ptlRef3");
            if (version >= 26) serializedObject.Props("unkRef1", "unkRef2");
            if (version >= 27) serializedObject.Props("unkRef3", "unkRef4");

            serializedObject.Prop("unknown1");
            if (version >= 7) serializedObject.Props("unknown2", "unknown3");
            if (version >= 8) serializedObject.Prop("unknown4");
            if (version >= 9) serializedObject.Prop("decal");
            if (version >= 14) serializedObject.Props("unknown5", "unknown6");
            if (version >= 15) serializedObject.Props("unknown7", "unknown8");

            if (version >= 16)
            {
                var nextDataProp = serializedObject.FindProperty("nextData");
                EditorGUILayout.PropertyField(nextDataProp);
                if (nextDataProp.boolValue) serializedObject.Prop("subDataSet");
            }

            if (version >= 18) serializedObject.Prop("emitObj1");
            if (version >= 22) serializedObject.Props("emitObj2", "emitObj3", "emitObj4");
            if (version >= 18) serializedObject.Props("unknown9", "unknown10", "unknown11");

            if (version >= 19) serializedObject.Prop("shadow");
            if (version >= 20) serializedObject.Prop("swap");

            if (version >= 23) serializedObject.Prop("unknown12");
            if (version >= 24) serializedObject.Prop("unknown13");

            if (version >= 33) serializedObject.Prop("unknown14");
            if (version >= 38) serializedObject.Prop("unknown15");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif