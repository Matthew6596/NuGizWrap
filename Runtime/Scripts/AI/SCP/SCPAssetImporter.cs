#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace NuGizWrap.AI.Scripting
{
    [ScriptedImporter(1, "scp")]
    public class SCPAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var script = ScriptableObject.CreateInstance<AIScript>();
            script.text = File.ReadAllText(ctx.assetPath);
            script.name = Path.GetFileNameWithoutExtension(ctx.assetPath);
            ctx.AddObjectToAsset(script.name, script);
        }
    }
}
#endif