#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Obj = UnityEngine.Object;

namespace NuGizWrap.Lighting
{
    using Helper;
    using System;

    public static class RTLExporter
    {
        [MenuItem("Nu Giz Wrap/Export/File/RTL")]
        static void Export()
        {
            string path = EditorUtility.SaveFilePanel("Export RTL File", TTUnityProject.GetDefaultFileExplorerPath(), "levelrtl", "rtl");
            if (string.IsNullOrEmpty(path) || !Directory.Exists(Path.GetDirectoryName(path))) return;

            Export(path, true);
        }

        public static void Export(string filepath, bool notify = false)
        {
            try
            {
                BinaryWriter bw = new(File.OpenWrite(filepath));

                var lm = Obj.FindFirstObjectByType<LightManager>(FindObjectsInactive.Exclude);
                int version = lm.version;
                bw.Write(version);

                var lights = Obj.FindObjectsByType<RTLight>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
                var unkLights = Obj.FindObjectsByType<UnknownLight>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);

                int lightCount = version switch { 1 => 0, 2 or 3 => 0x40, _ => 0x80 };
                int unkLightCount = version switch { 1 => 0, 2 => 1, _ => 0x20 };

                int sceneLightCount = lights.Length;
                int sceneUnkLightCount = unkLights.Length;

                if (sceneLightCount > lightCount) Debug.LogWarning($"There are {sceneLightCount} RT-Lights in the scene, however because the LightManager's version is {version}, the max amount of RT-Lights is {lightCount}");
                if (sceneUnkLightCount > unkLightCount) Debug.LogWarning($"There are {sceneUnkLightCount} Unknown-Light-Objs in the scene, however because the LightManager's version is {version}, the max amount of Unknown-Light-Objs is {unkLightCount}");

                for(int i=0; i<lightCount; i++)
                {
                    if (i > sceneLightCount)
                    {
                        bw.Write(new byte[140]);
                        continue;
                    }

                    var light = lights[i];
                    var lightTransform = light.transform;
                    bw.Write(lightTransform.position);
                    bw.Write(lightTransform.eulerAngles);

                    bw.WriteColor(light.color);
                    bw.WriteColor(light.highColor);
                    bw.WriteColor(light.flickerColor);

                    bw.Write(lightTransform.localScale.x);
                    bw.Write(light.falloff);

                    bw.Write(light.flickerHighTime);
                    bw.Write(light.flickerLowTime);
                    bw.Write(light.flickerRandomHighTime);
                    bw.Write(light.flickerRandomLowTime);
                    bw.Write(light.flickerTimer);

                    bw.Write((byte)light.type);
                    bw.Write((byte)light.option);
                    bw.Write(light.unk1);
                    bw.Write(light.unk2);
                    bw.Write(light.unk3);
                    bw.Write(light.unk4);
                    bw.Write(light.unk5);
                    bw.Write(light.unk6);
                    bw.Write(light.unk7);
                    bw.Write(light.multiplier);
                    bw.Write(light.unk8);
                    bw.Write(light.unk9);
                }

                for(int i=0; i<unkLightCount; i++)
                {
                    if (i > sceneLightCount)
                    {
                        bw.Write(new byte[76]);
                        continue;
                    }

                    var unkLight = unkLights[i];
                    bw.Write(unkLight.unk1);
                    bw.Write(unkLight.unk2);
                    bw.Write(unkLight.unk3);
                    bw.Write(unkLight.unk4);
                    bw.Write(unkLight.unk5);
                    bw.Write(unkLight.unk6);
                    bw.Write(unkLight.unk7);
                    bw.Write(unkLight.unk8);
                    bw.Write(unkLight.unk9);
                    bw.Write(unkLight.unk10);
                    bw.Write(unkLight.unk11);
                    bw.Write(unkLight.unk12);
                    bw.Write(unkLight.unk13);
                }

                if (notify) EditorUtility.DisplayDialog("RTL Exported!", $"Successfully exported RTL to '{filepath}'", "OK");
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
            }
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif