#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace NuGizWrap.Audio
{
    using Helper;

    public class AudioConfig : MonoBehaviour
    {
        public static AudioConfig Instance {get; private set;}

        public string version = "1.00";

        private void OnValidate()
        {
            if (Instance == null) Instance = this;
            else if (this != Instance)
            {
                EditorUtility.DisplayDialog($"Cannot Create AudioConfig", $"There can only be one instance of AudioConfig, and there is already one on GameObject '{Instance.gameObject.name}'", "OK");
                this.DelayDestroy();
            }
        }

        [ContextMenu("Export AUDIO.CFG to mod")]
        public void Export()
        {
            string path = EditorUtility.SaveFolderPanel("Select the 'Audio' folder in your mod", "", "");
            string fpath = Path.Combine(path, "AUDIO.CFG");
            if (string.IsNullOrEmpty(path) || !File.Exists(fpath)) return;

            try
            {
                File.WriteAllLines(Path.Combine(path, "AUDIO.CFG"), ToLines());
            }
            catch(IOException ioe)
            {
                EditorUtility.DisplayDialog("Audio.CFG Export Error", ioe.Message, "OK");
                return;
            }

            EditorUtility.DisplayDialog("Audio Exported!", $"Successfully exported Audio to '{path}'", "OK");
        }

        [ContextMenu("Load AUDIO.CFG from mod")]
        public void Load()
        {
            string path = EditorUtility.SaveFolderPanel("Select the 'Audio' folder in your mod", "", "");
            string fpath = Path.Combine(path, "AUDIO.CFG");
            if (string.IsNullOrEmpty(path) || !File.Exists(fpath)) return;

            string[] lines = new string[0];
            try
            {
                lines = File.ReadAllLines(fpath);
            }
            catch (IOException ioe)
            {
                EditorUtility.DisplayDialog("Audio.CFG Export Error", ioe.Message, "OK");
                return;
            }

            FromLines(lines);

            EditorUtility.DisplayDialog("Audio Loaded!", $"Successfully loaded Audio on '{name}' GameObject", "OK");
        }

        public static void FromLines(string[] lines)
        {
            int index = 0;

            bool startWasRead = false;
            while (!startWasRead && index < lines.Length)
            {
                string line = lines[index].Trim();
                if (line.StartsWith("Audio"))
                {
                    string[] parts = line.Split(' ');

                    if(parts.Length >= 2) Instance.version = parts[1];
                    if(parts.Length >= 3) Debug.Log("AudioCfg number of samples noted as: " + parts[2]);

                    startWasRead = true;
                }
                index++;
            }

            //clear children objects
            Transform cfg = Instance.transform;
            for(int i=cfg.childCount-1; i>=0; i--) cfg.GetChild(i).gameObject.DelayDestroy();

            Transform parent = cfg;
            for(;index < lines.Length; index++)
            {
                string line = lines[index].Trim();

                if (line.Length == 0 || line[0] == ';') continue;
                List<string> parts = line.Replace("\"",string.Empty).Split().ToList();
                parts.RemoveAll((s) => string.IsNullOrWhiteSpace(s));

                string type = parts[0].ToLower();
                if (type == "sample")
                {
                    string name = parts[parts.IndexOf("name") + 1];
                    var obj = new GameObject(name);
                    obj.transform.parent = parent;
                    var sample = obj.AddComponent<Sample>();
                    if (sample.FromLine(parts)) parent = sample.transform;
                }
                else if(type == "group")
                {
                    if(parent.name != parts[1]) Debug.LogWarning("Group seems to have wrong group leader");
                    parent = cfg;
                }
            }

        }

        public static string[] ToLines()
        {
            List<string> lines = new();

            return lines.ToArray();
        }
    }
}
#endif