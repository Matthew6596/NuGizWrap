#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class GSNHBlock : GscBlock
    {
        public static GSNHBlock Instance { get; private set; }

        public int textureCount;
        public int indexCount;
        public int vbibUnk1Count, vbibUnk2Count;

        public override void Load(BinaryReader br)
        {
            Instance = this;

            long textureIndexListAddr = br.ReadPtr();
            textureCount = br.ReadInt32();
            long textureMetaPtrsAddr = br.ReadPtr();
            long materialListAddr = br.ReadPtr();
            int materialCount = br.ReadInt32();

            int unk1 = br.ReadInt32();
            long ptr1Addr = br.ReadPtr();
            int unk2 = br.ReadInt32();
            long ptr2Addr = br.ReadPtr();
            int unk3 = br.ReadInt32();
            long ptr3Addr = br.ReadPtr();
            long ptr4Addr = br.ReadPtr();

            int splineCount = br.ReadInt32();
            long splineListAddr = br.ReadPtr();

            long ntblAddr = br.ReadPtr();

            int unk4 = br.ReadInt32();
            long ptr5Addr = br.ReadPtr();
            int unk5 = br.ReadInt32();
            int unk6 = br.ReadInt32();
            long iablObjsAddr = br.ReadPtr();
            long unk5ListAddr = br.ReadPtr();
            long unkAla3Addr = br.ReadPtr();
            indexCount = br.ReadInt32();
            long inidAddr = br.ReadPtr();

            int tas0Unk1Count = br.ReadInt32();
            long tas0Unk1Addr = br.ReadPtr();
            long tas0Unk2Addr = br.ReadPtr();

            int portUnk2Count = br.ReadInt32();
            long portUnk2Addr = br.ReadPtr();
            int portUnk3Count = br.ReadInt32();
            long portUnk3Addr = br.ReadPtr();
            int portUnk4Count = br.ReadInt32();
            long portUnk4Addr = br.ReadPtr();
            int portUnk5Count = br.ReadInt32();
            long portUnk5Addr = br.ReadPtr();
            int portRectPointsCount = br.ReadInt32();
            long portRectPointsAddr = br.ReadPtr();
            int portUnk7Count = br.ReadInt32();
            long portUnk7Addr = br.ReadPtr();

            long ptr6Addr = br.ReadPtr();
            long ptr7Addr = br.ReadPtr();
            long ptr8Addr = br.ReadPtr();
            long ptr9Addr = br.ReadPtr();
            long ptr10Addr = br.ReadPtr();
            long ptr11Addr = br.ReadPtr();
            long ptr12Addr = br.ReadPtr();
            long ptr13Addr = br.ReadPtr();
            long ptr14Addr = br.ReadPtr();
            long ptr15Addr = br.ReadPtr();
            long ptr16Addr = br.ReadPtr();
            long ptr17Addr = br.ReadPtr();
            long ptr18Addr = br.ReadPtr();
            long ptr19Addr = br.ReadPtr();
            long ptr20Addr = br.ReadPtr();
            long ptr21Addr = br.ReadPtr();

            int indexCount2 = br.ReadInt32();
            int indexCount3 = br.ReadInt32();

            int unk7 = br.ReadInt32();
            int unk8 = br.ReadInt32();

            long ptr22Addr = br.ReadPtr();
            int unk9 = br.ReadInt32();
            int unk10 = br.ReadInt32();
            long ptr23Addr = br.ReadPtr();
            long unkBndsAddr = br.ReadPtr();
            long ptr24Addr = br.ReadPtr();
            long ptr25Addr = br.ReadPtr();
            long ptr26Addr = br.ReadPtr();
            long ptr27Addr = br.ReadPtr();
            long dispAddr = br.ReadPtr();

            int unk11 = br.ReadInt32();
            long ptr28Addr = br.ReadPtr();
            long ptr29Addr = br.ReadPtr();
            int unk12 = br.ReadInt32();
            long ptr30Addr = br.ReadPtr();
            long ptr31Addr = br.ReadPtr();
            long ptr32Addr = br.ReadPtr();
            long ptr33Addr = br.ReadPtr();
            long ptr34Addr = br.ReadPtr();

            long portUnk2CountAddr = br.ReadPtr();

            long ptr35Addr = br.ReadPtr();
            long ptr36Addr = br.ReadPtr();
            long ptr37Addr = br.ReadPtr();
            int unk13 = br.ReadInt32();
            long ptr38Addr = br.ReadPtr();
            long unkDynoAddr = br.ReadPtr();
            long ptr39Addr = br.ReadPtr();
            int unk14 = br.ReadInt32();
            long ptr40Addr = br.ReadPtr();
            int unk15 = br.ReadInt32();
            long ptr41Addr = br.ReadPtr();

            int unk16 = br.ReadInt32();
            long ptr42Addr = br.ReadPtr();
            long ptr43Addr = br.ReadPtr();
            long ptr44Addr = br.ReadPtr();
            int unk17 = br.ReadInt32();
            long ptr45Addr = br.ReadPtr();
            int unk18 = br.ReadInt32();
            long ptr46Addr = br.ReadPtr();
            int unk19 = br.ReadInt32();
            long ptr47Addr = br.ReadPtr();
            int unk20 = br.ReadInt32();
            long ptr48Addr = br.ReadPtr();
            long ptr49Addr = br.ReadPtr();
            long ptr50Addr = br.ReadPtr();
            int unk21 = br.ReadInt32();
            int unk22 = br.ReadInt32();
            int unk23 = br.ReadInt32();
            int unk24 = br.ReadInt32();
            int unk25 = br.ReadInt32();
            int unk26 = br.ReadInt32();
            int unk27 = br.ReadInt32();
            int unk28 = br.ReadInt32();
            int unk29 = br.ReadInt32();
            int unk30 = br.ReadInt32();
            int unk31 = br.ReadInt32();
            int unk32 = br.ReadInt32();

            long vbibUnk1CountAddr = br.ReadPtr();
            int unk33 = br.ReadInt32();
            int unk34 = br.ReadInt32();
            long ptr51Addr = br.ReadPtr();
            long ala3Addr = br.ReadPtr();
            int unk35 = br.ReadInt32();
            long ptr52Addr = br.ReadPtr();
            long ptr53Addr = br.ReadPtr();
            int unk36 = br.ReadInt32();
            int unk37 = br.ReadInt32();
            long ptr54Addr = br.ReadPtr();
            long ptr55Addr = br.ReadPtr();
            long ptr56Addr = br.ReadPtr();
            long ptr57Addr = br.ReadPtr();
            int unk38 = br.ReadInt32();

            long[] materialAddrs = new long[materialCount];
            for(int i=0; i<materialCount; i++) materialAddrs[i] = br.ReadPtr();

            int[] textureIndices = new int[textureCount];
            for(int i=0; i<textureCount; i++) textureIndices[i] = br.ReadInt32();

            long[] txtrMetasAddrs = new long[textureCount];
            for (int i = 0; i < textureCount; i++) txtrMetasAddrs[i] = br.ReadPtr();

            Spline[] splines = new Spline[splineCount];
            for(int i=0; i<splineCount; i++) splines[i] = Spline.FromBytes(br);

            int unk40 = br.ReadInt32();
            int unk41 = br.ReadInt32();

            long matricesAddr = br.GoToAddr(iablObjsAddr-8);
            int iablCount = br.ReadInt32();
            br.BaseStream.Position = matricesAddr;
            Matrix4x4[] matrices = new Matrix4x4[iablCount];
            for(int i=0; i<iablCount; i++) matrices[i] = br.ReadM4x4();

            vbibUnk1Count = br.ReadInt32();
            long vbibUnk1Addr = br.ReadPtr();
            vbibUnk2Count = br.ReadInt32();
            long vbibUnk2Addr = br.ReadPtr();
            long meshPtrListAddr = br.ReadPtr();
            int meshCount = br.ReadInt32();
            long pntrBlockAddr = br.ReadPtr();
            int unk39 = br.ReadInt32();

            Debug.Log($"matrices addr: {matricesAddr}, mesh count?: {meshCount}, meshPtrsList addr: {br.Pos()}");
            //long[] meshAddrs = new long[meshCount];
            //for(int i=0; i<meshCount; i++) meshAddrs[i] = br.ReadPtr();

            //Load Texture Metas
            long endAddr = br.GoToAddr(textureMetaPtrsAddr);
            TextureSetBlock.TextureMeta[] metas = new TextureSetBlock.TextureMeta[textureCount];
            for(int i=0; i<textureCount; i++)
            {
                long metaAddr = br.ReadPtr();
                long prev = br.GoToAddr(metaAddr);
                metas[i] = TextureSetBlock.TextureMeta.FromBytes(br);
                br.BaseStream.Position = prev;
            }

            Debug.Log("Loading textures?");
            NuGameScene.Instance.TempLoadTextures(br, metas);
            //TempLoadMeshes(br);
        }

        //public Mesh[] tempMeshes;
        public NuMesh[] meshes;
        private void TempLoadMeshes(BinaryReader br)
        {
            //Other stuff
            br.BaseStream.Seek(111 * 4, SeekOrigin.Current);

            int vbibUnk1CountPtr = br.ReadInt32() + 16;
            br.BaseStream.Seek(vbibUnk1CountPtr, SeekOrigin.Current);

            int meshCount = br.ReadInt32();
            //tempMeshes = new Mesh[meshCount];
            meshes = new NuMesh[meshCount];
            br.BaseStream.Seek(8, SeekOrigin.Current);

            for (int i = 0; i <meshCount; i++)
            {
                //Read mesh pointer
                long pos = br.BaseStream.Position;
                int offset = br.ReadInt32();
                br.BaseStream.Position = pos + offset;

                //Read mesh
                int type = br.ReadInt32();
                if(type != 6)
                {
                    Debug.LogWarning($"Non-Triangle Strip Mesh Type: {type}, Index: {br.BaseStream.Position - 4}");
                    br.BaseStream.Position = pos + 4;
                    continue;
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

                NuMesh mesh = new()
                {
                    vertexBuffer = NuGameScene.Instance.TempReadVertexBuffer(vertListID, vertSize, vertOffset, vertCount),
                    indexBuffer = ConvertTriangleStripIndices(NuGameScene.Instance.tempIndexBuffers[indexListID].Skip(indexOffset).Take(triCount + 2).ToArray()),
                };

                meshes[i] = mesh;

                /*Mesh mesh = new();
                mesh.SetVertices(vertexList);
                mesh.SetIndices(ConvertTriangleStripIndicies(indexList), MeshTopology.Triangles, 0);
                tempMeshes[i] = mesh;*/

                //Advance in the pointer list
                br.BaseStream.Position = pos + 4;
            }
        }

        public static ushort[] ConvertTriangleStripIndices(ushort[] indices)
        {
            List<ushort> triangleList = new();
            bool swapWinding = true;
            for (int i = 0; i < indices.Length - 2; i++)
            {
                // Every other triangle needs winding order flipped
                // to keep normals/culling consistent
                if (swapWinding)
                {
                    triangleList.Add(indices[i]);
                    triangleList.Add(indices[i + 1]);
                    triangleList.Add(indices[i + 2]);
                }
                else
                {
                    triangleList.Add(indices[i + 1]);
                    triangleList.Add(indices[i]);
                    triangleList.Add(indices[i + 2]);
                }

                swapWinding = !swapWinding;
            }
            return triangleList.ToArray();
        }

        public struct NuMesh
        {
            public byte[] vertexBuffer;
            public ushort[] indexBuffer;
        }

        public struct Spline
        {
            public ushort pointCount;
            public ushort unk;
            public long nameAddr, pointsAddr;

            public static Spline FromBytes(BinaryReader br)
            {
                return new()
                {
                    pointCount = br.ReadUInt16(),
                    unk = br.ReadUInt16(),
                    nameAddr = br.ReadPtr(),
                    pointsAddr = br.ReadPtr(),
                };
            }
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.GSNHAddress = bw.Pos();
            throw new System.NotImplementedException();
        }
    }
}
#endif