#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TTModdingKit.Gizmos
{
    [InitializeOnLoad]
    public static class EditorGIZIconManager
    {
        private static bool mouseDown = false;
        static EditorGIZIconManager()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItem;
        }

        private static void OnHierarchyItem(int instanceID, Rect selectionRect)
        {
            //Shenanigans with bg color
            if (Event.current.type == EventType.MouseUp) mouseDown = false;
            if (Event.current.type == EventType.MouseDown) mouseDown = true;

            //Get gameObj
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            //Get gameObj icon
            Texture2D icon = EditorGUIUtility.GetIconForObject(go);
            if (icon == null) return;

            //Icon in hierarchy
            var iconRect = new Rect(selectionRect.x - 1, selectionRect.y, selectionRect.height, selectionRect.height);

            Color bgColor = GetRowColor(instanceID, selectionRect);

            //Clear default icon, draw custom
            EditorGUI.DrawRect(iconRect, bgColor);
            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
        }
        private static Color GetRowColor(int instanceID, Rect rowRect)
        {
            //Get current state
            bool isHovered = rowRect.Contains(Event.current.mousePosition);
            bool isSelected = Selection.Contains(instanceID);

            //Shenanigans (not 100% accurate, like when dragging)
            if(Event.current.type != EventType.Repaint || mouseDown)
            {
                if (isSelected && !isHovered) isSelected = false;
                else if(!isSelected && isHovered) isSelected = true;
            }

            //Get color to match state
            if (EditorGUIUtility.isProSkin)
            {
                if (isSelected) return new Color(0.173f, 0.365f, 0.529f);
                if (isHovered) return new Color(0.271f, 0.271f, 0.271f);
                return new Color(0.220f, 0.220f, 0.220f);
            }
            else
            {
                if (isSelected) return new Color(0.243f, 0.490f, 0.906f);
                if (isHovered) return new Color(0.698f, 0.698f, 0.698f);
                return new Color(0.796f, 0.796f, 0.796f);
            }
        }

    }
}
#endif