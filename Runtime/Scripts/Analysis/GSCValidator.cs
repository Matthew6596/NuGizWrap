#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace TTModdingKit.Analysis
{
    using Helper;
    using System;

    public static class GSCValidator
    {
        private enum Game { TCS, LIJ1LB1 }

        [MenuItem("TT Modding/Analysis/Validate GSC")]
        public static void ValidateGSC()
        {
            string path = EditorUtility.OpenFilePanel("Select a GSC File", "", "gsc");
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            ClearVars();

            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                ValidateGSC(bytes);
            }
            catch(IOException ioe)
            {
                Debug.LogError(ioe);
            }
        }

        private static void ClearVars()
        {
            head = new();
            ntbl = new();
        }

        public static void ValidateGSC(byte[] bytes, int index = 0)
        {
            int nu20Offset = bytes.ReadInt(ref index);

            const int NU20 = 0x4E553230; //"NU20" Header as int

            //If file starts with NU20, game is LIJ1/LB1, otherwise the read int is the offset to NU20 and game is TCS
            Game game;
            (nu20Offset, game) = nu20Offset == NU20 ? (0, Game.LIJ1LB1) : (nu20Offset, Game.TCS);

            //Read NU20
            index = nu20Offset; //+4 to skip "NU20"
            int nu20Header = bytes.ReadInt(ref index);
            if (nu20Header != NU20) Debug.LogError($"NU20 Header doesn't match at offset: {index}");
            int nu20Unk1 = bytes.ReadInt(ref index);
            int nu20Version = bytes.ReadInt(ref index);
            Debug.Log($"NU20 Version: {nu20Version}");
            int nu20Unk2 = bytes.ReadInt(ref index);

            //Read HEAD
            ReadBlock(bytes, ref index);

        }

        private static void ReadBlock(byte[] bytes, ref int index)
        {
            int startIndex = index;
            int blockID = bytes.ReadInt(ref index);
            int blockLen = bytes.ReadInt(ref index);

            string blockName = Encoding.UTF8.GetString(BitConverter.GetBytes(blockID), 0, 4);

            Block block = (blockName) switch
            {
                "HEAD" => head,
                "NTBL" => ntbl,
                _ => null
            };

            block?.Read(bytes, ref index);

            index = startIndex + blockLen;
        }

        public abstract class Block
        {
            public abstract void Read(byte[] bytes, ref int index);
        }

        private static HEAD head;
        public class HEAD : Block
        {
            public int PNTR_Offset;
            public int GSNH_Offset;
            public override void Read(byte[] bytes, ref int index)
            {
                PNTR_Offset = bytes.ReadInt(ref index);
                GSNH_Offset = bytes.ReadInt(ref index);
            }
        }

        private static NTBL ntbl;
        public class NTBL : Block
        {
            public string[] names;
            public override void Read(byte[] bytes, ref int index)
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif