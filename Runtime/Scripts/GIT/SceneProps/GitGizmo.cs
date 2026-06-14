#if UNITY_EDITOR
using UnityEngine;

namespace TTModdingKit.GizFlow
{
    using Gizmos;
    public class GitGizmo : MonoBehaviour
    {
        public Gizmo connectedGizmo;
        public string nodeName;
        public bool startInvisible, revInvis, outputOnly, finishedInvisible, finishedDeactive, reverse, notFreeplay, notStoryMode;
        //public float timer, randomOuptutChance, randomTime;
        //public int numRandomOutputs;

        private void OnDrawGizmosSelected()
        {
            if (connectedGizmo == null) return;
        }
    }
}
#endif