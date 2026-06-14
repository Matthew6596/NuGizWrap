#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TTModdingKit.GizFlow
{
    public class GitOptions : IGitNode
    {
        public string ID => "GitOptions";

        public Vector3 cameraPosTemp;
        public Color bgColor, cbColor, txtColor;
        public bool rclickDel;

        public void ContentFromLines(IEnumerable<string> linesIEnumerable, ref int index)
        {
            string[] lines = linesIEnumerable.ToArray();
            string line = lines[index].Trim();

            Vector3 ParseVec3(string val)
            {
                string[] vals = val.Split(' ');
                return new Vector3(float.Parse(vals[0]), float.Parse(vals[1]), float.Parse(vals[2]));
            }

            Color ParseColor(string val)
            {
                Vector3 v = ParseVec3(val);
                return new Color(v.x, v.y, v.z);
            }

            while (!line.Contains('}'))
            {
                int spaceInd = line.IndexOf(' ');
                string prop = line[..(spaceInd-1)];
                string val = line[(spaceInd + 1)..].Trim();

                switch (prop)
                {
                    case "CameraPos": SetCameraPos(ParseVec3(val)); break;
                    case "BgColour": bgColor = ParseColor(val); break;
                    case "CbColour": cbColor = ParseColor(val); break;
                    case "txtColour": txtColor = ParseColor(val); break;
                    case "RClickDel": rclickDel = !val.Contains('0'); break;
                }

                index++;
                if (index >= lines.Length) break;
                line = lines[index].Trim();
            }
        }

        public IEnumerable<string> ContentToLines()
        {
            Vector3 camPos = GetCameraPos();
            return new string[]
            {
                $"\tCameraPos {camPos.x} {camPos.y} {camPos.z}",
                $"\tBgColour {bgColor.r} {bgColor.g} {bgColor.b}",
                $"\tCbColour {txtColor.r} {txtColor.g} {txtColor.b}",
                $"\ttxtColour {txtColor.r} {txtColor.g} {txtColor.b}",
                $"\tRClickDel {(rclickDel ? 1 : 0)}"
            };
        }

        public Vector3 GetCameraPos()
        {
            return cameraPosTemp;
        }

        public void SetCameraPos(Vector3 pos)
        {
            cameraPosTemp = pos;
        }
    }
}
#endif