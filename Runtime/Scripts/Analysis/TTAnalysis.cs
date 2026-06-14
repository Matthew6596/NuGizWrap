#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

namespace TTModdingKit.Analysis
{
    using Gizmos;
    using System.Linq;
    using System.Text;

    public static class TTAnalysis
    {
        public static bool Running { get; private set; } = false;

        private static string r1, r2, r3;
        private readonly static Dictionary<string, string> vars = new();
        private static readonly Stack<int> scriptStack = new();
        private static string lastLoadedFile;
        private static readonly Queue<string> fileQueue = new();
        private static Scene loadedScene;
        private static bool tempSceneLoaded;
        private static int loopCount = 0;

        [MenuItem("TT Modding/Analysis/Run Script")]
        public static void RunAnalysisScript()
        {
            if (Running) EditorUtility.DisplayDialog("Already Running", "A script is already running so another cannot be run.", "Ok");
            else if (Selection.activeObject is AnalysisScript script)
            {
                r1 = "";
                r2 = "";
                r3 = "";
                vars.Clear();
                scriptStack.Clear();
                fileQueue.Clear();
                Running = true;
                EditorApplication.delayCall += () => { RunAnalysisScript(script); };
            }
            else EditorUtility.DisplayDialog("Select Script", "Please select an AnalysisScript (ScriptableObject) in the Asset Database before selecting this menu option.", "Ok");
        }

        public static void ForceStop() => Running = false;

        private static void RunAnalysisScript(AnalysisScript script)
        {
            for (int i = 0; i < script.lines.Length; i++)
            {
                void skipTillCommand(AnalysisScript.Command cmd)
                {
                    do i++;
                    while (i < script.lines.Length && script.lines[i].command != cmd);
                }

                var line = script.lines[i];
                string param = line.parameter;

                switch (line.command)
                {
                    case AnalysisScript.Command.Comment: break;
                    case AnalysisScript.Command.Log: Debug.Log(param); break;
                    case AnalysisScript.Command.LogR1: Debug.Log($"R1: {r1} //{param}"); break;
                    case AnalysisScript.Command.LogR2: Debug.Log($"R2: {r2} //{param}"); break;
                    case AnalysisScript.Command.LogR3: Debug.Log($"R3: {r3} //{param}"); break;
                    case AnalysisScript.Command.LogVar: Debug.Log($"var '{param}': {GetVar(param)}"); break;
                    case AnalysisScript.Command.LogCurrFile: Debug.Log($"file: {lastLoadedFile} //{param}"); break;
                    case AnalysisScript.Command.SetVarWithR1: SetVar(param, r1); break;
                    case AnalysisScript.Command.SetVarWithR2: SetVar(param, r2); break;
                    case AnalysisScript.Command.SetVarWithR3: SetVar(param, r3); break;
                    case AnalysisScript.Command.IncR1: r1 = AddInt(r1, param, "1"); break;
                    case AnalysisScript.Command.IncR2: r2 = AddInt(r2, param, "1"); break;
                    case AnalysisScript.Command.IncR3: r3 = AddInt(r3, param, "1"); break;
                    case AnalysisScript.Command.GetVarInR1: r1 = GetVar(param); break;
                    case AnalysisScript.Command.GetVarInR2: r2 = GetVar(param); break;
                    case AnalysisScript.Command.GetVarInR3: r3 = GetVar(param); break;
                    case AnalysisScript.Command.EnumerateAllGIZ:
                        static void enumPaths(string path)
                        {
                            if (Directory.Exists(path))
                            {
                                foreach (var fpath in Directory.EnumerateFiles(path, "*.giz", SearchOption.AllDirectories))
                                {
                                    if (new FileInfo(fpath).Length > 0) fileQueue.Enqueue(fpath);
                                }
                            }
                        }
                        if (param == "" || param.ToLower().Contains("tcs")) enumPaths(Path.GetDirectoryName(TTUnityProject.GetGamePath(TTGame.TCS)));
                        if (param == "" || param.ToLower().Contains("lij1")) enumPaths(Path.GetDirectoryName(TTUnityProject.GetGamePath(TTGame.LIJ1)));
                        if (param == "" || param.ToLower().Contains("lb1")) enumPaths(Path.GetDirectoryName(TTUnityProject.GetGamePath(TTGame.LB1)));
                        break;
                    case AnalysisScript.Command.PopNextFileInQueue:
                        if (fileQueue.Count <= 0) break;
                        if (!tempSceneLoaded)
                        {
                            loadedScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                            tempSceneLoaded = true;
                        }

                        EditorSceneManager.SetActiveScene(loadedScene);
                        lastLoadedFile = fileQueue.Dequeue();
                        GIZImporter.Import(lastLoadedFile, notify: false);
                        break;
                    case AnalysisScript.Command.GetFileQueueCountR1: r1 = fileQueue.Count.ToString(); break;
                    case AnalysisScript.Command.StartIfR1: if (r1 != param) skipTillCommand(AnalysisScript.Command.EndIf); break;
                    case AnalysisScript.Command.StartIfR2: if (r2 != param) skipTillCommand(AnalysisScript.Command.EndIf); break;
                    case AnalysisScript.Command.StartIfR3: if (r3 != param) skipTillCommand(AnalysisScript.Command.EndIf); break;
                    case AnalysisScript.Command.EndIf: break;
                    case AnalysisScript.Command.AddR1R2ToR3:
                        if (!int.TryParse(r1, out var n1) || !int.TryParse(r2, out var n2)) r3 = r1 + r2;
                        else r3 = (n1 + n2).ToString();
                        break;
                    case AnalysisScript.Command.PropertyReadToR1:
                        if (lastLoadedFile.ToLower().Contains(".giz"))
                        {
                            string[] propPath = param.Split('/');
                            if (propPath.Length < 2) break;
                            var gizs = Object.FindObjectsByType((propPath[0].ToLower()) switch
                            {
                                "gizobstacle" => typeof(GizObstacle),
                                "gizbuildit" => typeof(GizBuildit),
                                "gizforce" => typeof(GizForce),
                                "blowup" => typeof(Blowup),
                                "gizdig" => typeof(GizDig),
                                "gizmopickup" => typeof(GizmoPickup),
                                "shard" => typeof(Shard),
                                "signal" => typeof(Signal),
                                "grapple" => typeof(Grapple),
                                "tightrope" => typeof(TightRope),
                                "ledge" => typeof(Ledge),
                                "lever" => typeof(Lever),
                                "spinner" => typeof(Spinner),
                                "securitydoor" => typeof(SecurityDoor),
                                "attracto" => typeof(Attracto),
                                "minicut" => typeof(MiniCut),
                                "tube" => typeof(Tube),
                                "zipup" => typeof(ZipUp),
                                "whipper" => typeof(Whipper),
                                "gizturret" => typeof(GizTurret),
                                "bombgenerator" => typeof(BombGenerator),
                                "panel" => typeof(Panel),
                                "hatmachine" => typeof(HatMachine),
                                "plug" => typeof(Plug),
                                "pushblocks" => typeof(PushBlocks),
                                "torp machine" => typeof(TorpMachine),
                                "shadoweditor" => typeof(ShadowEditor),
                                "teleport" => typeof(Teleport),
                                "puzzle" => typeof(Puzzle),
                                "gizflock" => typeof(GizFlock),
                                "techno" => typeof(Techno),
                                _ => typeof(GizObstacle)

                            }, FindObjectsSortMode.InstanceID).Reverse().ToArray();
                            if (gizs.Length == 0) break;
                            var prop = gizs[0].GetType().GetField(propPath[1]);
                            if (prop == null) break;

                            Dictionary<string, int> valCounts = new();
                            foreach (var giz in gizs)
                            {
                                string propVal = prop.GetValue(giz).ToString();
                                if (valCounts.ContainsKey(propVal)) valCounts[propVal]++;
                                else valCounts.Add(propVal, 1);
                            }

                            StringBuilder sb = new();
                            foreach (var k in valCounts.Keys) sb.Append($"{k} x{valCounts[k]}, ");
                            r1 = sb.ToString();
                            if (r1.Length > 0) r1 = r1[..^2];
                        }
                        break;
                    case AnalysisScript.Command.StartLoop:
                        scriptStack.Push(i);
                        if (param != "")
                        {
                            if (loopCount != 0) break;
                            if (int.TryParse(param, out int loopN1)) loopCount = loopN1;
                            else if (param.ToLower()[0] == 'r') loopCount = int.Parse((param[1]) switch { '1' => r1, '2' => r2, '3' => r3, _ => r1 });
                            else if (vars.ContainsKey(param)) loopCount = int.Parse(vars[param]);
                        }
                        else
                        {
                            //while loop
                        }
                        break;
                    case AnalysisScript.Command.EndLoop:
                        loopCount--;
                        if (loopCount > 0) i = scriptStack.Pop() - 1;
                        break;
                    default: break;
                }
            }

            if (tempSceneLoaded)
            {
                EditorSceneManager.CloseScene(loadedScene, true);
                tempSceneLoaded = false;
            }
            Running = false;

            if (script.alertFinish) EditorUtility.DisplayDialog("Script Finished", $"The analysis script '{script.name}' has finished", "Ok");
        }

        private static void SetVar(string varName, string value)
        {
            if (vars.ContainsKey(varName)) vars[varName] = value;
            else vars.Add(varName, value);
        }

        private static string GetVar(string varName) => vars.ContainsKey(varName) ? vars[varName] : "";

        private static string AddInt(string s1, string s2, string defaultStr = "0")
        {
            if (s1 == "") s1 = defaultStr;
            if (s2 == "") s2 = defaultStr;
            if (!int.TryParse(s1, out int n1) || !int.TryParse(s2, out int n2)) return s1 + s2;
            else return (n1 + n2).ToString();
        }

        private static int ParseStr(string str, int defaultVal = 0) => int.TryParse(str, out var val) ? val : defaultVal;

        [MenuItem("TT Modding/Analysis/Validate Read TCS")]
        public static void ValidateReadTCS() => ValidateRead(TTGame.TCS);

        [MenuItem("TT Modding/Analysis/Validate Read LIJ1")]
        public static void ValidateReadLIJ1() => ValidateRead(TTGame.LIJ1);

        [MenuItem("TT Modding/Analysis/Validate Read LB1")]
        public static void ValidateReadLB1() => ValidateRead(TTGame.LB1);

        private static void ValidateRead(TTGame game)
        {
            tempSceneLoaded = false;

            string path = Path.GetDirectoryName(TTUnityProject.GetGamePath(game));
            if (!Directory.Exists(path))
            {
                Debug.LogError($"Directory for game {game} could not be found ({path})");
                return;
            }

            string[] files = Directory.EnumerateFiles(path, "*.giz", SearchOption.AllDirectories).ToArray();
            int fileIndex = 0;

            void validateFile()
            {
                if (fileIndex >= files.Length) return;

                string fpath = files[fileIndex];
                if (Path.GetFileName(fpath).ToLower() == "plopsarlaccpit_c.giz") return;
                if (new FileInfo(fpath).Length <= 0)
                {
                    fileIndex++;
                    validateFile();
                }

                if (!tempSceneLoaded)
                {
                    EditorSceneManager.CloseScene(loadedScene, true);

                    loadedScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    loadedScene.name = Path.GetFileName(fpath);
                    //tempSceneLoaded = true;
                }
                EditorSceneManager.SetActiveScene(loadedScene);

                //Load file
                GIZImporter.Import(fpath, notify: false);

                //Export file
                /*string tempPath = Path.Combine(Path.GetTempPath(),"tempgiz.tmp");
                GIZExporter.Export(tempPath, notify: false);

                //Reload to ensure export worked
                GIZImporter.Import(tempPath, notify: false);*/

                EditorApplication.delayCall += () =>
                {
                    fileIndex++;
                    validateFile();
                };
            }

            validateFile();
            //Debug.Log("done with validation");
            if (tempSceneLoaded)
            {
                EditorSceneManager.CloseScene(loadedScene, true);
                tempSceneLoaded = false;
            }
        }
    }
}
#endif