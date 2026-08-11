#if UNITY_EDITOR
using System;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomEditor(typeof(SecurityDoor))]
    public class SecurityDoorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!SecurityDoorSection.Instance.CreateVersionEditorGUI(s => s.version, "SecurityDoor", out int version)) return;

            if (version >= 2)
            {
                //Creating string for helpbox, showing valid types
                var game = TTUnityProject.Game;
                string validStr = "";
                var validArr = SecurityDoor.GetTypes(game);
                int validLen = validArr.Length;
                for(int i=0; i< validLen; i++)
                {
                    if(i != 0) validStr += ", ";
                    if (i == validLen - 1) validStr += "and ";
                    validStr += $"'{validArr[i]}'";
                }
                EditorGUILayout.HelpBox($"The valid types for {game} are: {validStr}.", MessageType.None);
                //

                serializedObject.Prop("type");
            }
            if (version >= 3) serializedObject.Prop("specialObject");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif