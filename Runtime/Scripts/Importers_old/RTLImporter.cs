#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NuGizWrap.Lighting
{
    using Helper;

    public static class RTLImporter
    {
        [MenuItem("Nu Giz Wrap/Import/File/RTL")]
        static void Import()
        {
            string path = EditorUtility.OpenFilePanel("Import RTL File", TTUnityProject.GetDefaultFileExplorerPath(), "rtl");
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            Import(path, true);
        }

        public static void Import(string path, bool notify)
        {
            BinaryReader br = null;
            try
            {
                br = new(File.OpenRead(path));

                GameObject lightingObj = GameObject.Find("Lighting");
                if (lightingObj == null) lightingObj = new GameObject("Lighting");
                Transform lightParent = lightingObj.transform;
                if (!lightingObj.TryGetComponent(out LightManager lm)) lm = lightingObj.AddComponent<LightManager>();

                int version = br.ReadInt32();
                lm.version = version;
                int lightCount = version switch { 1 => 0, 2 or 3 => 0x40, _ => 0x80 };
                int unkLightCount = version switch { 1 => 0, 2 => 1, _ => 0x20 };

                bool deleteInvalidLights = TTUnityProject.Prefs.lighting.deleteInvalidLights;
                for(int i=0; i<lightCount; i++)
                {
                    var light = new GameObject($"rtl_light_{i}").AddComponent<RTLight>();
                    var lightTransform = light.transform;
                    lightTransform.SetParent(lightParent);
                    lightTransform.position = br.ReadVector3();
                    lightTransform.eulerAngles = br.ReadVector3();

                    light.color = br.ReadColor();
                    light.highColor = br.ReadColor();
                    light.flickerColor = br.ReadColor();

                    lightTransform.localScale = Vector3.one * br.ReadSingle();
                    light.falloff = br.ReadSingle();

                    light.flickerHighTime = br.ReadSingle();
                    light.flickerLowTime = br.ReadSingle();
                    light.flickerRandomHighTime = br.ReadSingle();
                    light.flickerRandomLowTime = br.ReadSingle();
                    light.flickerTimer = br.ReadSingle();

                    byte typeVal = br.ReadByte();
                    light.type = (RTLight.Type)typeVal;
                    if (!Enum.IsDefined(typeof(RTLight.Type), (int)typeVal))
                    {
                        light.type = RTLight.Type.Invalid;
                        Debug.LogWarning("Undefined Light Type: " + typeVal);
                    }

                    //Delete, or continue reading
                    if (deleteInvalidLights && light.type == RTLight.Type.Invalid)
                    {
                        light.gameObject.DelayDestroy();
                        br.BaseStream.Seek(51, SeekOrigin.Current);
                    }
                    else
                    {
                        byte optionVal = br.ReadByte();
                        light.option = (RTLight.Option)optionVal;
                        if (!Enum.IsDefined(typeof(RTLight.Option), (int)optionVal)) Debug.LogWarning("Undefined Light Option: " + optionVal);

                        light.unk1 = br.ReadInt16();
                        light.unk2 = br.ReadInt16();
                        light.unk3 = br.ReadInt16();
                        light.unk4 = br.ReadInt32();
                        light.unk5 = br.ReadInt16();
                        light.unk6 = br.ReadInt16();
                        light.unk7 = br.ReadInt32();
                        light.multiplier = br.ReadSingle();
                        light.unk8 = br.ReadInt32();
                        light.unk9 = br.ReadBytes(24);
                    }
                }

                for(int i=0; i<unkLightCount; i++)
                {
                    var unkLight = new GameObject($"rtl_unk_light_{i}").AddComponent<UnknownLight>();
                    unkLight.transform.SetParent(lightParent);

                    unkLight.unk1 = br.ReadInt32();
                    unkLight.unk2 = br.ReadInt32();
                    unkLight.unk3 = br.ReadInt32();
                    unkLight.unk4 = br.ReadInt32();
                    unkLight.unk5 = br.ReadInt32();
                    unkLight.unk6 = br.ReadInt32();
                    unkLight.unk7 = br.ReadInt32();
                    unkLight.unk8 = br.ReadVector3();
                    unkLight.unk9 = br.ReadInt32();
                    unkLight.unk10 = br.ReadInt32();
                    unkLight.unk11 = br.ReadInt32();
                    unkLight.unk12 = br.ReadInt32();
                    unkLight.unk13 = br.ReadBytes(20);
                }

                br.Close();
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
                br?.Close();
                return;
            }

            if (notify) EditorUtility.DisplayDialog("RTL Imported!", $"Successfully imported RTL from '{path}'", "OK");
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif