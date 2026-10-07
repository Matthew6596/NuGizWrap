#if UNITY_EDITOR
using UnityEngine;

namespace NuGizWrap.Tools
{
    using Gizmos;

    public class GizmoPropertyVisualizer : MonoBehaviour
    {
        public Transform graphTransform;
        public GizmoConfig[] gizmos;

        private int selectedGizmoTypeIndex;
        private string SelectedGizmoType => GizmoConfig.GizmoNames[selectedGizmoTypeIndex];

        private void Awake()
        {
            if(graphTransform == null) graphTransform = transform;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GenerateGraph()
        {

        }

        private Transform GenerateLevelChart(GizmoConfig config)
        {
            string levelName = config.name;
            string gizmoType = SelectedGizmoType;

            GameObject lvlObj = new($"{levelName}_graph");
            Transform lvlGraph = lvlObj.transform;

            //

            return lvlGraph;
        }

        public void PageGizmoTypeLeft()
        {

        }

        public void PageGizmoTypeRight()
        {

        }
    }
}
#endif