#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace TTModdingKit.Gizmos
{
    using Helper;

    public static class GIZExporter
    {
        public static Type[] SectionTypes = new Type[]
        {
            typeof(GizObstacleSection),
            typeof(GizBuilditSection),
            typeof(GizForceSection), //TCS
            typeof(BlowupSection),
            typeof(GizDigSection), //LIJ1
            typeof(GizmoPickupSection),
            typeof(ShardSection), //LB1
            typeof(SignalSection), //LB1
            typeof(GrappleSection), //LB1/LIJ1
            typeof(TightRopeSection), //LB1
            typeof(LedgeSection), //LB1/LIJ1
            typeof(LeverSection),
            typeof(SpinnerSection),
            typeof(TechnoSection), //LB1/LIJ1
            typeof(SecurityDoorSection), //LB1/LIJ1
            typeof(AttractoSection), //LB1
            typeof(MiniCutSection),
            typeof(TubeSection),
            typeof(ZipUpSection),
            typeof(WhipperSection), //LIJ1
            typeof(GizTurretSection),
            typeof(BombGeneratorSection), //TCS/LB1
            typeof(PanelSection), //TCS/LB1
            typeof(HatMachineSection), //TCS
            typeof(PlugSection), //LIJ1/LB1
            typeof(PushBlocksSection),
            typeof(TorpMachineSection), //TCS/LB1
            typeof(ShadowEditorSection),
            typeof(TeleportSection), //LIJ1
            typeof(PuzzleSection), //LIJ1
            typeof(GizFlockSection), //LIJ1/LB1
        };

        [MenuItem("TT Modding/Export/File/GIZ")]
        static void Export() 
        {
            string path = EditorUtility.SaveFilePanel("Export GIZ File", TTUnityProject.GetDefaultFileExplorerPath(), "levelgiz", "giz");
            if (string.IsNullOrEmpty(path) || !Directory.Exists(Path.GetDirectoryName(path))) return;

            Export(path, true);
        }

        public static void Export(string path, bool notify=false)
        {
            try
            {
                byte[] bytes = GetBytes();
                if (bytes.Length == 0) return;
                File.WriteAllBytes(path, bytes);
            }
            catch(IOException ioe)
            {
                Error(ioe.Message);
                return;
            }

            if(notify) EditorUtility.DisplayDialog("Gizmos Exported!", $"Successfully exported Gizmos to '{path}'", "OK");
        }

        public static byte[] GetBytes()
        {
            EditorUtility.DisplayProgressBar("Exporting", $"Exporting Gizmos...", 0);

            if (!ValidationCheck())
            {
                EditorUtility.ClearProgressBar();
                return new byte[0];
            }

            List<byte> bytes = new();
            bytes.AddInt(1);

            int sectionsCount = SectionTypes.Length;

            for (int i = 0; i < sectionsCount; i++)
            {
                var section = Object.FindFirstObjectByType(SectionTypes[i], FindObjectsInactive.Exclude) as GizmoSection;
                if (section == null || (section is IGameCompatible s && !s.IsGameCompatible())) continue;
                EditorUtility.DisplayProgressBar("Exporting", $"Exporting Gizmos ({section.ID})...", i / (float)sectionsCount);
                bytes.AddInt(section.ID.Length);
                bytes.AddString(section.ID);
                byte[] sectionBytes = section.ToBytes();
                bytes.AddInt(sectionBytes.Length);
                bytes.AddRange(sectionBytes);
            }

            bytes.AddInt(0);
            EditorUtility.ClearProgressBar();

            return bytes.ToArray();
        }

        public static bool ValidationCheck()
        {
            //Check game compatibility
            var sections = Object.FindObjectsByType<GizmoSection>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach(var section in sections)
            {
                if (section is not IGameCompatible s) continue;
                if (!s.IsGameCompatible())
                {
                    bool cancel = !EditorUtility.DisplayDialog($"{section.ID} Incompatible", $"Gizmo Section '{section.ID}' is not compatible with {TTUnityProject.Game}. Continue without exporting this section, or cancel export?", "Continue", "Cancel");
                    if (cancel) return false;
                }
            }

            return true;
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif