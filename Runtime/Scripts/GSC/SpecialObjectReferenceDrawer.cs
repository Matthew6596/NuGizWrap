#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace TTModdingKit.GameScene
{
    [CustomPropertyDrawer(typeof(SpecialObjectReference))]
    public class SpecialObjectReferenceDrawer : PropertyDrawer
    {
        bool wasNameInput = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var refProp = property.FindPropertyRelative("referenceInScene");
            var nameProp = property.FindPropertyRelative("specialObject");
            var objProp = property.FindPropertyRelative("objectReference");

            // Draw label, returns remaining rect
            position = EditorGUI.PrefixLabel(position, label);

            // Small toggle for referenceInScene
            float toggleW = 16f;
            Rect toggleRect = new(position.x, position.y, toggleW, position.height);
            Rect fieldRect = new(position.x + toggleW + 2f, position.y, position.width - toggleW - 2f, position.height);

            refProp.boolValue = EditorGUI.Toggle(toggleRect, new GUIContent("", "Check this to reference a Special Object in the scene. Uncheck to enter BlowupType name manually."), refProp.boolValue);

            if (refProp.boolValue)
            {
                //If switching from textbox to reference, try finding blowupType with same name for reference value
                if (wasNameInput)
                {
                    var blwup = GameObject.FindObjectsByType<SpecialObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(b => b.name == nameProp.stringValue).FirstOrDefault();
                    if(blwup != null) objProp.objectReferenceValue = blwup;
                }

                //Reference field for blowupType
                EditorGUI.PropertyField(fieldRect, objProp, new GUIContent(""));
                //Update blowupTypeName to match
                if (objProp.objectReferenceValue != null) nameProp.stringValue = objProp.objectReferenceValue.name;

                wasNameInput = false;
            }
            else
            {
                //Textfield for blowupName
                nameProp.stringValue = EditorGUI.TextField(fieldRect, nameProp.stringValue);

                wasNameInput = true;
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif