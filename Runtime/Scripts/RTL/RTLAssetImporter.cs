#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using System.IO;
using Obj = UnityEngine.Object;

namespace NuGizWrap.Lighting
{
    using Helper;
    using System;

    [ScriptedImporter(1, "rtl")]
    public class RTLAssetImporter : ScriptedImporter
    {
        [Tooltip("Generates all RTLights, even those with Type 'Invalid'.")]
        public bool generateUnusedLights = true;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            using var br = new BinaryReader(File.OpenRead(ctx.assetPath));

            GameObject rootLightObj = new("Lighting");
            Transform lightParent = rootLightObj.transform;
            var lm = rootLightObj.AddComponent<LightManager>();

            int version = br.ReadInt32();
            lm.version = version;
            int lightCount = version switch { 1 => 0, 2 or 3 => 0x40, _ => 0x80 };
            int unkLightCount = version switch { 1 => 0, 2 => 1, _ => 0x20 };

            for (int i = 0; i < lightCount; i++)
            {
                var lightObj = new GameObject($"rtl_light_{i}");
                var light = lightObj.AddComponent<RTLight>();
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

                //Skip, or continue reading
                if (!generateUnusedLights && light.type == RTLight.Type.Invalid)
                {
                    DestroyImmediate(lightObj);
                    br.BaseStream.Seek(51, SeekOrigin.Current);
                    continue;
                }

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

                lightObj.transform.SetParent(lightParent);
            }

            for (int i = 0; i < unkLightCount; i++)
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

            ctx.AddObjectToAsset(rootLightObj.name, rootLightObj);

            br.Close();
        }
    }
}
#endif