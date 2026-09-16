#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    [ScriptedImporter(1, "gsc")]
    public class GSCAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {

        }
    }
}
#endif