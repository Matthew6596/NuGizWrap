#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TTModdingKit.Gizmos
{
    using Helper;
    [CustomPropertyDrawer(typeof(Spinner.SpecialObject))]
    public class SpinnerSpecialObjectDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            int specObjVers = property.serializedObject.FindProperty("specialObjectVersion").intValue;

            float height = EditorGUIUtility.singleLineHeight * 4;
            if (specObjVers >= 2) height += EditorGUIUtility.singleLineHeight;

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            int specObjVers = property.serializedObject.FindProperty("specialObjectVersion").intValue;

            float height = EditorGUIUtility.singleLineHeight;

            string lbl = label.text.Replace("Element", "Special Object");
            if (!(property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, height), property.isExpanded, new GUIContent(lbl), true))) return;

            // Draw label, returns remaining rect
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Small toggle for referenceInScene
            Rect specRect = new(position.x, position.y + height, position.width, height);
            Rect unk1Rect = new(position.x, position.y + height * 2, position.width, height);
            Rect animRect = new(position.x, position.y + height * 3, position.width, height);
            Rect unk2Rect = new(position.x, position.y + height * 4, position.width, height);

            property.Prop("specialObject", specRect);
            property.Prop("unknown1", unk1Rect);
            property.Prop("animationTime", animRect);

            if (specObjVers >= 2) property.Prop("unknown2", unk2Rect);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
#endif