//===== ===== ===== ===== =====
//DOCUMENTATION: https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?tab=t.0#heading=h.8ohmpzqllz92
//-Matton
//===== ===== ===== ===== =====

#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace TTModdingKit.Gizmos
{
    using Helper;
    using System.Linq;

    public class ShadowEditorSection : GizmoSection
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 8, TTGame.LIJ1=>8, TTGame.LB1=>12, _ => 1 };

        public override string ID => "ShadowEditor";

        public static ShadowEditorSection Instance { get; private set; }

        public byte version = 8;

        private void OnValidate()
        {
            Instance = DoSingleton(Instance);
        }

        public override byte[] ToBytes()
        {
            var shadowEdits = FindObjectsByType<ShadowEditor>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            byte shadowEditCount = (byte)shadowEdits.Length;

            List<byte> bytes = new() { version, shadowEditCount };

            for(int i=0; i<shadowEditCount; i++)
            {
                var shadowEdit = shadowEdits[i];
                bytes.AddVector3(shadowEdit.transform.forward);
                bytes.AddFloat(shadowEdit.opacity);
                if (version >= 2)
                {
                    bytes.AddFloat(shadowEdit.unknown2);
                    bytes.AddFloat(shadowEdit.unknown3);
                }
                if (version >= 3)
                {
                    bytes.AddFloat(shadowEdit.unknown4);
                    bytes.AddFloat(shadowEdit.unknown5);
                }
                if (version >= 4) bytes.AddFloat(shadowEdit.renderDistance);
                if (version >= 5)
                {
                    bytes.AddFloat(0); //padding
                    bytes.AddFloat(0);
                }
                if (version >= 6)
                {
                    bytes.AddFloat(shadowEdit.blur);
                    bytes.AddFloat(shadowEdit.unknown10);
                    bytes.AddFloat(shadowEdit.unknown11);
                }
                if (version >= 7) bytes.AddFloat(shadowEdit.quality);
                if (version >= 8) bytes.AddInt((int)shadowEdit.preset);
                if (version >= 9) bytes.AddFloat(shadowEdit.unknown14);
            }

            return bytes.ToArray();
        }

        public override void FromBytes(byte[] bytes, ref int index)
        {
            version = bytes.ReadByte(ref index);
            byte shadowEditCount = bytes.ReadByte(ref index);

            //Destroy existing shadoweditors before adding new ones
            foreach (var shadowEdit in FindObjectsByType<ShadowEditor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) 
                shadowEdit.gameObject.DelayDestroy();

            for(int i=0; i<shadowEditCount; i++)
            {
                GameObject shadowObj = new("shadow_editor");
                shadowObj.transform.SetParent(transform);
                //shadowObj.transform.rotation = Quaternion.LookRotation();
                shadowObj.transform.forward = bytes.ReadVector3(ref index);
                var shadowEdit = shadowObj.AddComponent<ShadowEditor>();

                shadowEdit.opacity = bytes.ReadFloat(ref index);
                if (version >= 2)
                {
                    shadowEdit.unknown2 = bytes.ReadFloat(ref index);
                    shadowEdit.unknown3 = bytes.ReadFloat(ref index);
                }
                if (version >= 3)
                {
                    shadowEdit.unknown4 = bytes.ReadFloat(ref index);
                    shadowEdit.unknown5 = bytes.ReadFloat(ref index);
                }
                if (version >= 4) shadowEdit.renderDistance = bytes.ReadFloat(ref index);
                if (version >= 5)
                {
                    bytes.ReadFloat(ref index); //padding
                    bytes.ReadFloat(ref index);
                }
                if (version >= 6)
                {
                    shadowEdit.blur = bytes.ReadFloat(ref index);
                    shadowEdit.unknown10 = bytes.ReadFloat(ref index);
                    shadowEdit.unknown11 = bytes.ReadFloat(ref index);
                }
                if (version >= 7) shadowEdit.quality = bytes.ReadFloat(ref index);
                if (version >= 8) shadowEdit.preset = (ShadowEditor.Preset)bytes.ReadInt(ref index);
                if (version >= 9) shadowEdit.unknown14 = bytes.ReadFloat(ref index);

                if (version >= 8) shadowEdit.RefreshPreset();
            }
        }
    }
}
#endif