#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NuGizWrap.GameScene
{
    public class PointerBlock : GscBlock
    {
        public Dictionary<int, int> pointers;

        //Based off of: https://github.com/Gatoradius95/RusTT/blob/main/src/map.rs
        public override void Load(BinaryReader br)
        {
            //Reading Ptr block may be unnecessary

            /*var bs = br.BaseStream;
            //using BinaryWriter bw = new(bs);
            pointers = new();

            //int pos = (int)bs.Position;
            int fileSize = (int)bs.Length;

            int numPointersParsed = 0;
            int pointerCount = br.ReadInt32();
            for (int i = 0; i < pointerCount; i++) 
            {
                if (bs.Position > fileSize - 4) break;
                int rel = br.ReadInt32();
                int ptr = (int)bs.Position + rel - 4;
                if (ptr > fileSize - 4 || ptr < 0) continue;
                bs.Position = ptr;
                int off = br.ReadInt32();
                if (off == 0) continue;
                int tgt = ptr + off;
                if (tgt < 0 || tgt > fileSize-4) continue;
                bs.Position = tgt;

                numPointersParsed++;
                //bw.Write(ptr);
                pointers.Add(ptr, tgt);
            }

            Debug.Log($"Total Pointers Written: {numPointersParsed}");*/
        }

        public override void Save(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif