#if UNITY_EDITOR
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace NuGizWrap.Audio
{
    [ScriptedImporter(1, "cfg")]
    public class CustomConfigAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string fileName = Path.GetFileNameWithoutExtension(ctx.assetPath).ToLower();
            if (fileName.Contains("audio"))
            {
                AudioConfig.Import(File.ReadAllLines(ctx.assetPath), ctx);
            }
            else if (fileName.Contains("music"))
            {

            }
            else
            {

            }
        }
    }
}
#endif