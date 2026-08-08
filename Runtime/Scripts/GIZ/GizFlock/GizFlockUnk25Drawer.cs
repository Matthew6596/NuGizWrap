#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    using Helper;
    [CustomPropertyDrawer(typeof(GizFlock.Unk25))]
    public class GizFlockUnk25Drawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            int unk1Val = property.FindPropertyRelative("unk1").intValue;
            int version = GizFlockSection.Instance.version;

            float height = EditorGUIUtility.singleLineHeight * 4;
            if (version >= 2) height += EditorGUIUtility.singleLineHeight*2;
            if (unk1Val == 0 || unk1Val == 1) height += EditorGUIUtility.singleLineHeight;

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            int version = GizFlockSection.Instance.version;

            float height = EditorGUIUtility.singleLineHeight;

            string lbl = label.text.Replace("Element", "Unknown25");
            if (!(property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, height), property.isExpanded, new GUIContent(lbl), true))) return;

            // Draw label, returns remaining rect
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Small toggle for referenceInScene
            Rect rect1 = new(position.x, position.y + height, position.width, height);
            Rect rect2 = new(position.x, position.y + height * 2, position.width, height);
            Rect rect3 = new(position.x, position.y + height * 3, position.width, height);
            Rect rect4 = new(position.x, position.y + height * 4, position.width, height);
            Rect rect5 = new(position.x, position.y + height * 5, position.width, height);

            var unk1Prop = property.FindPropertyRelative("unk1");
            EditorGUI.PropertyField(rect1, unk1Prop);
            if (version >= 2)
            {
                property.Prop("unk2", rect2);
                property.Prop("unk3", rect3);
            }
            else
            {
                rect4 = rect2;
                rect5 = rect3;
            }

            property.Prop("unk4", rect4);
            if (unk1Prop.intValue == 0) property.Prop("unk5", rect5);
            else if (unk1Prop.intValue == 1) property.Prop("unk6", rect5);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
#endif