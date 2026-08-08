#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NuGizWrap
{
    using Audio;
    using Helper;

    public static class AudioExporter
    {
        [MenuItem("Nu Giz Wrap/Export/Audio")]
        public static void Export()
        {
            string path = EditorUtility.SaveFolderPanel("Export Audio Files", "", "");
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;

            Export(path, true);
        }

        public static void Export(string path, bool notify = false)
        {
            EditorUtility.DisplayProgressBar("Exporting", $"Exporting Audio...", 0);

            try
            {
                File.WriteAllLines(Path.Combine(path,"AUDIO.CFG"),AudioConfig.ToLines());
                //File.WriteAllLines(Path.Combine(path,"MUSIC.CFG"),MusicConfig.ToLines());
            }catch(IOException ioe)
            {
                Error(ioe.Message);
                return;
            }

            EditorUtility.ClearProgressBar();
            if (notify) EditorUtility.DisplayDialog("Audio Exported!", $"Successfully exported Audio to '{path}'", "OK");
        }

        private static void Error(string msg) => TTLevelEditor.Error(msg);

        public static string[] GetAllSampleNames()
        {
            var scene = TTUnityProject.GetScene("PROJECT.unity");
            var samples = scene.FindAllInScene<Sample>().ToArray();
            var sampleNames = new string[samples.Length];
            for (int i = 0; i<samples.Length; i++) sampleNames[i] = samples[i].name;
            TTUnityProject.CloseScene(scene);
            return sampleNames;
        }

        /*public static List<AudioClip> GetAllAudioClipsInScene()
        {
            var clips = new List<AudioClip>();

            foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                foreach (var field in mb.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (field.FieldType == typeof(AudioClip))
                    {
                        if (field.GetValue(mb) is AudioClip clip)
                            clips.Add(clip);
                    }
                    else if (field.FieldType == typeof(AudioClip[]))
                    {
                        if (field.GetValue(mb) is AudioClip[] arr)
                            clips.AddRange(arr.Where(c => c != null));
                    }
                    else if (field.FieldType == typeof(List<AudioClip>))
                    {
                        if (field.GetValue(mb) is List<AudioClip> list)
                            clips.AddRange(list.Where(c => c != null));
                    }
                }
            }

            return clips.Distinct().ToList();
        }*/
    }
}
#endif