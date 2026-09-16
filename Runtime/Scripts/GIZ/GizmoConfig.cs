#if UNITY_EDITOR
using UnityEngine;

namespace NuGizWrap.Gizmos
{
    [ExecuteInEditMode]
    public class GizmoConfig : MonoBehaviour
    {
        public static GizmoConfig Instance { get; private set; }

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
            if (Instance != null)
            {
                Debug.LogWarning("Instance of GizmoConfig already exists, destroying old instance.");
                Destroy(Instance);
            }
            Instance = this;
        }
    }
}
#endif