#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NuGizWrap.Analysis
{
    using Helper;

    public static class HardcodedAnalysis
    {
        [MenuItem("Nu Giz Wrap/Analysis/Hardcoded/General Analysis")]
        public static void General()
        {
            string[] rootPaths = new[]
            {
                Path.Combine(Path.GetDirectoryName(TTUnityProject.GetGamePath(TTGame.TCS)),"LEVELS"),
                Path.Combine(Path.GetDirectoryName(TTUnityProject.GetGamePath(TTGame.LIJ1)),"LEVELS"),
                Path.Combine(Path.GetDirectoryName(TTUnityProject.GetGamePath(TTGame.LB1)),"LEVELS")
            };

            //HARDCODED GIZMO ANALYSIS CONFIGURATION
            //GizmosAnalysis(rootPaths, GizAnalysisMode.One, GetGizOneCode("Tube", 6)); //prop 7 = specObj
            GizmosAnalysis(rootPaths, GizAnalysisMode.One, GetGizOneCode("Lever", 13));
            GizmosAnalysis(rootPaths, GizAnalysisMode.One, GetGizOneCode("Lever", 14));
            GizmosAnalysis(rootPaths, GizAnalysisMode.One, GetGizOneCode("Lever", 15));
        }

        [MenuItem("Nu Giz Wrap/Analysis/Hardcoded/TCS Analysis")]
        public static void AnalyseTCS()
        {
            string[] rootPaths = new[] { Path.Combine(Path.GetDirectoryName(TTUnityProject.GetGamePath(TTGame.TCS)), "LEVELS") };
            //GizmosAnalysis(rootPaths, GizAnalysisMode.One, GetGizOneCode("Tube", 4));
        }

        [MenuItem("Nu Giz Wrap/Analysis/Hardcoded/LIJ1 Analysis")]
        public static void AnalyseLIJ1()
        {
            string[] rootPaths = new[] { Path.Combine(Path.GetDirectoryName(TTUnityProject.GetGamePath(TTGame.LIJ1)), "LEVELS") };
            //GizmosAnalysis(rootPaths, GizAnalysisMode.One, GetGizOneCode("Tube", 7));
            GizmosAnalysis(rootPaths, GizAnalysisMode.One, GetGizOneCode("Grapple", 12));
        }

        [MenuItem("Nu Giz Wrap/Analysis/Hardcoded/LB1 Analysis")]
        public static void AnalyseLB1()
        {
            string[] rootPaths = new[] { Path.Combine(Path.GetDirectoryName(TTUnityProject.GetGamePath(TTGame.LB1)), "LEVELS") };
            //GizmosAnalysis(rootPaths, GizAnalysisMode.One, GetGizOneCode("Tube", 7));
        }

        public enum GizAnalysisMode { All = 0, One = 1 }

        private static readonly string[] GizmoIDs = new string[] { "GizObstacle", "GizBuildit", "GizForce", "blowup", "GizDig", "GizmoPickup", "Shard", "Signal", "Grapple", "TightRope", "Ledge", "Lever", "Spinner", "Techno", "SecurityDoor", "Attracto", "MiniCut", "Tube", "ZipUp", "Whipper", "GizTurret", "BombGenerator", "Panel", "HatMachine", "Plug", "PushBlocks", "Torp Machine", "ShadowEditor", "Teleport", "Puzzle", "GizFlock" };

        private static int GetGizmoIDIndex(string gizID) => Array.IndexOf(GizmoIDs, gizID);

        private static uint GetGizOneCode(string gizID, int propertyNum) => ((uint)(propertyNum << 6)) | (uint)GetGizmoIDIndex(gizID);

        public static void GizmosAnalysis(string[] rootPaths, GizAnalysisMode mode = GizAnalysisMode.All, uint code=0)
        {
            List<string> files = new();
            foreach (var root in rootPaths) 
                if (Directory.Exists(root)) files.AddRange(Directory.EnumerateFiles(root, "*.giz", SearchOption.AllDirectories));

            Dictionary<string, int> valueCounts = new();
            void CountValue(object value, StringBuilder logOutput, string log="")
            {
                string val = value.ToString();
                if (log != string.Empty) logOutput.AppendLine(log);
                if (valueCounts.ContainsKey(val)) valueCounts[val]++;
                else valueCounts.Add(val, 1);
            }

            Debug.Log($"Analyzing {files.Count} giz files...");

            int gizCode = mode == GizAnalysisMode.One ? (int)(code & 0b111111) : -1;
            int propCode = mode == GizAnalysisMode.One ? (int)(code >> 6) : -1;

            void AnalyseGizFile(BinaryReader br, string filepath, StringBuilder output)
            {
                //Debug.Log($"Analysing File: {filepath}");
                //output.AppendLine($"Analysing File: {Path.GetFileName(filepath)} ({filepath})");

                if (br.BaseStream.Length == 0) { Debug.LogWarning($"Giz file had length of 0: {filepath}"); return; }

                int magic = br.ReadInt32();
                if (magic != 1) { Debug.LogWarning($"Giz file didn't start with 1: {filepath}"); return; }

                int IDLen;
                while(br.BaseStream.Position < br.BaseStream.Length-4 && (IDLen = br.ReadInt32()) != 0)
                {
                    string id = br.ReadString(IDLen);
                    int sectionLen = br.ReadInt32();
                    if (sectionLen == 0) continue;

                    long startPos = br.BaseStream.Position;
                    
                    int version=-1;
                    int gizCount=-1;
                    switch (id)
                    {
                        case "GizObstacle":
                            if (mode == GizAnalysisMode.One && gizCode != 0) break;
                            //code
                            break;
                        case "GizBuildit":
                            if (mode == GizAnalysisMode.One && gizCode != 1) break;
                            //code
                            break;
                        case "GizForce":
                            if (mode == GizAnalysisMode.One && gizCode != 2) break;
                            //code
                            break;
                        case "blowup":
                            if (mode == GizAnalysisMode.One && gizCode != 3) break;
                            //code
                            break;
                        case "GizDig":
                            if (mode == GizAnalysisMode.One && gizCode != 4) break;
                            //code
                            break;
                        case "GizmoPickup":
                            if (mode == GizAnalysisMode.One && gizCode != 5) break;
                            //code
                            break;
                        case "Shard":
                            if (mode == GizAnalysisMode.One && gizCode != 6) break;
                            //code
                            break;
                        case "Signal":
                            if (mode == GizAnalysisMode.One && gizCode != 7) break;
                            //code
                            break;
                        case "Grapple": 
                            if (mode == GizAnalysisMode.One && gizCode != 8) break;
                            //code
                            version = br.ReadInt32();
                            gizCount = br.ReadInt32();
                            for (int i = 0; i < gizCount; i++)
                            {
                                string grappleName = br.ReadString(16);
                                if (propCode == 2) CountValue(grappleName, output, $"Grapple '{grappleName}' | {filepath}");
                                Vector3 grapplePos = br.ReadVector3();
                                if (propCode == 3) CountValue(grapplePos, output, $"Grapple '{grappleName}' Pos {grapplePos} | {filepath}");

                                if (version < 2) br.ReadInt16(); //padding

                                short yrot = br.ReadInt16();
                                if (propCode == 4) CountValue(yrot, output, $"Grapple '{grappleName}' Y-Rot {yrot} | {filepath}");

                                if (version >= 3)
                                {
                                    float unk3 = br.ReadSingle();
                                    if (propCode == 5) CountValue(unk3, output, $"Grapple '{grappleName}' Unk3 {unk3} | {filepath}");
                                }

                                if (version >= 4)
                                {
                                    bool swing = br.ReadByte() != 0;
                                    if (propCode == 6) CountValue(swing, output, $"Grapple '{grappleName}' Swing Rope {swing} | {filepath}");

                                    float ropeLen = br.ReadSingle();
                                    if (propCode == 7) CountValue(ropeLen, output, $"Grapple '{grappleName}' Rope Length {ropeLen} | {filepath}");
                                }

                                if (version >= 5)
                                {
                                    short xrot = br.ReadInt16();
                                    if (propCode == 8) CountValue(xrot, output, $"Grapple '{grappleName}' X-Rot {xrot} | {filepath}");
                                }

                                if (version >= 6)
                                {
                                    bool nomove = br.ReadByte() != 0;
                                    if (propCode == 9) CountValue(nomove, output, $"Grapple '{grappleName}' No Movement {nomove} | {filepath}");
                                }

                                if (version >= 7)
                                {
                                    string specObj = br.ReadString8();
                                    if (propCode == 10) CountValue(specObj, output, $"Grapple '{grappleName}' Special Object {specObj} | {filepath}");
                                }

                                if (version >= 8)
                                {
                                    bool visible = br.ReadByte() != 0;
                                    if (propCode == 11) CountValue(visible, output, $"Grapple '{grappleName}' Visible {visible} | {filepath}");
                                }

                                if (version >= 9)
                                {
                                    byte unk10 = br.ReadByte();
                                    if (propCode == 12) CountValue(unk10, output, $"Grapple '{grappleName}' Unk10 {unk10} | {filepath}");
                                }

                                if (version >= 10)
                                {
                                    string blowup = br.ReadString(16);
                                    if (propCode == 13) CountValue(blowup, output, $"Grapple '{grappleName}' Blowup {blowup} | {filepath}");
                                }

                                if (version >= 11)
                                {
                                    byte unk11 = br.ReadByte();
                                    if (propCode == 14) CountValue(unk11, output, $"Grapple '{grappleName}' Unk11 {unk11} | {filepath}");
                                }
                            }
                            break;
                        case "TightRope": 
                            if (mode == GizAnalysisMode.One && gizCode != 9) break;
                            //code
                            break;
                        case "Ledge": 
                            if (mode == GizAnalysisMode.One && gizCode != 10) break;
                            //code
                            break;
                        case "Lever": 
                            if (mode == GizAnalysisMode.One && gizCode != 11) break;
                            //code
                            version = br.ReadInt32();
                            gizCount = br.ReadInt32();
                            for (int i = 0; i < gizCount; i++)
                            {
                                string leverName = br.ReadString(16);
                                if (propCode == 2) CountValue(leverName, output, $"Lever '{leverName}' | {filepath}");
                                Vector3 leverPos = br.ReadVector3();
                                if (propCode == 3) CountValue(leverPos, output, $"Lever '{leverName}' Position {leverPos} | {filepath}");
                                short leverAng = br.ReadInt16();
                                if (propCode == 4) CountValue(leverAng, output, $"Lever '{leverName}' Angle {leverAng} | {filepath}");
                                char leverCol = (char)br.ReadByte();
                                if (propCode == 5) CountValue(leverCol, output, $"Lever '{leverName}' Color {leverCol} | {filepath}");

                                if (version >= 2)
                                {
                                    bool multi = br.ReadByte() != 0;
                                    if (propCode == 6) CountValue(multi, output, $"Lever '{leverName}' Multi-Pulls {multi} | {filepath}");
                                }

                                if (version >= 3)
                                {
                                    float time = br.ReadSingle();
                                    if (propCode == 7) CountValue(time, output, $"Lever '{leverName}' Pull Time {time} | {filepath}");
                                }

                                if (version >= 4)
                                {
                                    bool invis = br.ReadByte() != 0;
                                    if (propCode == 8) CountValue(invis, output, $"Lever '{leverName}' Invisible {invis} | {filepath}");
                                }

                                if (version >= 5)
                                {
                                    Vector3 targetPos = br.ReadVector3();
                                    float targetSize = br.ReadSingle();
                                    if (propCode == 9) CountValue(targetPos, output, $"Lever '{leverName}' Target Pos {targetPos} | {filepath}");
                                    if (propCode == 10) CountValue(targetSize, output, $"Lever '{leverName}' Target Size {targetSize} | {filepath}");
                                }

                                if (version >= 6)
                                {
                                    bool targetInvis = br.ReadByte() != 0;
                                    if (propCode == 11) CountValue(targetInvis, output, $"Lever '{leverName}' Target Invis {targetInvis} | {filepath}");
                                }

                                if (version >= 7)
                                {
                                    string specObj = br.ReadString8();
                                    if (propCode == 12) CountValue(specObj, output, $"Lever '{leverName}' Special Object {specObj} | {filepath}");
                                }

                                if (version >= 8)
                                {
                                    byte unk2 = br.ReadByte();
                                    if (propCode == 13) CountValue(unk2, output, $"Lever '{leverName}' Unk2 {unk2} | {filepath}");
                                }

                                if (version >= 9)
                                {
                                    byte unk3 = br.ReadByte();
                                    if (propCode == 14) CountValue(unk3, output, $"Lever '{leverName}' Unk3 {unk3} | {filepath}");
                                    byte unk4 = br.ReadByte();
                                    if (propCode == 15) CountValue(unk4, output, $"Lever '{leverName}' Unk4 {unk4} | {filepath}");
                                }
                            }
                            break;
                        case "Spinner": 
                            if (mode == GizAnalysisMode.One && gizCode != 12) break;
                            //code
                            break;
                        case "Techno": 
                            if (mode == GizAnalysisMode.One && gizCode != 13) break;
                            //code
                            break;
                        case "SecurityDoor": 
                            if (mode == GizAnalysisMode.One && gizCode != 14) break;
                            //code
                            break;
                        case "Attracto": 
                            if (mode == GizAnalysisMode.One && gizCode != 15) break;
                            //code
                            break;
                        case "MiniCut": 
                            if (mode == GizAnalysisMode.One && gizCode != 16) break;
                            //code
                            break;
                        case "Tube": 
                            if (mode == GizAnalysisMode.One && gizCode != 17) break;
                            //code
                            version = br.ReadInt32();
                            gizCount = br.ReadInt32();
                            for (int i = 0; i < gizCount; i++)
                            {
                                string tubeName = br.ReadString(16);
                                if (propCode == 2) CountValue(tubeName, output, $"Tube '{tubeName}' | {filepath}");
                                Vector3 tubePos = br.ReadVector3();
                                if (propCode == 3) CountValue(tubePos, output, $"Tube '{tubeName}' Pos {tubePos} | {filepath}");
                                float h = br.ReadSingle();
                                if (propCode == 4) CountValue(h, output, $"Tube '{tubeName}' Height {h} | {filepath}");
                                float r = br.ReadSingle();
                                if (propCode == 5) CountValue(r, output, $"Tube '{tubeName}' Radius {r} | {filepath}");
                                if (version >= 2) 
                                { 
                                    bool magnetic = br.ReadByte() != 0;
                                    if (propCode == 6) CountValue(magnetic, output, $"Tube '{tubeName}' Magnetic {(magnetic ? "true" : "false")} | {filepath}");
                                }
                                if (version >= 3) 
                                { 
                                    byte specObjLen = br.ReadByte();
                                    string specObj = br.ReadString(specObjLen);
                                    if (propCode == 7) CountValue(specObj, output, $"Tube '{tubeName}' Special Object {specObj} | {filepath}");
                                }
                                if (version >= 4) 
                                { 
                                    bool glide = br.ReadByte() != 0;
                                    if (propCode == 8) CountValue(glide, output, $"Tube '{tubeName}' Glide Only {(glide ? "true" : "false")} | {filepath}");
                                }
                                if (version >= 5) 
                                { 
                                    bool hoz = br.ReadByte() != 0;
                                    if (propCode == 9) CountValue(hoz, output, $"Tube '{tubeName}' Horizontal {(hoz ? "true" : "false")} | {filepath}");
                                    float angle = br.ReadSingle();
                                    if (propCode == 10) CountValue(angle, output, $"Tube '{tubeName}' Angle {angle} | {filepath}");
                                }
                            }
                            break;
                        case "ZipUp": 
                            if (mode == GizAnalysisMode.One && gizCode != 18) break;
                            //code
                            break;
                        case "Whipper": 
                            if (mode == GizAnalysisMode.One && gizCode != 19) break;
                            //code
                            break;
                        case "GizTurret": 
                            if (mode == GizAnalysisMode.One && gizCode != 20) break;
                            //code
                            break;
                        case "BombGenerator": 
                            if (mode == GizAnalysisMode.One && gizCode != 21) break;
                            //code
                            break;
                        case "Panel": 
                            if (mode == GizAnalysisMode.One && gizCode != 22) break;
                            //code
                            break;
                        case "HatMachine": 
                            if (mode == GizAnalysisMode.One && gizCode != 23) break;
                            //code
                            break;
                        case "Plug": 
                            if (mode == GizAnalysisMode.One && gizCode != 24) break;
                            //code
                            break;
                        case "PushBlocks": 
                            if (mode == GizAnalysisMode.One && gizCode != 25) break;
                            //code
                            break;
                        case "Torp Machine": 
                            if (mode == GizAnalysisMode.One && gizCode != 26) break;
                            //code
                            break;
                        case "ShadowEditor": 
                            if (mode == GizAnalysisMode.One && gizCode != 27) break;
                            //code
                            break;
                        case "Teleport": 
                            if (mode == GizAnalysisMode.One && gizCode != 28) break;
                            //code
                            break;
                        case "Puzzle": 
                            if (mode == GizAnalysisMode.One && gizCode != 29) break;
                            //code
                            break;
                        case "GizFlock": 
                            if (mode == GizAnalysisMode.One && gizCode != 30) break;
                            //code
                            break;
                        default: 
                            Debug.LogWarning($"Unknown Gizmo Section '{id}' at index {br.BaseStream.Position}");
                            break;
                    }
                    if (gizCode == GetGizmoIDIndex(id))
                    {
                        if (propCode == 0) output.AppendLine($"Gizmo Version: {version}");
                        else if (propCode == 1) output.AppendLine($"Gizmo Count: {gizCount}");
                    }

                    br.BaseStream.Position = startPos + sectionLen;
                }
            }

            //Analyse all giz files
            StringBuilder analysisOutput = new("Analysis Results:\n");
            foreach(var path in files)
            {
                BinaryReader br = null;
                try
                {
                    //Open and analyse giz file
                    br = new(File.OpenRead(path));
                    AnalyseGizFile(br, path, analysisOutput);
                }
                catch(IOException ioe)
                {
                    Debug.LogError($"IO Error analysing Gizmo Files: {path} | Mode:{mode} Code:{code}\n{ioe}");
                }
                finally
                {
                    br?.Close();
                }
            }
            Debug.Log(analysisOutput.ToString());

            StringBuilder comparisonOutput = new($"Compared Gizmo Values ({GizmoIDs[gizCode]} - {propCode}):\n");
            foreach (var pair in valueCounts.OrderBy(p => -p.Value)) 
                comparisonOutput.Append($"[Value: {pair.Key} | Count: {pair.Value}], ");
            Debug.Log(comparisonOutput.ToString());
        }
    }
}
#endif