#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NuGizWrap.GameScene
{
    public class NameTableBlock : GscBlock
    {
        public static NameTableBlock Instance { get; private set; }

        public string[] nametable;
        private readonly List<int> nameOffsets = new();

        public override void Load(BinaryReader br)
        {
            Instance = this;
            int sectionSize = br.ReadInt32();

            List<string> names = new();
            string str;
            while ((str = LoadStr(br)) != string.Empty) names.Add(str);
            nametable = names.ToArray();
        }

        public static string LoadStr(BinaryReader br)
        {
            string str = string.Empty;
            char c;
            while ((c = br.ReadChar()) != '\0') str += c;
            return str;
        }

        public override void Save(BinaryWriter bw)
        {
            nameOffsets.Clear();
            int offset = 4;

            bw.Write(nametable.Length);

            for(int i=0; i<nametable.Length; i++)
            {
                var chars = nametable[i].ToCharArray().Append('\0').ToArray();
                bw.Write(chars);

                nameOffsets.Add(offset);
                offset += chars.Length;
            }

            bw.Write((int)0); //padding?
        }

        public override long GetPtrAddress(object key)
        {
            if(key is string n)
            {
                int nameInd = Array.IndexOf(nametable, n);
                if (nameInd == -1) return -1;
                return GSCExporter.NTBLAddress + nameOffsets[nameInd];
            }
            else if(key is int ind)
            {
                if (ind == -1) return -1;
                return GSCExporter.NTBLAddress + nameOffsets[ind];
            }

            return -1;
        }
    }
}
#endif