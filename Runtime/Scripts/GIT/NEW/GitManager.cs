#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NuGizWrap.GizFlow
{
    public class GitManager : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 cameraPosition;

        public GitOptionsAsset options;

        [SerializeReference] public GitBox[] boxes = new GitBox[0];
        public Connection[] connections = new Connection[0];

        //Primarily useful for CustomGitNodes, but can be used anywhere
        public Variable[] variables;

        //Other properties
        public bool visualizeInScene;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDrawGizmos()
        {
            if (!visualizeInScene) return;
        }

        private void OnDrawGizmosSelected()
        {
            if (!visualizeInScene) return;
        }

        [ContextMenu("Export .GIT File")]
        public void Export()
        {
            GITExporter.gm = this;
            GITExporter.Export();
        }

        public IEnumerable<int> GetChildren(GitBox parent)
        {
            var conns = connections.Where(c => c.parent == parent);
            return conns.Select(c => GITExporter.boxIDs[c.child]);
        }

        public string GetParentsStr(GitBox child)
        {
            var conns = connections.Where(c => c.child == child);
            string parentsStr = string.Empty;
            foreach(var p in conns.Select(c => (GITExporter.boxIDs[c.parent], c.parentOutput)))
            {
                parentsStr += $"\n\tParent {p.Item1} {p.parentOutput}";
            }
            return parentsStr;
        }

        [Serializable]
        public struct Variable
        {
            public string name;
            public string value;
        }
    }

    [Serializable]
    public class Connection
    {
        [SerializeReference] public GitBox parent;
        public int parentOutput;
        [SerializeReference] public GitBox child;
        public int childInput;
    }
}
#endif