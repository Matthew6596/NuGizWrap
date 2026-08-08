#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(PushBlocks))]
    public class PushBlocksEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!PushBlocksSection.Instance.CreateVersionEditorGUI(s => s.version, "PushBlocks", out int version)) return;

            if (version >= 8) serializedObject.Prop("specialObject");
            serializedObject.Props("snapRange", "pushLocation", "unknown1", "lockZ", "lockX");

            if (version >= 4) serializedObject.Props("unknown2", "unknown3");
            if (version >= 5) serializedObject.Props("unknown4", "noSlip");
            if (version >= 3) serializedObject.Prop("linkObjects");
            if (version >= 6) serializedObject.Props("unknown5", "unknown6");
            if (version >= 7) serializedObject.Props("unknown7", "unknown8", "unknown9", "unknown10");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif