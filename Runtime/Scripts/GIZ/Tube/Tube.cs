#if UNITY_EDITOR
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    using GameScene;

    public class Tube : Gizmo
    {
        public override string[] GetOutputNames(TTGame game) => new[] { "Active" };

        [Min(0.000001f)]
        public float height, radius;
        public bool magnetic;
        public SpecialObjectReference specialObject;
        public bool glideOnly, horizontal;

        public bool canBeHorizontal;

        private static Mesh cylinder;

        private void OnDrawGizmos()
        {
            if (cylinder == null) cylinder = TTResourceManager.LoadEditorAsset<Mesh>("Models/cylinder", ".mesh");
            if (cylinder == null) return;


            Color tubeCol = magnetic ? Color.red : Color.blue;
            tubeCol.a = 0.5f;
            Giz.color = tubeCol;

            float rad2 = radius * 2;
            Vector3 scale = new(rad2, height, rad2);
            float halfH = height / 2;

            if (canBeHorizontal && horizontal) 
            {
                float ang = transform.eulerAngles.y;
                float angR = Mathf.Deg2Rad * ang;
                Vector3 offset = new(Mathf.Sin(angR)* halfH, 0, Mathf.Cos(angR)*halfH);
                Giz.DrawMesh(cylinder, transform.position+offset, Quaternion.Euler(90, ang, 0), scale);
            }
            else Giz.DrawMesh(cylinder, transform.position + new Vector3(0, halfH, 0), Quaternion.identity, scale);
        }
    }
}
#endif