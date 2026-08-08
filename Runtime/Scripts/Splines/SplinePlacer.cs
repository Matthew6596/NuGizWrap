#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Splines;

namespace NuGizWrap.Helper.Splines 
{
    [ExecuteInEditMode]
    public class SplinePlacer : MonoBehaviour
    {
        public SplineContainer splineContainer;

        [Range(0f,1f)]
        public float offset = 0;

        private void Start()
        {
            if (splineContainer == null) splineContainer = GetComponent<SplineContainer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (splineContainer == null) return;

            int childCount = transform.childCount;
            float rate = 1/(float)childCount;
            float percent = offset;
            for(int i=0; i<childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.position = splineContainer.EvaluatePosition(percent);

                percent += rate;
                if (percent > 1) percent--;
            }
        }
    }
}
#endif