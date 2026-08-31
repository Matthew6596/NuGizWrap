#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace NuGizWrap.GameScene
{
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
                    //"TAS0" => CreateBlock<TAS0Block>(),
                    //"PORT" => CreateBlock<PortalBlock>(),
                    "TREF" => CreateBlock<TREFBlock>(),
                    "TST0" => CreateBlock<TextureSetBlock>(),
                    "VBIB" => CreateBlock<VBIBBlock>(),
                    "SALI" => CreateBlock<SALIBlock>(),
                    "ALA3" => CreateBlock<ALA3Block>(),
                    "DYNO" => CreateBlock<DYNOBlock>(),
                    "GSNH" => CreateBlock<GSNHBlock>(),
                    "PNTR" => CreateBlock<PointerBlock>(),
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

        /*private void LoadBlock(BinaryReader br)
        {
            long startIndex = br.BaseStream.Position;
            int blockID = br.ReadInt32();
            int blockLen = br.ReadInt32();

            string blockName = Encoding.UTF8.GetString(BitConverter.GetBytes(blockID), 0, 4);
            if (blockName == "NUS:") return;

            Debug.Log($"Loading {blockName} block");

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
                "TREF" => CreateBlock<TREFBlock>(),
                "TST0" => CreateBlock<TextureSetBlock>(),
                "VBIB" => CreateBlock<VBIBBlock>(),
                "SALI" => CreateBlock<SALIBlock>(),
                "ALA3" => CreateBlock<ALA3Block>(),
                "DYNO" => CreateBlock<DYNOBlock>(),
                //If HeadBlock Instance exists, it should've already read GSNH/PointerBlock
                "GSNH" => HeadBlock.Instance != null ? null : CreateBlock<GSNHBlock>(),
                "PNTR" => HeadBlock.Instance != null ? null : CreateBlock<PointerBlock>(),
                _ => null
            };
            block.gameObject.name = $"{blockName} Block";

            if (block != null) block.Load(br);
            else Debug.LogWarning($"Block not found: id:'{string.Format("{0:X8}",blockID)}' name:'{blockName}'");

            br.BaseStream.Position = startIndex + blockLen;
        }*/

        public GscBlock CreateBlock<T>(string name="new_gsc_block") where T : GscBlock
        {
            var block = new GameObject(name).AddComponent<T>();
            block.transform.SetParent(transform);
            return block;
        }
    }
}
#endif