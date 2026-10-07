#if UNITY_EDITOR
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace NuGizWrap.Gizmos
{
    using Helper;

    [ExecuteInEditMode]
    public class GizmoConfig : MonoBehaviour
    {
        public static string[] GizmoNames => new[]
        {
            "GizObstacle", "GizBuildit", "GizForce", "blowup", "GizDig", "GizmoPickup",
            "Shard", "Signal", "Grapple", "TightRope", "Ledge", "Lever", "Spinner",
            "Techno", "SecurityDoor", "Attracto", "MiniCut", "Tube", "ZipUp", "Whipper",
            "GizTurret", "BombGenerator", "Panel", "HatMachine", "Plug", "PushBlocks",
            "Torp Machine", "ShadowEditor", "Teleport", "Puzzle", "GizFlock"
        };

        /*private static GizmoConfig _instance;
        public static GizmoConfig Instance
        {
            get
            {
                if(_instance == null)
                {
                    var configs = FindObjectsByType<GizmoConfig>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Where(c => c.activeInstance);
                    _instance = configs.FirstOrDefault();
                    if(configs.Count() > 1)
                    {
                        Debug.LogWarning($"Multiple Active Instances of GizmoConfig found, disabling all but the first ({_instance.name}).");
                        foreach(var c in configs) if(c != _instance) c.activeInstance = false;
                    }
                    if(_instance == null)
                    {
                        _instance = new GameObject("Gizmo Config").AddComponent<GizmoConfig>();
                    }
                    _instance.activeInstance = true;
                }
                return _instance;
            }
            private set { _instance = value; }
        }*/

        public static GizmoConfig Instance { 
            get
            {
                GizmoConfig _instance;
                var configs = FindObjectsByType<GizmoConfig>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Where(c => c.activeInstance);
                _instance = configs.FirstOrDefault();
                if (configs.Count() > 1)
                {
                    Debug.LogWarning($"Multiple Active Instances of GizmoConfig found, disabling all but {_instance.name}.");
                    foreach (var c in configs) if (c != _instance) c.activeInstance = false;
                }
                if (_instance == null)
                {
                    _instance = new GameObject("Gizmo Config").AddComponent<GizmoConfig>();
                }
                return _instance;
            } 
        }

        public bool activeInstance = false;

        public GizObstacleConfig gizObstacle;
        public GizBuilditConfig gizBuildit;
        public GizForceConfig gizForce;
        public BlowupConfig blowup;
        public GizDigConfig gizDig;
        public GizmoPickupConfig gizmoPickup;
        public ShardConfig shard;
        public SignalConfig signal;
        public GrappleConfig grapple;
        public TightRopeConfig tightRope;
        public LedgeConfig ledge;
        public LeverConfig lever;
        public SpinnerConfig spinner;
        public TechnoConfig techno;
        public SecurityDoorConfig securityDoor;
        public AttractoConfig attracto;
        public MiniCutConfig miniCut;
        public TubeConfig tube;
        public ZipUpConfig zipUp;
        public WhipperConfig whipper;
        public GizTurretConfig gizTurret;
        public BombGeneratorConfig bombGenerator;
        public PanelConfig panel;
        public HatMachineConfig hatMachine;
        public PlugConfig plug;
        public PushBlocksConfig pushBlocks;
        public TorpMachineConfig torpMachine;
        public ShadowEditorConfig shadowEditor;
        public TeleportConfig teleport;
        public PuzzleConfig puzzle;
        public GizFlockConfig gizFlock;

        private void Awake()
        {
            if (!activeInstance) return;
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"Instance of GizmoConfig already exists, setting Active Instance property on {name} to false.");
                activeInstance = false;
                return;
            }
            //Instance = this;
        }

        public GizmoTypeConfig[] GetGizmoConfigs() => new GizmoTypeConfig[]
        {
            gizObstacle, gizBuildit, gizForce, blowup, gizDig, gizmoPickup, shard, signal,
            grapple, tightRope, ledge, lever, spinner, techno, securityDoor, attracto,
            miniCut, tube, zipUp, whipper, gizTurret, bombGenerator, panel, hatMachine,
            plug, pushBlocks, torpMachine, shadowEditor, teleport, puzzle, gizFlock
        };

        public GizmoTypeConfig GetConfigByName(string name) => GetGizmoConfigs().Where(c=>c.ID == name).FirstOrDefault();

        public void ToBytes(BinaryWriter bw, ExportSettings exportSettings)
        {
            var configs = GetGizmoConfigs();

            string[] ignoreIDs = exportSettings.ignoredGizmos;
            foreach(var config in configs)
            {
                if (config == null || ignoreIDs.Contains(config.ID)) continue;
                bw.WriteString32(config.ID, terminate: false);

                long startPos = bw.Pos();
                bw.Write(0); //temp size

                config.Save(bw);
                long endPos = bw.Pos();
                bw.Pos(startPos);
                bw.Write((int)(endPos - startPos - 4));
                bw.Pos(endPos);
            }
        }

        [Serializable]
        public struct ExportSettings
        {
            public string[] ignoredGizmos;

            public ExportSettings(params string[] ignoredGizmos)
            {
                this.ignoredGizmos = ignoredGizmos;
            }

            public static ExportSettings Default => new(new string[0]);
        }
    }
}
#endif