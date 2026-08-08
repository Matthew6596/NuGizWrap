#if UNITY_EDITOR
using UnityEngine;
using System.Linq;

namespace NuGizWrap.GizFlow
{
    using Gizmos;
    using Helper;

    public class GitGizmo : MonoBehaviour
    {
        public Gizmo connectedGizmo;
        public string connectedGizmoName;
        public string connectedGizmoType;
        public string nodeName;
        public bool startInvisible, revInvis, outputOnly, finishedInvisible, finishedDeactive, reverse, notFreeplay, notStoryMode;
        public float timer, randomOutputChance, randomTime;
        public int numRandomOutputs;

        private void OnDrawGizmosSelected()
        {
            if (connectedGizmo == null) return;
        }

        public static GitGizmo Load(string[] lines, ref int index)
        {
            index++; //"Gizmo {"

            string type = lines[index].ParseQuotedString();
            index++;
            string gizName = lines[index].ParseQuotedString();
            index++;

            var gizType = (type) switch
            {
                "GizObstacle" => typeof(GizObstacle),
                "GizBuildit" => typeof(GizBuildit),
                "GizForce" => typeof(GizForce),
                "blowup" => typeof(Blowup),
                "GizmoPickup" => typeof(GizmoPickup),
                "Lever" => typeof(Lever),
                "Spinner" => typeof(Spinner),
                "MiniCut" => typeof(MiniCut),
                "Tube" => typeof(Tube),
                "ZipUp" => typeof(ZipUp),
                "GizTurret" => typeof(GizTurret),
                "BombGenerator" => typeof(BombGenerator),
                "NuSpecial" => typeof(GameScene.SpecialObject),
                "Panel" => typeof(Panel),
                "HatMachine" => typeof(HatMachine),
                "PushBlocks" => typeof(PushBlocks),
                "Door" => typeof(GameScene.Door),
                "Teleport" => typeof(Teleport),
                "Torp Machine" => typeof(TorpMachine),
                "ShadowEditor" => typeof(ShadowEditor),
                "Portal" => typeof(GameScene.Portal),
                "Grapple" => typeof(Grapple),
                "Plug" => typeof(Plug),
                "Techno" => typeof(Techno),
                "GizDig" => typeof(GizDig),
                "Ledge" => typeof(Ledge),
                "SecurityDoor" => typeof(SecurityDoor),
                "Whipper" => typeof(Whipper),
                "Puzzle" => typeof(Puzzle),
                "GizFlock" => typeof(GizFlock),
                "Shard" => typeof(Shard),
                "Signal" => typeof(Signal),
                "TightRope" => typeof(TightRope),
                "Attracto" => typeof(Attracto),
                "GizTimer" => typeof(GizTimer),
                "Message" => typeof(AI.AIMessage),
                "AIProcessor" => typeof(AI.AIProcessor),
                _ => typeof(Gizmo)
            };
            Gizmo gizmo = FindObjectsByType(gizType, FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Where(g=>g.name.Trim() == gizName.Trim()).FirstOrDefault() as Gizmo;

            if (gizmo == null && gizType == typeof(PushBlocks))
            {
                gizmo = FindObjectsByType<PushBlocks>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Where(g => g!=null && g.specialObject.specialObject != null && g.specialObject.specialObject.Trim() == gizName.Trim()).FirstOrDefault();
            }

            GitGizmo gitGiz = null;
            if (gizmo == null)
            {
                GameObject ga = new(gizName, gizType);
                ga.transform.SetParent(GitManager.Instance.transform);
                gitGiz = ga.AddComponent<GitGizmo>();
                gitGiz.connectedGizmo = ga.GetComponent(gizType) as Gizmo;
                gitGiz.connectedGizmoName = gizName;
            }
            else
            {
                if (!gizmo.TryGetComponent(out gitGiz))
                {
                    gitGiz = gizmo.gameObject.AddComponent<GitGizmo>();
                    gitGiz.connectedGizmo = gizmo;
                    gitGiz.connectedGizmoName = gizName;
                }
            }
            gitGiz.connectedGizmoType = type;

            while (index < lines.Length && !lines[index].Contains('}'))
            {
                index++;
            }
            return gitGiz;
        }
    }
}
#endif