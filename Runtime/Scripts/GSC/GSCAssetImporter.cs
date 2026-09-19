#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    [ScriptedImporter(1, "gsc")]
    public class GSCAssetImporter : ScriptedImporter
    {
        public bool testBool;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            GameObject gameSceneObj = new("Game Scene");
            var gameScene = gameSceneObj.AddComponent<NuGameScene>();

            ctx.AddObjectToAsset(gameSceneObj.name, gameSceneObj);

            gameScene.Load(new MemoryStream(File.ReadAllBytes(ctx.assetPath)));

            int txtrCount = gameScene.tempTextures.Length;
            for (int i = 0; i < txtrCount; i++)
            {
                var txtr = gameScene.tempTextures[i];
                if(txtr != null) ctx.AddObjectToAsset(txtr.name, txtr);
            }

            foreach(var mat in MaterialBlock.Instance.materials.Values)
            {
                if(mat != null) ctx.AddObjectToAsset(mat.name, mat);
            }
        }
    }
}
#endif