#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(ZipUp))]
    public class ZipUpEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!ZipUpSection.Instance.CreateVersionEditorGUI(s => s.version, "ZipUp", out int version)) return;

            serializedObject.Props("start","axis","end","unknown1","unknown2","swing","unknown3","twoWay");
            if (version >= 2) serializedObject.Prop("invisible");
            if (version >= 3) serializedObject.Prop("unknown4");
            if (version >= 4) serializedObject.Prop("targetsInvisible");
            if (version >= 5) serializedObject.Prop("unknown5");
            if (version >= 6) serializedObject.Props("unknown6", "unknown7");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif