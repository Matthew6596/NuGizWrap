#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomEditor(typeof(Grapple))]
    public class GrappleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!GrappleSection.Instance.CreateVersionEditorGUI(s => s.version, "Grapple", out int version)) return;

            if (version < 2) serializedObject.Prop("unknown1");
            serializedObject.Prop("unknown2");
            if (version >= 3) serializedObject.Prop("unknown3");
            if (version >= 4) serializedObject.Props("unknown4", "length");
            if (version >= 5) serializedObject.Prop("unknown6");
            if (version >= 6) serializedObject.Prop("unknown7");
            if (version >= 7) serializedObject.Prop("specialObject");
            if (version >= 8) serializedObject.Prop("unknown8");
            if (version >= 9) serializedObject.Prop("unknown9");
            if (version >= 10) serializedObject.Prop("blowup");
            if (version >= 11) serializedObject.Prop("unknown10");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif