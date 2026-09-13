#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NuGizWrap.GameScene
{
    using Helper;

    public class NameTableBlock : GscBlock
    {
        public static NameTableBlock Instance { get; private set; }

        public readonly Dictionary<long, string> nametable = new();
        private readonly Dictionary<string, int> nameOffsets = new();
        private int nametableByteSize = 0;

        public override void Load(BinaryReader br)
        {
            Instance = this;
            long endAddr = br.ReadInt32() + br.Pos();

            nametable.Clear();
            nameOffsets.Clear();
            while (br.Pos() < endAddr) nametable.Add(br.Pos(), LoadStr(br, endAddr));
        }

        public string LoadStr(BinaryReader br, long endAddr)
        {
            string str = string.Empty;
            byte c;
            while ((c = br.ReadByte()) != 0 && br.Pos() < endAddr)
            {
                str += (char)c;
            }
            return str;
        }

        public override void Save(BinaryWriter bw)
        {
            GSCExporter.NTBLAddress = bw.BaseStream.Position;

            nameOffsets.Clear();
            int offset = 4;

            bw.Write(0); //write entire size in bytes (later)

            foreach(var pair in nametable)
            {
                string name = pair.Value;
                var chars = name.ToCharArray().Append('\0').ToArray();
                bw.Write(chars);

                nameOffsets.Add(name,offset);
                offset += chars.Length;
            }

            nametableByteSize = offset - 4;
            bw.Write((int)0); //padding?
        }

        public override void PostSave(BinaryWriter bw)
        {
            bw.Write(nametableByteSize); //not a pointer, but still writing it in post
        }

        public override long GetPtrAddress(object key)
        {
            if(key is string n)
            {
                if (!nametable.ContainsValue(n)) return -1;
                return GSCExporter.NTBLAddress + nameOffsets[n];
            }
            else if(key is int ind)
            {
                if (ind == -1) return -1;
                return GSCExporter.NTBLAddress + nameOffsets.ElementAt(ind).Value;
            }

            return -1;
        }

        public static string GetName(long address)
        {
            var nametable = Instance.nametable;
            if (nametable.TryGetValue(address, out string n)) return n;
            else return string.Empty;
        }
    }
}
#endif