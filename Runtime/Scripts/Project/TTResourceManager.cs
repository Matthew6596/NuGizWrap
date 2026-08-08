#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NuGizWrap
{
    //[InitializeOnLoad]
    public static class TTResourceManager
    {
        public static string PackagePath => "Packages/com.mattonmat.nugizwrap";
        public static TTGame LoadedGame {get; private set;}

        /*static TTResourceManager()
        {
            //EditorGUIUtility.Icon
            EditorGUIUtility.SetIconForObject()
        }*/

        public static void LoadGameResources(TTGame game)
        {
            UnloadResources();
            LoadedGame = game;
        }

        public static void UnloadResources()
        {

        }

        public static void SaveResources()
        {

        }

        /// <summary>
        /// Loads an asset from the TT Modding Kit package Assets folder.
        /// </summary>
        /// <typeparam name="T">The type of object to load</typeparam>
        /// <param name="path">The relative path to the package's Assets folder. Example: "Models/cylinder"</param>
        /// <param name="ext">The extension of the file to load with '.'. Example: ".mesh"</param>
        /// <returns></returns>
        public static T LoadEditorAsset<T>(string path, string ext) where T : Object => AssetDatabase.LoadAssetAtPath<T>($"{PackagePath}/Assets/{path}{ext}");
    }
}
#endif