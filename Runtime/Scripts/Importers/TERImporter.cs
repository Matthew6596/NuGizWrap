#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TTModdingKit.Terrain
{
    using Helper;

    public static class TERImporter
    {
        [MenuItem("TT Modding/Import/File/TER")]
        static void Import()
        {
            string path = EditorUtility.OpenFilePanel("Import TER File", TTUnityProject.GetDefaultFileExplorerPath(), "ter");
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            Import(path, true);
        }

        public static void Import(string path, bool notify)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                if (bytes.Length == 0) return;
                LoadBytes(bytes);
            }
            catch (IOException ioe)
            {
                Error(ioe.Message);
                return;
            }

            if (notify) EditorUtility.DisplayDialog("Terrain Imported!", $"Successfully imported Terrain from '{path}'", "OK");
        }

        /// <summary>
        /// Documentation: https://docs.google.com/document/d/1RXuAUQNmjFDr9oPZbHN-2UgKCik0rLudYj0bE-GejzM/edit?tab=t.0#heading=h.hnqbynefesup
        /// </summary>
        public static int LoadBytes(byte[] bytes, int index=0)
        {
            GameObject terParentObj = GameObject.Find("Terrain");
            Transform terParent = terParentObj == null ? new GameObject("Terrain").transform : terParentObj.transform;

            int dataLen = bytes.ReadInt(ref index) * 2;
            int globalPtr = index;

            index = dataLen;

            short objCount = bytes.ReadShort(ref index);

            //disobeying docs, ignoring first 4 bytes of terrain object, still assuming object is 0x34 bytes
            short unk1 = bytes.ReadShort(ref index);

            for (int i = 0; i < objCount; i++)
            {
                //Read terrain object
                int globalPtrOffset = bytes.ReadInt(ref index) * 2;

                Vector3 position = bytes.ReadVector3(ref index);

                short flag = bytes.ReadShort(ref index);
                short terrainPlatformOffset = bytes.ReadShort(ref index);

                //Skipped in docs
                index += 24;

                int unk2 = bytes.ReadInt(ref index);
                int unk3 = bytes.ReadInt(ref index); //was short in docs, consequence of disobeying docs earlier

                switch (flag)
                {
                    case 0: LoadTerrainGroup(bytes, globalPtr, terParent); break;
                    //case 1: LoadTerrainPlatform(bytes, ref index, terrainPlatformOffset); LoadTerrainGroup(bytes, globalPtr, terParent); break;
                    case 2: LoadInvisWall(bytes, globalPtr, terParent); break;
                    default: Debug.LogError($"Unsupported terrain flag at {index - 40}: {flag}"); break;
                }

                globalPtr += globalPtrOffset;
            }

            return index;
        }

        private static void LoadTerrainGroup(byte[] bytes, int index, Transform terrainParent)
        {
            while(bytes.ReadShort(ref index) > -1)
            {
                GameObject terMeshObj = new("terrain_mesh");
                terMeshObj.transform.SetParent(terrainParent);
                var terMesh = terMeshObj.AddComponent<TerrainMesh>();
                terMesh.faces = new();

                int faceCount = bytes.ReadShort(ref index);

                float blockMinX = bytes.ReadFloat(ref index);
                float blockMaxX = bytes.ReadFloat(ref index);
                float blockMinZ = bytes.ReadFloat(ref index);
                float blockMaxZ = bytes.ReadFloat(ref index);

                for(int i=0; i<faceCount; i++)
                {
                    TerrainMesh.Face face = new();

                    Vector3 min, max;
                    min.x = bytes.ReadFloat(ref index);
                    max.x = bytes.ReadFloat(ref index);
                    min.y = bytes.ReadFloat(ref index);
                    max.y = bytes.ReadFloat(ref index);
                    min.z = bytes.ReadFloat(ref index);
                    max.z = bytes.ReadFloat(ref index);
                    face.min = min;
                    face.max = max;

                    face.p1 = bytes.ReadVector3(ref index);
                    face.p2 = bytes.ReadVector3(ref index);
                    face.p3 = bytes.ReadVector3(ref index);
                    face.p4 = bytes.ReadVector3(ref index);
                    face.norm1 = bytes.ReadVector3(ref index);
                    face.norm2 = bytes.ReadVector3(ref index);

                    face.property1 = bytes.ReadByte(ref index);
                    face.property2 = bytes.ReadByte(ref index);

                    face.flag1 = bytes.ReadByte(ref index);
                    face.flag2 = bytes.ReadByte(ref index);

                    terMesh.faces.Add(face);
                }
            }
        }

        private static void LoadInvisWall(byte[] bytes, int index, Transform terParent)
        {
            int unk1 = bytes.ReadInt(ref index);
            int listSize = bytes.ReadInt(ref index);

            GameObject infWallObj = new("infinite_wall");
            infWallObj.transform.SetParent(terParent);
            var infWall = infWallObj.AddComponent<InfiniteWall>();
            infWall.points = new();
            for (int i = 0; i < listSize; i++)
            {
                Vector3 p = bytes.ReadVector3(ref index);
                p.y = 0;
                infWall.points.Add(p);
            }
        }

        private static void LoadTerrainPlatform(byte[] bytes, ref int index, short terrainPlatformOffset)
        {

        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);
    }
}
#endif