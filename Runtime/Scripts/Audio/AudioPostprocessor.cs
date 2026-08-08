#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace NuGizWrap.Audio
{
    public class AudioPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessAudio()
        {
            if (string.IsNullOrEmpty(assetImporter.userData)) assetImporter.userData = "";
        }
    }

    public static class AudioClipMetaData
    {
        public static string GetTTPath(this AudioClip clip)
        {
            var path = AssetDatabase.GetAssetPath(clip);
            return AssetImporter.GetAtPath(path)?.userData ?? "";
        }

        public static void SetTTPath(this AudioClip clip, string fpath)
        {
            var path = AssetDatabase.GetAssetPath(clip);
            var importer = AssetImporter.GetAtPath(path);
            if (importer == null) return;
            importer.userData = fpath;
            AssetDatabase.WriteImportSettingsIfDirty(path);
        }
    }
}
#endif