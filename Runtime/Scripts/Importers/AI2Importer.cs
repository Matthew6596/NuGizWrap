#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NuGizWrap.AI
{
    using Helper;

    public static class AI2Importer
    {
        [MenuItem("Nu Giz Wrap/Import/File/AI2")]
        static void Import()
        {
            string path = EditorUtility.OpenFilePanel("Import AI2 File", TTUnityProject.GetDefaultFileExplorerPath(), "ai2");
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            Import(path, true);
        }

        public static void Import(string path, bool notify)
        {
            BinaryReader br = null;
            try
            {
                br = new(File.OpenRead(path));

                GameObject aiObj = GameObject.Find("AI");
                if (aiObj == null) aiObj = new GameObject("AI");
                Transform aiParent = aiObj.transform;

                int version = br.ReadInt32();
                int pathCount = br.ReadInt32();
                for(int i=0; i<pathCount; i++)
                {
                    GameObject pathObj = new("ai_path");
                    pathObj.transform.SetParent(aiParent);
                    pathObj.AddComponent<AIPath>().FromBytes(br, version);
                }

                if (version >= 19)
                {
                    short unk39Count = br.ReadInt16();
                    for(int i=0; i<unk39Count; i++)
                    {
                        byte unk40Count = br.ReadByte();
                        byte[] unk40 = br.ReadBytes(unk40Count);
                    }
                }

                if (version >= 4)
                {
                    int triggerCount = br.ReadInt32();
                    for(int i=0; i<triggerCount; i++)
                    {
                        GameObject trigObj = new("ai_trigger");
                        trigObj.transform.SetParent(aiParent);
                        trigObj.AddComponent<Trigger>().FromBytes(br, version);
                    }
                }

                Locator[] locators = new Locator[0];
                if (version >= 6)
                {
                    int locatorCount = br.ReadInt32();
                    locators = new Locator[locatorCount];
                    for(int i=0; i<locatorCount; i++)
                    {
                        GameObject locatorObj = new("ai_locator");
                        locatorObj.transform.SetParent(aiParent);
                        locators[i] = locatorObj.AddComponent<Locator>();
                        locators[i].FromBytes(br, version);
                    }
                }

                if (version >= 18)
                {
                    int locatorSetCount = br.ReadInt32();
                    for(int i=0; i<locatorSetCount; i++)
                    {
                        GameObject setObj = new("ai_locator_set");
                        setObj.transform.SetParent(aiParent);
                        setObj.AddComponent<LocatorSet>().FromBytes(br, locators);
                    }
                }

                int creatureCount = br.ReadInt32();
                for(int i=0; i<creatureCount; i++)
                {
                    GameObject creatureObj = new("ai_creature");
                    creatureObj.transform.SetParent(aiParent);
                    creatureObj.AddComponent<Creature>().FromBytes(br, version);
                }

                if (version >= 13)
                {
                    int obstacleCount = br.ReadInt32();
                    for(int i=0; i<obstacleCount; i++)
                    {
                        GameObject obstacleObj = new("ai_obstacle");
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

                br.Close();
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
                br?.Close();
                return;
            }

            if (notify) EditorUtility.DisplayDialog("AI2 Imported!", $"Successfully imported AI2 from '{path}'", "OK");
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif