#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using System.IO;
using Obj = UnityEngine.Object;

namespace NuGizWrap.Gizmos
{
    using Helper;
    using System;

    [ScriptedImporter(1, "giz")]
    public class GIZAssetImporter : ScriptedImporter
    {
        [Tooltip("Whether the game should be automatically detected while importing.")]
        public bool autoDetectGame = true;

        [Tooltip("The game that this file is from.")]
        public TTGame game;

        [Tooltip("Generates objects for gizmos, even if there are no instances of that gizmo.")]
        public bool generateEmptyBlocks;

        [Tooltip("Generates objects for all 31 gizmo types, regardless of whether they are compatible with the origin game.")]
        public bool generateAllGizmoTypeBlocks;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            using var br = new BinaryReader(File.OpenRead(ctx.assetPath));

            //Checking the file's size & magic
            long fileSize = br.BaseStream.Length;
            if(fileSize < 4)
            {
                Debug.LogError("Cannot import empty .giz file.");
                return;
            }
            else if (br.ReadInt32() != 1)
            {
                Debug.LogError("Couldn't import .giz, the file must have a magic of 1");
                return;
            }

            //Auto-detecting the file's game.
            if (autoDetectGame)
            {
                br.BaseStream.Seek(15, SeekOrigin.Current);
                br.BaseStream.Seek(br.ReadInt32(), SeekOrigin.Current); //skip GizObstacle
                br.BaseStream.Seek(14, SeekOrigin.Current);
                br.BaseStream.Seek(br.ReadInt32(), SeekOrigin.Current); //skip GizBuildit

                int nextSectionSize = br.ReadInt32();
                if(nextSectionSize == 8) //GizForce (TCS only)
                {
                    game = TTGame.TCS;
                }
                else
                {
                    br.BaseStream.Seek(nextSectionSize, SeekOrigin.Current);
                    br.BaseStream.Seek(br.ReadInt32(), SeekOrigin.Current); //skip blowup

                    nextSectionSize = br.ReadInt32();
                    if(nextSectionSize == 6) //GizDig (LIJ1 only)
                    {
                        game = TTGame.LIJ1;
                    }
                    else
                    {
                        game = TTGame.LB1;
                    }
                }
            }

            //Importing the file's data.
            br.BaseStream.Position = 4;
            GameObject baseSceneObj = new("Gizmos");
            Transform baseSceneTransform = baseSceneObj.transform;
            var config = baseSceneObj.AddComponent<GizmoConfig>();
            ctx.AddObjectToAsset("Gizmos Scene Object", baseSceneObj);

            //Read Gizmo Blocks
            int blockNameLen = 0;
            while (
                br.Pos() < fileSize - 4 &&
                (blockNameLen = br.ReadInt32()) > 0
                )
            {
                var blockName = br.ReadString(blockNameLen);
                int sectionLen = br.ReadInt32();

                long nextSectionPos = br.Pos() + sectionLen;

                //Get Gizmo Type Config
                var gizmoConfig = (blockName) switch
                {
                    "GizObstacle" => CreateGizmoConfig<GizObstacleConfig>((t) => config.gizObstacle = t),
                    "GizBuildit" => CreateGizmoConfig<GizBuilditConfig>((t) => config.gizBuildit = t),
                    "GizForce" => CreateGizmoConfig<GizForceConfig>((t) => config.gizForce = t),
                    "blowup" => CreateGizmoConfig<BlowupConfig>((t) => config.blowup = t),
                    "GizDig" => CreateGizmoConfig<GizDigConfig>((t) => config.gizDig = t),
                    "GizmoPickup" => CreateGizmoConfig<GizmoPickupConfig>((t) => config.gizmoPickup = t),
                    "Shard" => CreateGizmoConfig<ShardConfig>((t) => config.shard = t),
                    "Signal" => CreateGizmoConfig<SignalConfig>((t) => config.signal = t),
                    "Grapple" => CreateGizmoConfig<GrappleConfig>((t) => config.grapple = t),
                    "TightRope" => CreateGizmoConfig<TightRopeConfig>((t) => config.tightRope = t),
                    "Ledge" => CreateGizmoConfig<LedgeConfig>((t) => config.ledge = t),
                    "Lever" => CreateGizmoConfig<LeverConfig>((t) => config.lever = t),
                    "Spinner" => CreateGizmoConfig<SpinnerConfig>((t) => config.spinner = t),
                    "Techno" => CreateGizmoConfig<TechnoConfig>((t) => config.techno = t),
                    "SecurityDoor" => CreateGizmoConfig<SecurityDoorConfig>((t) => config.securityDoor = t),
                    "Attracto" => CreateGizmoConfig<AttractoConfig>((t) => config.attracto = t),
                    "MiniCut" => CreateGizmoConfig<MiniCutConfig>((t) => config.miniCut = t),
                    "Tube" => CreateGizmoConfig<TubeConfig>((t) => config.tube = t),
                    "ZipUp" => CreateGizmoConfig<ZipUpConfig>((t) => config.zipUp = t),
                    "Whipper" => CreateGizmoConfig<WhipperConfig>((t) => config.whipper = t),
                    "GizTurret" => CreateGizmoConfig<GizTurretConfig>((t) => config.gizTurret = t),
                    "BombGenerator" => CreateGizmoConfig<BombGeneratorConfig>((t) => config.bombGenerator = t),
                    "Panel" => CreateGizmoConfig<PanelConfig>((t) => config.panel = t),
                    "HatMachine" => CreateGizmoConfig<HatMachineConfig>((t) => config.hatMachine = t),
                    "Plug" => CreateGizmoConfig<PlugConfig>((t) => config.plug = t),
                    "PushBlocks" => CreateGizmoConfig<PushBlocksConfig>((t) => config.pushBlocks = t),
                    "Torp Machine" => CreateGizmoConfig<TorpMachineConfig>((t) => config.torpMachine = t),
                    "ShadowEditor" => CreateGizmoConfig<ShadowEditorConfig>((t) => config.shadowEditor = t),
                    "Teleport" => CreateGizmoConfig<TeleportConfig>((t) => config.teleport = t),
                    "Puzzle" => CreateGizmoConfig<PuzzleConfig>((t) => config.puzzle = t),
                    "GizFlock" => CreateGizmoConfig<GizFlockConfig>((t) => config.gizFlock = t),
                    _ => null
                };

                //Create Scene Objects
                if (gizmoConfig != null)
                {
                    gizmoConfig.name = $"{blockName} Config";
                    GameObject gizmoParent = new($"{ObjectNames.NicifyVariableName(blockName)} Block");
                    int gizmoInstanceCount = 0;
                    if (sectionLen > 0) gizmoInstanceCount = gizmoConfig.Load(br, gizmoParent.transform);

                    if (generateEmptyBlocks || gizmoInstanceCount > 0)
                    {
                        gizmoParent.transform.SetParent(baseSceneTransform);
                        ctx.AddObjectToAsset(gizmoConfig.name, gizmoConfig);
                    }
                    else
                    {
                        gizmoParent.DelayDestroy();
                    }
                }

                br.BaseStream.Position = nextSectionPos;
            }
        }

        private GizmoTypeConfig CreateGizmoConfig<T>(Action<T> setBaseConfig) where T : GizmoTypeConfig
        {
            var config = ScriptableObject.CreateInstance<T>();
            setBaseConfig(config);
            return (generateAllGizmoTypeBlocks || config.IsGameCompatible(game)) ? config : null;
        }

    }
}
#endif