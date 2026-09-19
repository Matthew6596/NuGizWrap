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
        private static GizmoConfig _instance;
        public static GizmoConfig Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = GameObject.FindFirstObjectByType<GizmoConfig>();
                    if(_instance == null)
                    {
                        _instance = new GameObject("Gizmo Config").AddComponent<GizmoConfig>();
                    }
                }
                return _instance;
            }
            private set { _instance = value; }
        }

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
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Instance of GizmoConfig already exists, destroying old instance.");
                DestroyImmediate(Instance);
            }
            Instance = this;
        }

        public void ToBytes(BinaryWriter bw, ExportSettings exportSettings)
        {
            var configs = new GizmoTypeConfig[]
            {
                gizObstacle, gizBuildit, gizForce, blowup, gizDig, gizmoPickup, shard, signal,
                grapple, tightRope, ledge, lever, spinner, techno, securityDoor, attracto,
                miniCut, tube, zipUp, whipper, gizTurret, bombGenerator, panel, hatMachine,
                plug, pushBlocks, torpMachine, shadowEditor, teleport, puzzle, gizFlock
            };

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