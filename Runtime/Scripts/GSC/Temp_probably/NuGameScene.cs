#if UNITY_EDITOR
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    using Helper;
    using UnityEditor.AssetImporters;

    public class NuGameScene : MonoBehaviour
    {
        public static NuGameScene Instance { get; private set; }

        public int unk1;
        public int version;
        public int unk2;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Load(MemoryStream ms)
        {
            Instance = this;

            using BinaryReader br = new(ms);

            int nu20Offset = br.ReadInt32();

            //TempLoadTextures(br);
            TempSkipTextures(br);
            TempLoadVertices(br);
            TempLoadIndices(br);

            const int NU20 = 0x3032554E; //"NU20" Header as int

            //If file starts with NU20, game is LIJ1/LB1, otherwise the read int is the offset to NU20 and game is TCS
            nu20Offset = nu20Offset == NU20 ? 0 : nu20Offset + 4;

            //Read NU20
            br.BaseStream.Position = nu20Offset;
            int nu20Header = br.ReadInt32();
            if (nu20Header != NU20)
            {
                Debug.LogError($"NU20 Header doesn't match at offset: {br.BaseStream.Position}, is:{nu20Header} should be:{NU20}");
                return;
            }

            unk1 = br.ReadInt32();
            version = br.ReadInt32();
            unk2 = br.ReadInt32();

            long fileSize = br.BaseStream.Length;

            //Load Blocks, starting with HEAD
            do
            {
                long startIndex = br.BaseStream.Position;
                int blockID = br.ReadInt32();
                int blockLen = br.ReadInt32();
                string blockName = Encoding.UTF8.GetString(BitConverter.GetBytes(blockID), 0, 4);

                //Debug.Log($"Loading {blockName} block");

                GscBlock block = (blockName) switch
                {
                    "HEAD" => CreateBlock<HeadBlock>(),
                    "NTBL" => CreateBlock<NameTableBlock>(),
                    "MS00" => CreateBlock<MaterialBlock>(),
                    "SST0" => CreateBlock<SplineBlock>(),
                    "INID" => CreateBlock<IndicesBlock>(),
                    "FDNS" => CreateBlock<FDNSBlock>(),
                    "BNDS" => CreateBlock<BoundsBlock>(),
                    "DISP" => CreateBlock<DisplayBlock>(),
                    "IABL" => CreateBlock<IABLBlock>(),
                    "TAS0" => CreateBlock<TAS0Block>(),
                    "PORT" => CreateBlock<PortalBlock>(),
                    "TREF" => CreateBlock<TREFBlock>(),
                    "TST0" => CreateBlock<TextureSetBlock>(),
                    "VBIB" => CreateBlock<VBIBBlock>(),
                    "SALI" => CreateBlock<SALIBlock>(),
                    "ALA3" => CreateBlock<ALA3Block>(),
                    "DYNO" => CreateBlock<DYNOBlock>(),
                    //"GSNH" => CreateBlock<GSNHBlock>(), //Should be read by HEAD
                    //"PNTR" => CreateBlock<PointerBlock>(), //Should be read by HEAD
                    _ => null
                };
                
                if (block != null)
                {
                    block.gameObject.name = $"{blockName} Block";
                    block.Load(br);
                }
                //else Debug.LogWarning($"Block not found: id:'{string.Format("{0:X8}", blockID)}' name:'{blockName}'");

                if (startIndex + blockLen >= fileSize - 8) break;
                br.BaseStream.Position = startIndex + blockLen;
            } 
            while (br.BaseStream.Position < fileSize - 8);
        }

        public GscBlock CreateBlock<T>(string name="new_gsc_block") where T : GscBlock
        {
            var block = new GameObject(name).AddComponent<T>();
            block.transform.SetParent(transform);
            return block;
        }

        public void TempSkipTextures(BinaryReader br)
        {
            short textureCount = br.ReadInt16();
            tempTextures = new Texture2D[textureCount];
            for (int i = 0; i < textureCount; i++)
            {
                long pos = br.BaseStream.Position;
                int w = br.ReadInt32(), h = br.ReadInt32();
                int mipmap = br.ReadInt32();
                br.ReadInt32(); //unk1
                br.ReadInt32(); //unk2
                int dataSize = br.ReadInt32();
                long startPos = br.BaseStream.Position;

                char idstart = (char)br.PeekChar();
                if (idstart != 'D') { br.BaseStream.Position = pos; break; }

                //tempTextures[i] = DDSConvert.DDSBytesToTexture(br.ReadBytes(dataSize), FilterMode.Bilinear, TextureWrapMode.Repeat);
                br.BaseStream.Position = startPos + dataSize;
            }
        }

        public Texture2D[] tempTextures;
        public void TempLoadTextures(BinaryReader br, TextureSetBlock.TextureMeta[] metas)
        {
            Debug.Log($"loading temp txtrs, {metas.Length} metas count");
            br.BaseStream.Position = 4;
            short textureCount = br.ReadInt16();
            tempTextures = new Texture2D[textureCount];
            for(int i=0; i<textureCount; i++)
            {
                if (metas[i].width == 0 || metas[i].height == 0) continue;

                long pos = br.BaseStream.Position;
                int w = br.ReadInt32(), h = br.ReadInt32();
                int mipmap = br.ReadInt32();
                br.ReadInt32(); //unk1
                br.ReadInt32(); //unk2
                int dataSize = br.ReadInt32();
                long startPos = br.BaseStream.Position;

                char idstart = (char)br.PeekChar();
                if (idstart != 'D') { br.BaseStream.Position = pos; break; }

                try
                {
                    tempTextures[i] = FlipTextureVertically(DDSConvert.DDSBytesToTexture(br.ReadBytes(dataSize), FilterMode.Bilinear, TextureWrapMode.Repeat));
                    if (tempTextures[i] != null) tempTextures[i].name = $"Texture_{i}";
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading texture at {pos}: {e}");
                }
                tempTextures[i].alphaIsTransparency = true;
                br.BaseStream.Position = startPos + dataSize;
            }
        }

        public static Texture2D FlipTextureVertically(Texture2D sourceTxtr)
        {
            if (sourceTxtr == null)
                throw new ArgumentNullException(nameof(sourceTxtr));

            int width = sourceTxtr.width;
            int height = sourceTxtr.height;

            Color32[] pixels = sourceTxtr.GetPixels32();
            Color32[] flipped = new Color32[pixels.Length];

            for (int y = 0; y < height; y++)
            {
                int srcRow = y * width;
                int dstRow = (height - 1 - y) * width;
                Array.Copy(pixels, srcRow, flipped, dstRow, width);
            }

            var result = new Texture2D(width, height, TextureFormat.RGBA32, false);
            result.SetPixels32(flipped);
            result.Apply(false, false);

            return result;
        }

        /*private void FlipTexture(Texture2D txtr)
        {
            int w = txtr.width, h = txtr.height;

            var pixels = txtr.GetPixels(0, 0, w, h);
            Color[] newPixels = new Color[pixels.Length];

            for(int i=0; i<w; i++)
            {
                for(int j=0; j<h; j++)
                {
                    newPixels[i + j * w] = pixels[i + (h - j - 1) * w];
                }
            }

            txtr.SetPixels(newPixels);
            txtr.Apply();
        }*/

        public byte[][] tempVertexBuffers;
        private void TempLoadVertices(BinaryReader br)
        {
            short vertBufferCount = br.ReadInt16();
            //Debug.Log(vertBufferCount+" vert buffers at " + br.BaseStream.Position);
            tempVertexBuffers = new byte[vertBufferCount][];
            for(int i=0; i<vertBufferCount; i++)
            {
                int size = br.ReadInt32();
                tempVertexBuffers[i] = br.ReadBytes(size);
            }
        }

        public BinaryReader TempLoadVertexBuffer(int listID, short vertexSize, int vertexOffset, int vertexCount)
        {
            return new BinaryReader(new MemoryStream(TempReadVertexBuffer(listID, vertexSize, vertexOffset, vertexCount)));
        }

        public byte[] TempReadVertexBuffer(int vertexListID, short vertexSize, int vertexOffset, int vertexCount) 
            => tempVertexBuffers[vertexListID].Skip(vertexOffset * vertexSize).Take(vertexCount * vertexSize).ToArray();

        public Vector3[] TempReadVertices(int vertexListID, short vertexSize, int vertexOffset, int vertexCount)
        {
            List<Vector3> verts = new();
            byte[] buffer = TempReadVertexBuffer(vertexListID, vertexSize, vertexOffset, vertexCount);

            using (BinaryReader br = new(new MemoryStream(buffer)))
            {
                int skipSize = vertexSize - 12;
                for (int i = 0; i < vertexCount; i++)
                {
                    //Vertex v = new() { position = br.ReadVector3() };
                    /*if (vertexSize > 12) v.uvs = br.ReadInt32();
                    if (vertexSize > 16) v.uvs = br.ReadVector3();
                    if (vertexSize > ) v.color = br.ReadInt32();
                    verts.Add(v.position);*/
                    verts.Add(br.ReadVector3());

                    br.BaseStream.Seek(skipSize, SeekOrigin.Current);
                }
            }
            return verts.ToArray();
        }

        public ushort[][] tempIndexBuffers;
        private void TempLoadIndices(BinaryReader br)
        {
            short indexBufferCount = br.ReadInt16();
            //Debug.Log(indexBufferCount + " index buffers at " + br.BaseStream.Position);
            tempIndexBuffers = new ushort[indexBufferCount][];
            for (int i = 0; i < indexBufferCount; i++)
            {
                int size = br.ReadInt32()/2;
                tempIndexBuffers[i] = new ushort[size];
                for (int j = 0; j < size; j++) tempIndexBuffers[i][j] = br.ReadUInt16();
            }
        }
    }
}
#endif