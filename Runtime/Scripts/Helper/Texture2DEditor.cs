#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Obj = UnityEngine.Object;

namespace NuGizWrap.Helper
{
    [CustomEditor(typeof(Texture2D))]
    [CanEditMultipleObjects]
    public class Texture2DEditor : Editor
    {
        private bool _showBase = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            /*EditorGUILayout.IntSlider(anisoLevelProp, 0, 16);
            if(anisoLevelProp.intValue > 1)
            {
                EditorGUILayout.HelpBox("Anisotropic filtering is enabled for all textures in Quality Settings.", MessageType.Info);
            }*/

            EditorGUILayout.LabelField("Reveal Base Inspector (Advanced):");
            _showBase = EditorGUILayout.Toggle(_showBase);
            if (_showBase) base.OnInspectorGUI();

            EditorGUILayout.LabelField("Convert Texture:");
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create .PNG")) ConvertTextureToPNG();
            if (GUILayout.Button("Create .JPG")) ConvertTextureToJPG();
            if (GUILayout.Button("Create .DDS")) ConvertTextureToDDS();

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void ConvertTextureToPNG() => ConvertTexture(t => ConvertTexture2DBytes(t, t2 => t2.EncodeToPNG()), ".png");
        private void ConvertTextureToJPG() => ConvertTexture(t => ConvertTexture2DBytes(t, t2 => t2.EncodeToJPG()), ".jpg");
        private void ConvertTextureToDDS() => ConvertTexture(t => t.TextureToDDSBytes(), ".dds");

        private void ConvertTexture(Func<Texture2D,byte[]> getTextureBytes, string ext)
        {
            foreach (var txtr2d in targets)
            {
                string ogAssetPath = AssetDatabase.GetAssetPath(txtr2d);

                string newTxtrPath = Path.Combine(Application.dataPath, ogAssetPath[7..]);
                newTxtrPath = newTxtrPath.Replace(".texture2D", ext);

                File.WriteAllBytes(newTxtrPath, getTextureBytes(txtr2d as Texture2D));
                //AssetDatabase.DeleteAsset(ogAssetPath);
            }
            AssetDatabase.Refresh();
        }

        private byte[] ConvertTexture2DBytes(Texture2D texture, Func<Texture2D, byte[]> convertMethod)
        {
            RenderTexture rt = RenderTexture.GetTemporary(
            texture.width, texture.height, 0,
            RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Linear);

            Graphics.Blit(texture, rt);

            RenderTexture prevActive = RenderTexture.active;
            RenderTexture.active = rt;

            var readableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            readableTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            readableTexture.Apply();

            RenderTexture.active = prevActive;
            RenderTexture.ReleaseTemporary(rt);

            byte[] bytes = convertMethod(readableTexture);
            
            DestroyImmediate(readableTexture);

            return bytes;
        }
    }
}
#endif