#if UNITY_EDITOR
using UnityEngine;

namespace TTModdingKit.GizFlow
{
    using Gizmos;
    public class GitGizmo : MonoBehaviour
    {
        public Gizmo connectedGizmo;
        public string connectedGizmoName;
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
            Debug.Log("skipping gizmo load at " + index);
            while (index < lines.Length && !lines[index].Contains('}')) index++;
            return null;
        }
    }
}
#endif