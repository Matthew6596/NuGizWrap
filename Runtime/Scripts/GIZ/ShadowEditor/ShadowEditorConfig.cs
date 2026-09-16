#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NuGizWrap.Gizmos
{
    using Helper;

    public class ShadowEditorConfig : GizmoTypeConfig
    {
        public override bool IsGameCompatible(TTGame game) => game.CompareGames(TTGame.LB1, TTGame.TCS, TTGame.LIJ1);
        public override int GetMaxVersion(TTGame game) => (game) switch { TTGame.TCS => 8, TTGame.LIJ1 => 8, TTGame.LB1 => 12, _ => 1 };

        public byte version = 8;

        public override int Load(BinaryReader br, Transform parent)
        {
            version = br.ReadByte();
            byte shadowEditCount = br.ReadByte();

            for (int i = 0; i < shadowEditCount; i++)
            {
                GameObject shadowObj = new($"shadow_editor_{i}");
                shadowObj.transform.SetParent(parent);
                shadowObj.transform.forward = br.ReadVector3();
                var shadowEdit = shadowObj.AddComponent<ShadowEditor>();

                shadowEdit.opacity = br.ReadSingle();
                if (version >= 2)
                {
                    shadowEdit.unknown2 = br.ReadSingle();
                    shadowEdit.unknown3 = br.ReadSingle();
                }
                if (version >= 3)
                {
                    shadowEdit.unknown4 = br.ReadSingle();
                    shadowEdit.unknown5 = br.ReadSingle();
                }
                if (version >= 4) shadowEdit.renderDistance = br.ReadSingle();
                if (version >= 5)
                {
                    br.ReadSingle(); //padding
                    br.ReadSingle();
                }
                if (version >= 6)
                {
                    shadowEdit.blur = br.ReadSingle();
                    shadowEdit.unknown10 = br.ReadSingle();
                    shadowEdit.unknown11 = br.ReadSingle();
                }
                if (version >= 7) shadowEdit.quality = br.ReadSingle();
                if (version >= 8) shadowEdit.preset = (ShadowEditor.Preset)br.ReadInt32();
                if (version >= 9) shadowEdit.unknown14 = br.ReadSingle();

                if (version >= 8) shadowEdit.RefreshPreset();
            }

            return shadowEditCount;
        }

        public override void Save(BinaryWriter bw)
        {
            var shadowEdits = FindObjectsByType<ShadowEditor>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).Reverse().ToArray();
            byte shadowEditCount = (byte)shadowEdits.Length;

            bw.Write(version);
            bw.Write(shadowEditCount);

            for (int i = 0; i < shadowEditCount; i++)
            {
                var shadowEdit = shadowEdits[i];
                bw.Write(shadowEdit.transform.forward);
                bw.Write(shadowEdit.opacity);
                if (version >= 2)
                {
                    bw.Write(shadowEdit.unknown2);
                    bw.Write(shadowEdit.unknown3);
                }
                if (version >= 3)
                {
                    bw.Write(shadowEdit.unknown4);
                    bw.Write(shadowEdit.unknown5);
                }
                if (version >= 4) bw.Write(shadowEdit.renderDistance);
                if (version >= 5)
                {
                    bw.Write(0f); //padding
                    bw.Write(0f);
                }
                if (version >= 6)
                {
                    bw.Write(shadowEdit.blur);
                    bw.Write(shadowEdit.unknown10);
                    bw.Write(shadowEdit.unknown11);
                }
                if (version >= 7) bw.Write(shadowEdit.quality);
                if (version >= 8) bw.Write((int)shadowEdit.preset);
                if (version >= 9) bw.Write(shadowEdit.unknown14);
            }
        }
    }
}
#endif