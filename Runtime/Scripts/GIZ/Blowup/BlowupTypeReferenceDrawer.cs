#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace NuGizWrap.Gizmos
{
    [CustomPropertyDrawer(typeof(BlowupTypeReference))]
    public class BlowupTypeReferenceDrawer : PropertyDrawer
    {
        bool wasNameInput = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var refProp = property.FindPropertyRelative("referenceInScene");
            var blowupProp = property.FindPropertyRelative("blowupType");
            var nameProp = property.FindPropertyRelative("blowupTypeName");

            // Draw label, returns remaining rect
            position = EditorGUI.PrefixLabel(position, label);

            // Small toggle for referenceInScene
            float toggleW = 16f;
            Rect toggleRect = new(position.x, position.y, toggleW, position.height);
            Rect fieldRect = new(position.x + toggleW + 2f, position.y, position.width - toggleW - 2f, position.height);

            refProp.boolValue = EditorGUI.Toggle(toggleRect, new GUIContent("", "Check this to reference a BlowupType in the scene. Uncheck to enter BlowupType name manually."), refProp.boolValue);

            if (refProp.boolValue)
            {
                //If switching from textbox to reference, try finding blowupType with same name for reference value
                if (wasNameInput)
                {
                    var blwup = GameObject.FindObjectsByType<BlowupType>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(b => b.name == nameProp.stringValue).FirstOrDefault();
                    if(blwup != null) blowupProp.objectReferenceValue = blwup;
                }

                //Reference field for blowupType
                EditorGUI.PropertyField(fieldRect, blowupProp,new GUIContent());
                //Update blowupTypeName to match
                if (blowupProp.objectReferenceValue == null) nameProp.stringValue = "";
                else nameProp.stringValue = blowupProp.objectReferenceValue.name;

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