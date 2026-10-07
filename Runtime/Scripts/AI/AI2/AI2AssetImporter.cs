#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using System.IO;
using Obj = UnityEngine.Object;

namespace NuGizWrap.AI
{
    using Helper;
    using System;

    [ScriptedImporter(1, "ai2")]
    public class AI2AssetImporter : ScriptedImporter
    {
        public bool test;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            using var br = new BinaryReader(File.OpenRead(ctx.assetPath));

            GameObject rootAIObj = new("AI");
            Transform aiParent = rootAIObj.transform;
            var ai2 = rootAIObj.AddComponent<AI2Manager>();

            int version = br.ReadInt32();
            ai2.version = version;

            int pathCount = br.ReadInt32();
            for (int i = 0; i < pathCount; i++)
            {
                GameObject pathObj = new($"ai_path_{i}");
                pathObj.transform.SetParent(aiParent);
                pathObj.AddComponent<AIPath>().FromBytes(br, version);
            }

            if (version >= 19)
            {
                short unk39Count = br.ReadInt16();
                for (int i = 0; i < unk39Count; i++)
                {
                    byte unk40Count = br.ReadByte();
                    byte[] unk40 = br.ReadBytes(unk40Count);
                }
            }

            if (version >= 4)
            {
                int triggerCount = br.ReadInt32();
                for (int i = 0; i < triggerCount; i++)
                {
                    GameObject trigObj = new($"ai_trigger_{i}");
                    trigObj.transform.SetParent(aiParent);
                    trigObj.AddComponent<Trigger>().FromBytes(br, version);
                }
            }

            Locator[] locators = new Locator[0];
            if (version >= 6)
            {
                int locatorCount = br.ReadInt32();
                locators = new Locator[locatorCount];
                for (int i = 0; i < locatorCount; i++)
                {
                    GameObject locatorObj = new($"ai_locator_{i}");
                    locatorObj.transform.SetParent(aiParent);
                    locators[i] = locatorObj.AddComponent<Locator>();
                    locators[i].FromBytes(br, version);
                }
            }

            if (version >= 18)
            {
                int locatorSetCount = br.ReadInt32();
                for (int i = 0; i < locatorSetCount; i++)
                {
                    GameObject setObj = new($"ai_locator_set_{i}");
                    Transform setTransform = setObj.transform;
                    setTransform.SetParent(aiParent);
                    var set = setObj.AddComponent<LocatorSet>();
                    set.FromBytes(br, locators);
                    foreach (var loc in set.locators) loc.transform.SetParent(setTransform);
                }
            }

            int creatureCount = br.ReadInt32();
            for (int i = 0; i < creatureCount; i++)
            {
                GameObject creatureObj = new($"ai_creature_{i}");
                creatureObj.transform.SetParent(aiParent);
                creatureObj.AddComponent<Creature>().FromBytes(br, version);
            }

            if (version >= 13)
            {
                int obstacleCount = br.ReadInt32();
                for (int i = 0; i < obstacleCount; i++)
                {
                    GameObject obstacleObj = new($"ai_obstacle_{i}");
                    obstacleObj.transform.SetParent(aiParent);
                    obstacleObj.AddComponent<AIObstacle>().FromBytes(br, version);
                }
            }

            if (version >= 7)
            {
                //I swear its reading a byte in the code but int seems to be what works
                //byte unk114Length = br.ReadByte();
                int unk114Length = br.ReadInt32();

                string unk114 = string.Empty;
                if (unk114Length != 0 && unk114Length <= 8) unk114 = br.ReadString(unk114Length).Trim();
                if (unk114 != "LEGO")
                {
                    Debug.Log("unk114 not LEGO: " + unk114);
                    br.ReadInt32(); //padding
                }
            }

            ctx.AddObjectToAsset(rootAIObj.name, rootAIObj);
        }
    }
}
#endif