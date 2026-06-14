#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TTModdingKit.GameScene
{
    [CustomEditor(typeof(Things))]
    public class ThingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var namesProp = serializedObject.FindProperty("objectNames");
            var objsProp = serializedObject.FindProperty("objects");

            int arrLen = namesProp.arraySize;
            EditorGUILayout.LabelField("Objects");
            for(int i=0; i< arrLen; i++)
            {
                var nameProp = namesProp.GetArrayElementAtIndex(i);
                var objProp = objsProp.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(nameProp, new GUIContent(""));
                objProp.objectReferenceValue = EditorGUILayout.ObjectField(objProp.objectReferenceValue, typeof(GameObject), false);

                if (GUILayout.Button("v", GUILayout.Width(20)) && i != arrLen)
                {
                    namesProp.MoveArrayElement(i, i + 1);
                    objsProp.MoveArrayElement(i, i + 1);
                    break;
                }

                if (GUILayout.Button("^", GUILayout.Width(20)) && i != 0) 
                {
                    namesProp.MoveArrayElement(i, i - 1);
                    objsProp.MoveArrayElement(i, i - 1);
                    break;
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    namesProp.DeleteArrayElementAtIndex(i);
                    objsProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            if(GUILayout.Button("+", GUILayout.Width(20)))
            {
                namesProp.InsertArrayElementAtIndex(arrLen);
                objsProp.InsertArrayElementAtIndex(arrLen);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif