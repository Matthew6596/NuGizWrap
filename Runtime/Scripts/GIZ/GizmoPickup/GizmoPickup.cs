#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Giz = UnityEngine.Gizmos;

namespace TTModdingKit.Gizmos
{
    public class GizmoPickup : Gizmo
    {
        public enum Type { SilverStud= (byte)'s', GoldStud= (byte)'g', BlueStud= (byte)'b', PurpleStud= (byte)'p', 
            Minikit= (byte)'m', Powerup= (byte)'u', Heart= (byte)'h', 
            RedBrick= (byte)'r', ChallengeMinikit= (byte)'c', Torpedo= (byte)'t'
        }
        public enum SpawnType { None, Triggered=2, AutoCollect=6 }

        public Type type;
        public SpawnType spawnType;
        public byte spawnGroup;

        private bool meshGenerated = false;
        private Type prevType;

        private static Mesh studMesh, minikitMesh;

        private void OnValidate()
        {
            if (!Enum.IsDefined(typeof(Type), type)) type = Type.SilverStud;
            if (prevType != type || !meshGenerated) EditorApplication.delayCall += RefreshModel;
            prevType = type;
        }

        [MenuItem("TT Modding/Gizmos/Pickups/Refresh Models")]
        private static void RefreshAllPickupModels()
        {
            foreach (var pup in FindObjectsByType<GizmoPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None)) pup.RefreshModel();
        }

        private void RefreshModel()
        {
            if (this == null || gameObject == null) return;
            if (!TryGetComponent(out MeshFilter filter)) filter = gameObject.AddComponent<MeshFilter>();

            //Get pickup meshes
            if (studMesh == null) studMesh = TTResourceManager.LoadEditorAsset<Mesh>("Models/stud", ".mesh");
            if (minikitMesh == null) minikitMesh = TTResourceManager.LoadEditorAsset<Mesh>("Models/cylinder", ".mesh");

            filter.mesh = (type) switch
            {
                //To do: add meshes for other pickups
                Type.Minikit => minikitMesh,
                Type.ChallengeMinikit => minikitMesh,
                _ => studMesh,
            };

            if (!TryGetComponent(out MeshRenderer renderer)) renderer = gameObject.AddComponent<MeshRenderer>();
            Material mat = new(Shader.Find("Universal Render Pipeline/Lit"));

            mat.color = (type) switch
            {
                Type.SilverStud => Color.silver,
                Type.GoldStud => Color.gold,
                Type.BlueStud => Color.blue,
                Type.PurpleStud => Color.purple,
                Type.Minikit => Color.whiteSmoke,
                Type.Powerup => Color.cornflowerBlue,
                Type.Heart => Color.darkRed,
                Type.RedBrick => Color.red,
                Type.ChallengeMinikit => Color.blue,
                Type.Torpedo => Color.magenta,
                _ => Color.black
            };
            renderer.material = mat;

            meshGenerated = true;
        }
    }

    public static class EnumExt 
    {
        public static T ToEnumOrDefault<T>(this int value, T defaultValue) where T : Enum => Enum.IsDefined(typeof(T), value) ? (T)(object)value : defaultValue;
    }

}
#endif