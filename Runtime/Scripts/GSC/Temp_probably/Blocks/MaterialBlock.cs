#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;
    using System.Collections.Generic;

    public class MaterialBlock : GscBlock
    {
        public static MaterialBlock Instance { get; private set; }

        public int unknown1;
        public Dictionary<NuMaterial, Material> materials;
        public Dictionary<long, NuMaterial> numaterials;

        public override void Load(BinaryReader br)
        {
            Instance = this;

            int matCount = br.ReadInt32();
            numaterials = new();
            materials = new();
            unknown1 = br.ReadInt32();

            for(int i=0; i<matCount; i++)
            {
                long matPos = br.BaseStream.Position;
                var numat = NuMaterial.LoadNew(br);
                numaterials.Add(matPos, numat);
                materials.Add(numat, CreateMaterial(numat));
            }
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.MS00Address = bw.Pos();

            //Create/update NuMaterials from materials

            //Write NuMaterials
            bw.Write(numaterials.Count);
            bw.Write(unknown1);

            foreach(var numat in numaterials.Values) numat.Write(bw);
        }

        public override void PostSave(BinaryWriter bw)
        {
            bw.Seek(8, SeekOrigin.Current);

            foreach (var numat in numaterials.Values)
            {
                long pos = bw.BaseStream.Position;
                numat.PostWrite(bw);
                bw.BaseStream.Position = pos + 708; //Each material is 708 bytes
            }
        }

        public Material CreateMaterial(NuMaterial numat)
        {
            Material mat = new(Shader.Find("Universal Render Pipeline/Unlit"));
            //numat.diffuseColor = new(numat.diffuseColor.r, numat.diffuseColor.g, numat.diffuseColor.b, 1);
            mat.color = numat.diffuseColor;
            if(numat.diffuseTextureID >= 0 && numat.diffuseTextureID < NuGameScene.Instance.tempTextures.Length)
                mat.mainTexture = NuGameScene.Instance.tempTextures[numat.diffuseTextureID];
            return mat;
        }

        [Serializable]
        public struct NuMaterial
        {
            public byte[] unk1;
            public int ptr1, ptr2, ptr3, unk2;
            public int materialID, ptr4, alphaBlend;
            public byte[] unk15;
            public Color diffuseColor, unkColor;
            public short diffuseTextureID, unkTextureID1, unkTextureID2;
            public byte[] unk3;
            public int textureFlags;
            public int diffuseTextureID2, layer1TextureID;
            public byte[] unk4;
            public byte combineOp1, unk5, unk6, unk7;
            public int specularTextureID, normalTextureID;
            public byte[] unk8;
            public float reflectionPower, exponent;
            public byte[] unk9;
            public float frenselMult, frenselCoef;
            public byte[] unk10;
            public byte lightmapIndex, surfaceIndex, specularIndex, normalIndex;
            public byte[] unk11;
            public int uvAnimEnabled1, uvAnimEnabled2, uvAnimEnabled3, uvAnimEnabled4;
            public byte[] unk12;
            public int vertexFormatBits, unk16;
            public TextureAnim textureAnim1, textureAnim2, textureAnim3, textureAnim4;
            public byte[] unk13;
            public int inputDefines, shaderDefines, uvSetCoords;
            public byte[] unk14;

            public NuMaterial(
                byte[] unk1, int ptr1, int ptr2, int ptr3, int unk2,
                int materialID, int ptr4, int alphaBlend, byte[] unk15, Color diffuseColor, Color unkColor,
                short diffuseTextureID, short unkTextureID1, short unkTextureID2,
                byte[] unk3, int textureFlags, int diffuseTextureID2, int layer1TextureID, byte[] unk4,
                byte combineOp1, byte unk5, byte unk6, byte unk7, int specularTextureID, int normalTextureID,
                byte[] unk8, float reflectionPower, float exponent, byte[] unk9, float frenselMult, float frenselCoef,
                byte[] unk10, byte lightmapIndex, byte surfaceIndex, byte specularIndex, byte normalIndex,
                byte[] unk11, int uvAnimEnabled1, int uvAnimEnabled2, int uvAnimEnabled3, int uvAnimEnabled4,
                byte[] unk12, int vertextFormatBits1, int vertexFormatBits2,
                TextureAnim textureAnim1, TextureAnim textureAnim2, TextureAnim textureAnim3, TextureAnim textureAnim4,
                byte[] unk13, int inputDefines, int shaderDefines, int uvSetCoords, byte[] unk14
            )
            {
                this.unk1 = unk1;
                this.ptr1 = ptr1;
                this.ptr2 = ptr2;
                this.ptr3 = ptr3;
                this.unk2 = unk2;
                this.materialID = materialID;
                this.ptr4 = ptr4;
                this.alphaBlend = alphaBlend;
                this.unk15 = unk15;
                this.diffuseColor = diffuseColor;
                this.unkColor = unkColor;
                this.diffuseTextureID = diffuseTextureID;
                this.unkTextureID1 = unkTextureID1;
                this.unkTextureID2 = unkTextureID2;
                this.unk3 = unk3;
                this.textureFlags = textureFlags;
                this.diffuseTextureID2 = diffuseTextureID2;
                this.layer1TextureID = layer1TextureID;
                this.unk4 = unk4;
                this.combineOp1 = combineOp1;
                this.unk5 = unk5;
                this.unk6 = unk6;
                this.unk7 = unk7;
                this.specularTextureID = specularTextureID;
                this.normalTextureID = normalTextureID;
                this.unk8 = unk8;
                this.reflectionPower = reflectionPower;
                this.exponent = exponent;
                this.unk9 = unk9;
                this.frenselMult = frenselMult;
                this.frenselCoef = frenselCoef;
                this.unk10 = unk10;
                this.lightmapIndex = lightmapIndex;
                this.surfaceIndex = surfaceIndex;
                this.specularIndex = specularIndex;
                this.normalIndex = normalIndex;
                this.unk11 = unk11;
                this.uvAnimEnabled1 = uvAnimEnabled1;
                this.uvAnimEnabled2 = uvAnimEnabled2;
                this.uvAnimEnabled3 = uvAnimEnabled3;
                this.uvAnimEnabled4 = uvAnimEnabled4;
                this.unk12 = unk12;
                this.vertexFormatBits = vertextFormatBits1;
                this.unk16 = vertexFormatBits2;
                this.textureAnim1 = textureAnim1;
                this.textureAnim2 = textureAnim2;
                this.textureAnim3 = textureAnim3;
                this.textureAnim4 = textureAnim4;
                this.unk13 = unk13;
                this.inputDefines = inputDefines;
                this.shaderDefines = shaderDefines;
                this.uvSetCoords = uvSetCoords;
                this.unk14 = unk14;
            }

            public static NuMaterial LoadNew(BinaryReader br)
            {
                NuMaterial numat = new(
                    br.ReadBytes(40), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(),
                    br.ReadInt32(), br.ReadBytes(16), br.ReadColorA(), br.ReadColorA(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadBytes(58),
                    br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadBytes(56), br.ReadByte(), br.ReadByte(), br.ReadByte(),
                    br.ReadByte(), br.ReadInt32(), br.ReadInt32(), br.ReadBytes(40), br.ReadSingle(), br.ReadSingle(), br.ReadBytes(16),
                    br.ReadSingle(), br.ReadSingle(), br.ReadBytes(16), br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte(),
                    br.ReadBytes(96), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadBytes(32), br.ReadInt32(),
                    br.ReadInt32(), TextureAnim.Read(br), TextureAnim.Read(br), TextureAnim.Read(br), TextureAnim.Read(br),
                    br.ReadBytes(32), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadBytes(80)
                    );
                return numat;
            }

            public readonly void Write(BinaryWriter bw)
            {
                bw.Write(unk1); //[TODO LATER] ensure byte arrays correct size
                PointerBlock.WritePlaceholdPtr(bw);
                PointerBlock.WritePlaceholdPtr(bw);
                PointerBlock.WritePlaceholdPtr(bw);
                bw.Write(unk2);
                bw.Write(materialID);
                PointerBlock.WritePlaceholdPtr(bw);
                bw.Write(alphaBlend);
                bw.Write(unk15);
                bw.WriteColorA(diffuseColor);
                bw.WriteColorA(unkColor);
                bw.Write(diffuseTextureID);
                bw.Write(unkTextureID1);
                bw.Write(unkTextureID2);
                bw.Write(unk3);
                bw.Write(textureFlags);
                bw.Write(diffuseTextureID2);
                bw.Write(layer1TextureID);
                bw.Write(unk4);
                bw.Write(combineOp1);
                bw.Write(unk5);
                bw.Write(unk6);
                bw.Write(unk7);
                bw.Write(specularTextureID);
                bw.Write(normalTextureID);
                bw.Write(unk8);
                bw.Write(reflectionPower);
                bw.Write(exponent);
                bw.Write(unk9);
                bw.Write(frenselMult);
                bw.Write(frenselCoef);
                bw.Write(unk10);
                bw.Write(lightmapIndex);
                bw.Write(surfaceIndex);
                bw.Write(specularIndex);
                bw.Write(normalIndex);
                bw.Write(unk11);
                bw.Write(uvAnimEnabled1);
                bw.Write(uvAnimEnabled2);
                bw.Write(uvAnimEnabled3);
                bw.Write(uvAnimEnabled4);
                bw.Write(unk12);
                bw.Write(vertexFormatBits);
                bw.Write(unk16);
                textureAnim1.Write(bw);
                textureAnim2.Write(bw);
                textureAnim3.Write(bw);
                textureAnim4.Write(bw);
                bw.Write(unk13);
                bw.Write(inputDefines);
                bw.Write(shaderDefines);
                bw.Write(uvSetCoords);
                bw.Write(unk14);
            }

            public readonly void PostWrite(BinaryWriter bw)
            {
                bw.Seek(40, SeekOrigin.Current);
                //bw.WritePtr(ptr1Addr);
                //bw.WritePtr(ptr2Addr);
                //bw.WritePtr(ptr3Addr);
                bw.Seek(8, SeekOrigin.Current);
                //bw.WritePtr(ptr4Addr);
            }
        }

        [Serializable]
        public struct TextureAnim
        {
            public byte animTypeX, animTypeY, unk1, unk2;
            public float XTrigScale, YTrigScale;
            public float XScrollSpeed, YScrollSpeed;

            public static TextureAnim Read(BinaryReader br) => new()
                {
                    animTypeX = br.ReadByte(),
                    animTypeY = br.ReadByte(),
                    unk1 = br.ReadByte(),
                    unk2 = br.ReadByte(),
                    XTrigScale = br.ReadSingle(),
                    YTrigScale = br.ReadSingle(),
                    XScrollSpeed = br.ReadSingle(),
                    YScrollSpeed = br.ReadSingle()
                };

            public readonly void Write(BinaryWriter bw)
            {
                bw.Write(animTypeX);
                bw.Write(animTypeY);
                bw.Write(unk1);
                bw.Write(unk2);
                bw.Write(XTrigScale);
                bw.Write(YTrigScale);
                bw.Write(XScrollSpeed);
                bw.Write(YScrollSpeed);
            }
        }
    }
}
#endif