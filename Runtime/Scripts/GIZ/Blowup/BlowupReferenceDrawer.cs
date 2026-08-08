#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace NuGizWrap.Gizmos
{
    [CustomPropertyDrawer(typeof(BlowupReference))]
    public class BlowupReferenceDrawer : PropertyDrawer
    {
        bool wasNameInput = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var refProp = property.FindPropertyRelative("referenceInScene");
            var blowupProp = property.FindPropertyRelative("blowup");
            var nameProp = property.FindPropertyRelative("blowupName");

            // Draw label, returns remaining rect
            position = EditorGUI.PrefixLabel(position, label);

            // Small toggle for referenceInScene
            float toggleW = 16f;
            Rect toggleRect = new(position.x, position.y, toggleW, position.height);
            Rect fieldRect = new(position.x + toggleW + 2f, position.y, position.width - toggleW - 2f, position.height);

            refProp.boolValue = EditorGUI.Toggle(toggleRect, new GUIContent("", "Check this to reference a blowup in the scene. Uncheck to enter blowup name manually."), refProp.boolValue);

            if (refProp.boolValue)
            {
                //If switching from textbox to reference, try finding blowup with same name for reference value
                if (wasNameInput)
                {
                    var blwup = GameObject.FindObjectsByType<Blowup>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(b => b.name == nameProp.stringValue).FirstOrDefault();
                    if(blwup != null) blowupProp.objectReferenceValue = blwup;
                }

                //Reference field for blowup
                EditorGUI.PropertyField(fieldRect, blowupProp,new GUIContent());
                //Update blowupName to match
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