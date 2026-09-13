#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using static GSNHBlock;
    using Helper;

    public class DisplayBlock : GscBlock
    {
        public static DisplayBlock Instance { get; private set; }

        public DisplayCommand[] displayCommands;

        public override void Load(BinaryReader br)
        {
            Instance = this;

            int gscFilePathPtr = br.ReadInt32();

            int displayCmdCount = br.ReadInt32();
            int displayCmdListPtr = br.ReadInt32();

            int unkFdnsPtr = br.ReadInt32();

            int gameModelsCount = br.ReadInt32();
            long gameModelsPos = br.BaseStream.Position;
            int gameModelsPtr = br.ReadInt32();
            gameModelsPos += gameModelsPtr;

            int modelSizeListPtr = br.ReadInt32();
            int unk2IndListPtr = br.ReadInt32();
            int cmdIndexListPtr = br.ReadInt32();
            int materialIndexListPtr = br.ReadInt32();
            int unkPtr1 = br.ReadInt32();
            int unkPtr2 = br.ReadInt32();
            int unkPtr3 = br.ReadInt32();
            int unkPtr4 = br.ReadInt32();
            int unk1 = br.ReadInt32();
            int unk2 = br.ReadInt32();
            int unkPtr5 = br.ReadInt32();
            int bndsPtr = br.ReadInt32();
            int unkPtr6 = br.ReadInt32();

            int materialCount = br.ReadInt32();
            int materialPtrsListPtr = br.ReadInt32();
            long unkDispObjPtrsListAddr = br.ReadPtr();
            int unkPtr8 = br.ReadInt32();
            int unkPtr9 = br.ReadInt32();

            int staticObjCount = br.ReadInt32();
            long staticObjsAddr = br.ReadPtr();
            int unkCount1 = br.ReadInt32();
            int specialObjCount = br.ReadInt32();
            long specialObjsAddr = br.ReadPtr();
            int unk3 = br.ReadInt32();
            int gsnhPtr = br.ReadInt32();
            int unkPtr10 = br.ReadInt32();
            int unkPtr11 = br.ReadInt32();
            int unkCount2 = br.ReadInt32();
            int unkPtr12 = br.ReadInt32();
            int unkPtr13 = br.ReadInt32();
            int unk4 = br.ReadInt32();
            int unk5 = br.ReadInt32();

            return;
            MaterialBlock.NuMaterial material = default;
            Matrix4x4 matrix = Matrix4x4.identity;

            //Read display commands
            displayCommands = new DisplayCommand[displayCmdCount];
            for(int i=0; i<displayCmdCount; i++)
            {
                displayCommands[i] = ReadDisplayCommand(br);
                RunDisplayCommand(displayCommands[i], br, br.BaseStream.Position, ref material, ref matrix, transform, $"cmd_{i}");
            }

            //MaterialBlock.NuMaterial material = default;
            //Matrix4x4 matrix = Matrix4x4.identity;
            //return;

            //Read Unknown Display Objects
            GameObject unkDisplayObjectsParent = new("Unknown Display Objects");
            unkDisplayObjectsParent.transform.SetParent(transform);
            long dispCmdListEnd = br.GoToAddr(unkDispObjPtrsListAddr);
            for(int i=0; i<materialCount; i++)
            {
                GameObject unkDispObj = new($"unknown_display_object_{i}");
                unkDispObj.transform.SetParent(unkDisplayObjectsParent.transform);

                long unkDispObjAddr = br.ReadPtr();
                long pos = br.GoToAddr(unkDispObjAddr);

                int subSec1Ptr = br.ReadInt32();
                int _unk1 = br.ReadInt32();
                int subSec2Ptr = br.ReadInt32();

                long startDispCmdAddr = br.ReadPtr();

                int subSec5Ptr = br.ReadInt32();
                int _unkPtr1 = br.ReadInt32();

                long endDispCmdAddr = br.ReadPtr();

                //Other unknown values

                //Running display commands
                int meshCount = 0;
                br.BaseStream.Position = startDispCmdAddr;
                while(br.BaseStream.Position < endDispCmdAddr)
                {
                    var cmd = ReadDisplayCommand(br);
                    //if (cmd.flags == DisplayCommand.Flags.Skip) continue;
                    if (cmd.flags == DisplayCommand.Flags.End) break;

                    RunDisplayCommand(cmd, br, br.BaseStream.Position, ref material, ref matrix, unkDispObj.transform, $"mesh_{meshCount}");
                    if (cmd.type == DisplayCommand.Type.GEOMCALL) meshCount++;
                }

                br.BaseStream.Position = pos;
            }

            //Read Static Objects
            GameObject staticObjectsParent = new("Static Objects");
            staticObjectsParent.transform.SetParent(transform);
            long unkDispObjListEnd = br.GoToAddr(staticObjsAddr);
            for(int i=0; i<staticObjCount; i++)
            {
                GameObject staticObj = new($"static_obj_{i}");
                staticObj.transform.SetParent(staticObjectsParent.transform);

                int displayOrder = br.ReadInt32();
                long startCmdAddr = br.ReadPtr();
                long staticObjAddr = br.GoToAddr(startCmdAddr);

                int meshCount = 0;
                for(int j=0; j<999; j++)
                {
                    var cmd = ReadDisplayCommand(br);
                    //if (cmd.flags == DisplayCommand.Flags.Skip) continue;
                    if (cmd.flags == DisplayCommand.Flags.End) break;

                    RunDisplayCommand(cmd, br, br.BaseStream.Position, ref material, ref matrix, staticObj.transform, $"mesh_{meshCount}");
                    if (cmd.type == DisplayCommand.Type.GEOMCALL) meshCount++;
                }

                br.BaseStream.Position = staticObjAddr;

                int staticUnkPtr1 = br.ReadInt32();
                int staticUnkPtr2 = br.ReadInt32();
                int staticUnkPtr3 = br.ReadInt32();
                int staticUnk1 = br.ReadInt32();
                int staticUnkPtr4 = br.ReadInt32();
                int staticUnkPtr5 = br.ReadInt32();
                short staticUnk2 = br.ReadInt16();
                short staticUnk3 = br.ReadInt16();
            }

            //Read Special Objects
            GameObject specialObjectsParent = new("Special Objects");
            specialObjectsParent.transform.SetParent(transform);
            long staticObjsListEnd = br.GoToAddr(specialObjsAddr);
            for(int i=0; i<specialObjCount; i++)
            {
                SpecialObject specObj = new GameObject($"special_obj_{i}").AddComponent<SpecialObject>();
                specObj.transform.SetParent(specialObjectsParent.transform);

                Matrix4x4 specObjMatrix = br.ReadM4x4();
                specObj.localIABLObject = IABLBlock.IABLObject.FromBytes(br);
                specObj.unk1 = br.ReadSingle();
                specObj.unk2 = br.ReadSingle();
                specObj.unk3 = br.ReadSingle();
                specObj.unk4 = br.ReadSingle();

                //Game Model
                long gameModelAddr = br.ReadPtr();
                long specObjAddr = br.GoToAddr(gameModelAddr);
                ReadGameModel(br, ref material, ref specObjMatrix, specObj.transform);
                br.BaseStream.Position = specObjAddr;

                //Name
                long specObjNameAddr = br.ReadPtr();
                specObj.name = NameTableBlock.GetName(specObjNameAddr);

                specObj.visibilityFn = br.ReadInt32();
                specObj.lodPtr = br.ReadInt32();
                specObj.boundingBoxIndex = br.ReadInt32();
                specObj.iablObjectPtr = br.ReadInt32();
                specObj.windShearFactor = br.ReadInt16();
                specObj.windSpeedFactor = br.ReadInt16();
                specObj.unkPtr = br.ReadInt32();
            }

            //Skip to game models
            //br.BaseStream.Position = gameModelsPos;
            //ReadAllGameModels(br, gameModelsCount);
        }

        public void ReadGameModel(BinaryReader br, ref MaterialBlock.NuMaterial material, ref Matrix4x4 matrix, Transform parent)
        {
            //Command Count
            int modelCmdCount = br.ReadInt32();

            //Material Pointer
            long pos = br.BaseStream.Position;
            int matIndex = br.ReadInt32();
            br.BaseStream.Position = pos + matIndex;
            matIndex = br.ReadInt32();
            if (matIndex >= 0 && matIndex < MaterialBlock.Instance.numaterials.Count) material = MaterialBlock.Instance.numaterials.Values.ElementAt(matIndex);
            //var matrix = Matrix4x4.identity;
            //meshRenderer.material = MaterialBlock.Instance.CreateMaterial(mat);

            //Command Index Pointer
            br.BaseStream.Position = pos + 4;
            int cmdIndexPtr = br.ReadInt32();
            br.BaseStream.Position = pos + 4 + cmdIndexPtr;
            int cmdIndex = br.ReadInt32();
            br.BaseStream.Position = pos + 8;

            RunDisplayCommands(cmdIndex, modelCmdCount, br, parent, ref material, ref matrix);
        }

        /*public void ReadAllGameModels(BinaryReader br, int gameModelsCount)
        {
            //Read game models
            MaterialBlock.NuMaterial material = default;
            Matrix4x4 matrix = Matrix4x4.identity;
            for (int i = 0; i < gameModelsCount; i++)
            {
                ReadGameModel(br, ref material, ref matrix, $"model_{i}");
            }
        }*/

        public void RunDisplayCommands(int commandIndex, int commandCount, BinaryReader br, Transform parent, ref MaterialBlock.NuMaterial material, ref Matrix4x4 matrix)
        {
            long brPos = br.BaseStream.Position;

            int meshCount = 0;
            //Reading Commands
            for (int j = 0; j < commandCount; j++)
            {
                var cmd = displayCommands[commandIndex + j];
                //Debug.Log($"parent {parent.name} reading command {j} ({cmd.type}, {cmd.flags})");
                //if (cmd.flags == DisplayCommand.Flags.Skip) continue;
                //if (cmd.flags == DisplayCommand.Flags.End) break;

                RunDisplayCommand(cmd, br, brPos, ref material, ref matrix, parent, $"mesh_{meshCount}");

                if (cmd.type == DisplayCommand.Type.GEOMCALL) meshCount++;
            }
        }

        private void RunDisplayCommand(DisplayCommand cmd, BinaryReader br, long brPos, ref MaterialBlock.NuMaterial material, ref Matrix4x4 matrix, Transform parent, string meshName)
        {
            switch (cmd.type)
            {
                case DisplayCommand.Type.MTL:
                    //br.BaseStream.Position = cmd.resourceAddr;
                    material = MaterialBlock.Instance.numaterials[cmd.resourceAddr];
                    break;
                case DisplayCommand.Type.GEOMCALL:
                    GameObject obj = new(meshName);
                    obj.transform.SetParent(parent);

                    var meshFilter = obj.AddComponent<MeshFilter>();
                    meshFilter.mesh = LoadMesh(br, cmd.resourceAddr, material.vertexFormatBits);
                    var meshRenderer = obj.AddComponent<MeshRenderer>();
                    meshRenderer.material = MaterialBlock.Instance.materials[material];

                    obj.transform.position = matrix.ExtractPosition();
                    obj.transform.rotation = matrix.ExtractRotation();
                    obj.transform.localScale = matrix.ExtractScale();
                    //Vector3 lossyScale = transform.lossyScale;
                    //Vector3 newScale = matrix.ExtractScale();
                    //obj.transform.localScale = new(newScale.x / lossyScale.x, newScale.y / lossyScale.y, newScale.z / lossyScale.z);
                    break;
                case DisplayCommand.Type.MTXLOAD:
                    br.BaseStream.Position = cmd.resourceAddr;
                    matrix = br.ReadM4x4();
                    break;
                default: break;
            }

            br.BaseStream.Position = brPos;
        }

        private DisplayCommand ReadDisplayCommand(BinaryReader br)
        {
            DisplayCommand dispCmd = new()
            {
                type = (DisplayCommand.Type)br.ReadByte(),
                flags = (DisplayCommand.Flags)br.ReadByte(),
                unk1 = br.ReadInt16(),
            };
            long pos = br.BaseStream.Position;
            int resourcePtr = br.ReadInt32();
            dispCmd.resourceAddr = pos + resourcePtr;

            dispCmd.unk2 = br.ReadInt32();
            dispCmd.unk3 = br.ReadInt32();

            return dispCmd;
        }

        private Mesh LoadMesh(BinaryReader br, long meshAddr, int vertexFormat)
        {
            long ogPos = br.BaseStream.Position;
            br.BaseStream.Position = meshAddr;

            int type = br.ReadInt32();
            if (type != 6)
            {
                Debug.LogWarning($"Non-Triangle Strip Mesh Type: {type}, Index: {br.BaseStream.Position - 4}");
                br.BaseStream.Position = ogPos;
                return null;
            }

            int triCount = br.ReadInt32();
            short vertSize = br.ReadInt16();
            short mesh_unk1 = br.ReadInt16();
            int mesh_unk2 = br.ReadInt32();
            int mesh_unk3 = br.ReadInt32();
            int vertOffset = br.ReadInt32();
            int vertCount = br.ReadInt32();
            int indexOffset = br.ReadInt32();
            int indexListID = br.ReadInt32();
            int vertListID = br.ReadInt32();
            int useDynamicBuffer = br.ReadInt32();
            int mesh_unk4_ptr = br.ReadInt32();
            int mesh_unk5 = br.ReadInt32();
            int dynamicBuffer = br.ReadInt32();

            //Vector3[] vertexList = NuGameScene.Instance.TempReadVertices(vertListID, vertSize, vertOffset, vertCount);
            //ushort[] indexList = NuGameScene.Instance.tempIndexBuffers[indexListID].Skip(indexOffset).Take(triCount + 2).ToArray();

            /*NuMesh mesh = new()
            {
                vertexBuffer = NuGameScene.Instance.TempReadVertexBuffer(vertListID, vertSize, vertOffset, vertCount),
                indexBuffer = ConvertTriangleStripIndicies(NuGameScene.Instance.tempIndexBuffers[indexListID].Skip(indexOffset).Take(triCount + 2).ToArray()),
            };*/

            var vertBuffer = NuGameScene.Instance.TempLoadVertexBuffer(vertListID, vertSize, vertOffset, vertCount);

            NuMesh numesh = NuMesh.Load(vertBuffer, vertexFormat, vertCount, vertSize);

            //Creating mesh
            Mesh mesh = new();
            mesh.SetVertices(numesh.vertexPositions);
            mesh.SetIndices(ConvertTriangleStripIndices(NuGameScene.Instance.tempIndexBuffers[indexListID].Skip(indexOffset).Take(triCount + 2).ToArray()), MeshTopology.Triangles, 0);

            if (numesh.vertexUV1s.Length > 0) mesh.SetUVs(0, numesh.vertexUV1s);
            if (numesh.vertexNormals.Length > 0) mesh.SetNormals(numesh.vertexNormals);

            mesh.RecalculateBounds();

            br.BaseStream.Position = ogPos;

            return mesh;
        }

        public override void Save(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public struct NuMesh
        {
            public enum AttributeType { None=0, Vector3=1, Packed4Bytes=2 }

            public Vector3[] vertexPositions;
            public Vector2[] vertexUV1s, vertexUV2s, vertexUV3s, vertexUV4s, vertexUV5s, vertexUV6s, vertexUV7s, vertexUV8s;
            public Vector3[] vertexNormals, vertexTangents, vertexBitangents;
            public Color32[] vertexColors1, vertexColors2;
            //vertex blend indices, blend weights, and other stuff

            public static NuMesh Load(BinaryReader vertexBuffer, int vertexFormat, int vertexCount, int vertSize) //vertSize TEMP
            {
                //Reading vertex format
                int normalType = 2;
                if (!vertexFormat.IsBitSet(4) && !vertexFormat.IsBitSet(20) && !vertexFormat.IsBitSet(24))
                    normalType = vertexFormat.GetBit(3);

                int tangentType = 2;
                if (!vertexFormat.IsBitSet(6) && !vertexFormat.IsBitSet(25))
                    tangentType = vertexFormat.GetBit(5);

                int bitangentType = 2;
                if (!vertexFormat.IsBitSet(26) && !vertexFormat.IsBitSet(32))
                    bitangentType = vertexFormat.GetBit(7);

                bool colorset1 = vertexFormat.IsBitSet(9);
                bool colorset2 = vertexFormat.IsBitSet(10) || vertexFormat.IsBitSet(11);

                bool halfFloatUvs = vertexFormat.IsBitSet(28); //Bit 28, Half-floats if set
                int uvSetCount = ((vertexFormat >> 11) & 7); //Bits 12, 13, 14

                //Local lists
                List<Vector3> vertices = new();
                List<Vector3> normals = new();
                List<Vector2>[] uvsets = new List<Vector2>[uvSetCount];
                for (int i = 0; i < uvSetCount; i++) uvsets[i] = new();

                //Reading Vertices
                for (int i = 0; i < vertexCount; i++)
                {
                    vertexBuffer.BaseStream.Position = vertSize * i;
                    vertices.Add(vertexBuffer.ReadVector3());

                    if (normalType == 1) normals.Add(vertexBuffer.ReadVector3());
                    else if (normalType == 2) vertexBuffer.ReadInt32();
                    if (tangentType == 1) normals.Add(vertexBuffer.ReadVector3());
                    else if (tangentType == 2) vertexBuffer.ReadInt32();
                    if (bitangentType == 1) normals.Add(vertexBuffer.ReadVector3());
                    else if (bitangentType == 2) vertexBuffer.ReadInt32();

                    if (colorset1) vertexBuffer.ReadInt32();
                    if (colorset2) vertexBuffer.ReadInt32();

                    for(int j=0; j<uvSetCount; j++)
                    {
                        if (halfFloatUvs) uvsets[j].Add(new(vertexBuffer.ReadHalf(), vertexBuffer.ReadHalf()));
                        else uvsets[j].Add(new(vertexBuffer.ReadSingle(), vertexBuffer.ReadSingle()));
                    }

                    //do other stuff later
                }

                return new()
                {
                    vertexPositions = vertices.ToArray(),
                    vertexNormals = normals.ToArray(),
                    vertexUV1s = uvsets.Length > 0 ? uvsets[0].ToArray() : new Vector2[0]
                };
            }
        }
    }

    [Serializable]
    public struct DisplayCommand
    {
        public enum Type
        {
            MTL = 0x80, GEOMCALL = 0x82, MTXLOAD = 0x83, TERMINATE = 0x84, MTL_CLIP = 0x85, DUMMY = 0x87, END_ITEM = 0x8E, LIGHTMAP_ITEM = 0xB0
        }

        public enum Flags
        {
            Skip = 0, Jump = 1, Run = 3, End = 4
        }

        public Type type;
        public Flags flags;
        public short unk1;
        public long resourceAddr;
        public int unk2, unk3;
    }
}
#endif