#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.Events;

namespace TTModdingKit
{
    using Terrain;

    [InitializeOnLoad]
    public class TTUnityProject : ScriptableObject
    {
        public static TTGame Game => Instance.modMeta.game;
        public static Preferences Prefs => Instance.prefs;

        [Tooltip("The absolute path of your vanilla game")]
        public string vanillaGamePath;
        public bool modManagerLinked = false;
        public ModManagerSettings modManagerSettings;
        public ModMeta modMeta;
        public Preferences prefs;
        public GameScene.Things globalThings;

        [Tooltip("Relative paths of files or directories the mod should to remove from the game. Example: stuff/text/danish.txt or chars/gonkdroid")]
        public string[] removalPaths;

        //[Header("Project Paths")]
        public string audioPath = "Audio";
        public string scenesPath = "Scenes";
        public string materialsPath = "Materials";
        public string textAssetsPath = "Text";

        public static string ProjectAssetPath => Path.GetDirectoryName(AssetDatabase.GetAssetPath(Instance));
        public static string AbsoluteProjectAssetPath => ProjectAssetPath.Replace("Assets", Application.dataPath);
        public static string GetGamePath(TTGame game) => _instance.modManagerLinked ? _instance.modManagerSettings.GetGamePath(game) : "";
        public static string GetGamePath() => GetGamePath(Game);
        public static string GetDefaultFileExplorerPath() => Prefs.general.defaultFileDirectoryToCurrentGame ? Path.GetDirectoryName(TTUnityProject.GetGamePath()) : "";

        private static TTUnityProject _instance;

        public static TTUnityProject Instance
        {
            get
            {
                if (_instance == null) _instance = FindOrCreate();
                return _instance;
            }
        }

        static TTUnityProject()
        {
            //Make sure TTUnityProject is generated/exists in project
            EditorApplication.delayCall += () => { _ = Instance; };
        }

        private static Texture2D icon;
        private void OnValidate()
        {
            if (icon == null) icon = TTResourceManager.LoadEditorAsset<Texture2D>("Textures/ProjectIcon", ".png");
            if (EditorGUIUtility.GetIconForObject(Instance) != icon) EditorGUIUtility.SetIconForObject(Instance, icon);

            //Check texture
            Texture2D iconTxtr = modMeta.icon.texture;
            if (iconTxtr != null)
            {
                if (!iconTxtr.isReadable) Debug.LogError($"Texture '{iconTxtr.name}' must be Read/Write to be used as the Mod's Icon");
                if (IsTextureCompressed(iconTxtr)) Debug.LogError($"Texture '{iconTxtr.name}' must be Uncompressed to be used as the Mod's Icon");
            }

            //Check project game, load game resources
            TTGame game = Game;
            if(TTResourceManager.LoadedGame != game) TTResourceManager.LoadGameResources(game);
        }

        private static TTUnityProject FindOrCreate()
        {
            // Search anywhere in the project
            var guids = AssetDatabase.FindAssets("t:TTUnityProject");

            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<TTUnityProject>(path);
            }

            // None found — create a default one
            var config = CreateInstance<TTUnityProject>();
            AssetDatabase.CreateAsset(config, "Assets/TTUnityProject.asset");
            AssetDatabase.SaveAssets();
            return config;
        }

        /*public static T FindAsset<T>(string path) where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets($"{Path.GetFileName(path)} t:{typeof(T).Name}");

            foreach (string guid in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(guid);
                if (p.Contains("TTUnityKit/"+Path.GetDirectoryName(path).Replace('\\','/'))) return AssetDatabase.LoadAssetAtPath<T>(p);
            }
            return null;
        }*/

        public static AudioClip[] GetAllAudioClips()
        {
            List<AudioClip> clips = new();
            string path = ProjectAssetPath+"/"+Instance.audioPath;

            foreach(var guid in AssetDatabase.FindAssets("t:audioclip", new string[] { path }))
            {
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guid));
                if(clip!=null) clips.Add(clip);
            }

            return clips.ToArray();
        }

        /*public static SceneAsset GetSceneAsset(string path)
        {
            string scenesPath = ProjectAssetPath + "/" + Instance.scenesPath + "/" + path;
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(scenesPath);
        }*/

        public static Scene GetScene(string path)
        {
            string scenesPath = ProjectAssetPath + "/" + Instance.scenesPath + "/" + path;
            return EditorSceneManager.OpenScene(scenesPath, OpenSceneMode.Additive);
        }

        public static void CloseScene(Scene scene) => EditorSceneManager.CloseScene(scene, removeScene: true);

        private static bool IsTextureCompressed(Texture2D texture)
        {
            return (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter).GetDefaultPlatformTextureSettings().textureCompression != TextureImporterCompression.Uncompressed;
        }

        [Serializable]
        public struct ModManagerSettings
        {
            public string exe;
            public string dataPath;

            public Dictionary<TTGame, string> gamePaths;

            public void RefreshGamePaths()
            {
                gamePaths = new();
                string gamesFilePath = Path.Combine(dataPath, "games.txt");
                if (!File.Exists(gamesFilePath)) return;

                var lines = File.ReadAllLines(gamesFilePath);

                for(int i=0; i<lines.Length; i += 2)
                {
                    if (!Enum.TryParse<TTGame>(lines[i], out var game)) continue;

                    if (gamePaths.ContainsKey(game)) gamePaths[game] = lines[i + 1];
                    else gamePaths.Add(game, lines[i + 1]);
                }

            }

            public string GetGamePath(TTGame game)
            {
                if (gamePaths == null) RefreshGamePaths();
                if (gamePaths.ContainsKey(game)) return gamePaths[game];
                return string.Empty;
            }
        }

        [Serializable]
        public struct Preferences
        {
            //General
            [Serializable]
            public struct General
            {
                public bool defaultFileDirectoryToCurrentGame;
            }
            public General general;

            //Gizmo
            [Serializable]
            public struct Gizmo
            {
                public bool generateEmptyGizmoSections;
                public bool onlyGenerateCompatibleGizmoSections;
                public bool allowAllRegisteredGizmos;
            }
            public Gizmo gizmo;

            //Terrain
            [Serializable]
            public struct Terrain
            {
                public float terrainAlpha;
                public Dictionary<SurfaceType, Color> terrainColors;
            }
            public Terrain terrain;

            //AI2
            [Serializable]
            public struct AI2
            {
                public int version;
                public bool alwaysExportMaxVersion;
                public Color locatorColor, triggerColor;
            }
            public AI2 ai2;

            public static Preferences Default => new()
            {
                general = new() 
                { 
                    defaultFileDirectoryToCurrentGame = true 
                },

                gizmo = new()
                {
                    generateEmptyGizmoSections = false,
                    onlyGenerateCompatibleGizmoSections = true,
                    allowAllRegisteredGizmos = false,
                },

                terrain = new()
                {
                    terrainAlpha = 0.1f,
                    terrainColors = new()
                    {
                        { SurfaceType.Unknown, Color.magenta }, //meant to be temp
                        { SurfaceType.None, Color.grey },
                        { SurfaceType.Slip, Color.yellow },
                        { SurfaceType.Water, Color.blue },
                        { SurfaceType.Instakill, new Color(1f, 0f, 0.3f) },
                        { SurfaceType.Fastkill, Color.red },
                        { SurfaceType.Slowkill, new Color(0.7f, 0.4f, 0f) },
                        { SurfaceType.R2SwampWater, new Color(0.44f, 0.52f, 0f) },
                        { SurfaceType.PushblockSurface, new Color(0.3f, 0f, 0.8f) },
                        { SurfaceType.Edge, new Color(0.1f, 0.2f, 0.6f) },
                        { SurfaceType.ForceMovable, new Color(0.2f, 0.8f, 0.2f) },
                        { SurfaceType.GameMovable, new Color(0f, 0.6f, 0.5f) },
                        { SurfaceType.SpinnerSide, Color.green },
                        { SurfaceType.Ice, new Color(0.6f, 0.94f, 0.94f) },
                        { SurfaceType.MetalObject, new Color(0.55f, 0.55f, 0.55f) },
                        { SurfaceType.EnergyWall, new Color(0.25f, 0.87f, 0.87f) },
                        { SurfaceType.ReflectiveFloor, Color.black },
                        { SurfaceType.MapCustomFloor, new Color(1, 0.510f, 0.776f) },
                        { SurfaceType.Button, new Color(0.298f, 0.733f, 0.09f) },
                        { SurfaceType.StopHover, new Color(0.248f, 0.001f, 0.001f) },
                    }
                },

                ai2 = new() 
                { 
                    version = 21,
                    alwaysExportMaxVersion = true,
                    locatorColor = Color.purple,
                    triggerColor = new(1f,.647f,1f,0.1f),
                },
            };
        }
    }
}
#endif