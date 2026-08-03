#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Giz = UnityEngine.Gizmos;
using System;
using System.Text;
using System.Linq;

namespace TTModdingKit.Terrain
{
    public enum SurfaceType
    {
        Unknown = 67000, //Meant to be temp (one day we'll know ALL surface/terrain types) //Magenta
        None,                // Black
        Slip,             // Yellow
        Water,               // Blue
        Instakill,         // Hot pink / rose red
        Fastkill,          // Red
        Slowkill,            // Orange-brown
        R2SwampWater,        // Olive green
        PushblockSurface,    // Purple
        Edge,             // Dark blue
        ForceMovable,     // Medium green
        GameMovable,      // Teal
        SpinnerSide,      // Bright green
        Ice,              // Light cyan / ice blue
        MetalObject,      // Gray
        EnergyWall,       // Cyan
        ReflectiveFloor,   // Black
        MapCustomFloor,   // Pink
        Button,           // Lime green
        StopHover,        // Very dark red / near black
    }

    public class TerrainMesh : MonoBehaviour
    {
        public List<Face> faces = new();

        private void OnDrawGizmosSelected()
        {
            //Color tempTerCol = new(1, 1, 0, 0.5f);
            //Color tempTerOutlineCol = new(0.33f, 0.33f, 0, 0.05f);

            //Giz.color = tempTerOutlineCol;

            foreach (Face face in faces)
            {
                Giz.color = face.GetColor();

                Vector3[] verts = face.Vertices;
                Giz.DrawLine(verts[0], verts[1]);
                Giz.DrawLine(verts[1], verts[2]);
                Giz.DrawLine(verts[2], verts[0]);
                if (!face.OnlyOneTri)
                {
                    Giz.DrawLine(verts[0], verts[2]);
                    Giz.DrawLine(verts[2], verts[3]);
                    Giz.DrawLine(verts[3], verts[0]);
                }
                /*Giz.DrawCube(verts[0], Vector3.one * 0.1f);
                Giz.DrawCube(verts[1], Vector3.one * 0.1f);
                Giz.DrawCube(verts[2], Vector3.one * 0.1f);
                Giz.DrawCube(verts[3], Vector3.one * 0.1f);*/
            }
        }

        [Serializable]
        public class Face
        {
            public Vector3 min, max, p1, p2, p3, p4, norm1, norm2;
            public int property1, property2, flag1, flag2;

            public Vector3[] Vertices => new Vector3[] { p1, p2, p3, p4 };
            //public static int[] Indicies => new int[] { 0, 1, 2, 0, 2, 3 };
            public Vector3[] Normals => new Vector3[] { norm1, norm2 };
            public bool OnlyOneTri => norm2 == new Vector3(0, 65536f, 0);

            public Color GetColor()
            {
                var terPrefs = TTUnityProject.Prefs.terrain;
                var colorDictionary = terPrefs.terrainColors ?? TTUnityProject.Preferences.Default.terrain.terrainColors;
                Color col = colorDictionary[GetSurfaceType(property1, property2)];
                col.a = terPrefs.terrainAlpha;
                return col;
            }

            public static SurfaceType GetSurfaceType(int surface, int layer) => (layer, surface) switch
            {
                (0,0) => SurfaceType.None,
                (0,1) => SurfaceType.Fastkill,
                (0,2) => SurfaceType.ReflectiveFloor,
                (0,6) => SurfaceType.Button,
                (0,9) => SurfaceType.Ice,
                (0,12) => SurfaceType.EnergyWall,
                (0,20) or (0,14) or (0,15) => SurfaceType.MapCustomFloor,
                (0,16) => SurfaceType.Slip,
                (0,19) => SurfaceType.PushblockSurface,
                (0,22) => SurfaceType.Edge,
                (0,24) => SurfaceType.GameMovable,
                (0,25) => SurfaceType.ForceMovable,
                (0,26) => SurfaceType.StopHover,
                (0,27) => SurfaceType.MetalObject,
                (0,30) or (0,31) => SurfaceType.SpinnerSide,
                (1,0) => SurfaceType.Water,
                (3,0) => SurfaceType.Instakill,
                (6,0) => SurfaceType.Slowkill,
                (8,0) => SurfaceType.PushblockSurface,
                (9,0) => SurfaceType.R2SwampWater,
                _ => SurfaceType.Unknown
            };

            public override string ToString()
            {
                return $"{p1}, {p2}, {p3}, {p4}, n1:{norm1}, n2:{norm2}, f1:{flag1}, f2:{flag2}";
            }
        }

        [MenuItem("TT Modding/Analysis/Log Loaded Terrain Types")]
        public static void LogAllLoadedSurfaceTypes()
        {
            List<(int, int)> norm2Types = new();
            StringBuilder sb = new("Listing all faces:\n");
            Dictionary<(int, int), int> types = new();
            foreach(var mesh in FindObjectsByType<TerrainMesh>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                foreach(var face in mesh.faces)
                {
                    var type = (face.property1, face.property2);
                    if (types.ContainsKey(type)) types[type]++;
                    else types.Add(type, 1);
                    
                    sb.AppendLine(face.ToString());
                }
            }
            Debug.Log(sb.ToString());

            sb = new("Found the following terrain surface types:\n");
            foreach (var pair in types) 
            {
                var type = pair.Key;
                int amt = pair.Value;
                string typeName = Face.GetSurfaceType(type.Item1, type.Item2).ToString();
                if (typeName == "None" && (type.Item1 != 0 || type.Item2 != 0)) typeName = "Unknown";
                sb.AppendLine($"{typeName} (surface:{type.Item1},layer:{type.Item2}): {amt}");
            }

            Debug.Log(sb.ToString());
        }
    }
}
#endif