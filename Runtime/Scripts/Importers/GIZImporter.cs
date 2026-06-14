#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TTModdingKit.Gizmos
{
    using Helper;

    public static class GIZImporter
    {
        public static Type[] SectionTypes => GIZExporter.SectionTypes;

        [MenuItem("TT Modding/Import/File/GIZ")]
        static void Import()
        {
            string dir = TTUnityProject.Prefs.defaultFileDirectoryToCurrentGame ? Path.GetDirectoryName(TTUnityProject.GetGamePath()) : "";
            string path = EditorUtility.OpenFilePanel("Import GIZ File", dir, "giz");
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            Import(path, true);
        }

        public static void Import(string path, bool notify)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                if (bytes.Length == 0) return;
                LoadBytes(bytes);
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
                return;
            }

            if (notify) EditorUtility.DisplayDialog("Gizmos Imported!", $"Successfully imported Gizmos from '{path}'", "OK");
        }

        public static int LoadBytes(byte[] bytes, int index=0)
        {
            int magic = bytes.ReadInt(ref index);
            if (magic != 1) return -1;

            GameObject gizParentObj = GameObject.Find("Gizmos");
            Transform gizParent = gizParentObj==null ? new GameObject("Gizmos").transform : gizParentObj.transform;

            //Create all gizmo sections
            var sections = CreateSections(gizParent);

            string name;
            while (index < bytes.Length-4 && (name = bytes.ReadString32(ref index)).Length > 0) //Continue reading gizmo section IDs
            {
                //Debug.Log("INDEX: " + index);
                //Find gizmo section by ID
                //Debug.Log("finding section: " + name);
                var section = sections.Where(s => s.ID == name).FirstOrDefault();
                int len = bytes.ReadInt(ref index);

                //If no gizmo section was found or section is empty, skip
                if (section == null || len == 0)
                {
                    index += len;
                    //Debug.Log("Section NOT supported, skipped to: " + index);
                    continue;
                }

                //Debug.Log($"LOADING section at {index}: " + section.ID);
                //Load gizmo section
                section.FromBytes(bytes, ref index);
                //Debug.Log("INDEX after read: " + index);
            }

            //Get project prefs for clean up
            var projPrefs = TTUnityProject.Instance.prefs;
            bool destroyEmptySections = !projPrefs.generateEmptyGizmoSections;
            bool destroyUncompatible = projPrefs.onlyGenerateCompatibleGizmoSections;

            //Loop gizmo types
            foreach (var sectionType in SectionTypes)
            {
                //Find the section
                var section = GameObject.FindFirstObjectByType(sectionType) as GizmoSection;
                EditorApplication.delayCall += () =>
                {
                    //Maybe delete section based on project prefs
                    bool hasNoChildren = section != null && section.gameObject != null && section.transform.childCount == 0;
                    bool notCompatible = !section.IsGameCompatible(TTUnityProject.Game);
                    bool sectionNotNull = section != null && section.gameObject != null;

                    if ((hasNoChildren && destroyEmptySections) || (notCompatible && destroyUncompatible) && sectionNotNull)
                        Object.DestroyImmediate(section.gameObject);
                };
            }

            return index;
        }

        private static IEnumerable<GizmoSection> CreateSections(Transform gizParent)
        {
            List<GizmoSection> sections = new();
            foreach(var sectionType in SectionTypes)
            {
                //Debug.Log("trying section: " + sectionType.Name);
                //Try finding section in scene
                var section = GameObject.FindFirstObjectByType(sectionType) as GizmoSection;
                if (section == null)
                {
                    //Create section object
                    GameObject sectionObj = new();
                    sectionObj.transform.SetParent(gizParent);

                    //Add section instance
                    section = sectionObj.AddComponent(sectionType) as GizmoSection;
                    sectionObj.name = $"{section.ID} Section";
                }
                sections.Add(section);
            }
            return sections;
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif