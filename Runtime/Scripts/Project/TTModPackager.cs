#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace NuGizWrap
{
    using Helper;
    public enum TTGame { Unspecified, TCS=1, LIJ1=2, LB1=3 }

    public static class TTModPackager
    {
        public enum FileDiffType { None=0, Addition=1, Change=2, Removal=3 }

        const int magic = 1347253332; //TtMP (0x54744D50)
        const int packagerVersion = 1;

        private static string targetModPath;

        [MenuItem("Nu Giz Wrap/Package/Existing Mod")]
        public static void ExportPackage()
        {
            //Select a built mod to package
            string modPath = EditorUtility.OpenFolderPanel("Select Mod to Package", "", "");
            if (string.IsNullOrEmpty(modPath) || !Directory.Exists(modPath)) return;
            targetModPath = modPath;

            Export();
        }

        private static void Export()
        {
            //Select location for ttmod package file
            string dir = Path.Combine(TTUnityProject.Instance.modManagerSettings.dataPath,"mod_packages");
            string packagePath = EditorUtility.SaveFilePanel("Export Mod Package", Directory.Exists(dir) ? dir : "", "mymod", "ttmod");
            if (string.IsNullOrEmpty(packagePath) || !Directory.Exists(Path.GetDirectoryName(packagePath))) return;

            try
            {
                //Package
                PackageMod(packagePath);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                EditorUtility.ClearProgressBar();
                return;
            }
        }

        /// <summary>
        /// Creates a ttmod file for your mod.
        /// </summary>
        public static void PackageMod(string path)
        {
            List<byte> bytes = new();
            using FileStream fs = new(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920);
            using BinaryWriter bw = new(fs);

            EditorUtility.DisplayProgressBar("Packaging Mod", $"Generating {Path.GetFileName(path)}...", 0);

            bw.Write(magic);
            bw.Write(packagerVersion);
            bw.Write(TTUnityProject.Instance.modMeta.ToBytes());

            //Export Levels

            //Export Audio

            //Export Project Assets (.txt)

            bw.Flush();
            bw.Dispose();

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Package Created", $"TT Mod Package created at {path}", "OK");
        }

        private static FileChange[] GetFileChanges(string vanillaFile, string modFile)
        {
            byte[] vanillaBytes = File.ReadAllBytes(vanillaFile);
            byte[] modBytes = File.ReadAllBytes(modFile);

            List<FileChange> changes = new();

            int i = 0;
            while (i < vanillaBytes.Length || i < modBytes.Length)
            {
                // Find start of a differing region
                if (i < vanillaBytes.Length && i < modBytes.Length && vanillaBytes[i] == modBytes[i])
                {
                    i++;
                    continue;
                }

                int changeStart = i;

                // Scan forward until we're back in sync (both files match for a run of bytes)
                // Use a small look-ahead to avoid splitting logically contiguous changes
                const int syncRunLength = 8;
                while (i < vanillaBytes.Length || i < modBytes.Length)
                {
                    // Check if we've re-synced
                    bool inSync = true;
                    for (int k = 0; k < syncRunLength; k++)
                    {
                        int idx = i + k;
                        bool vanillaEnd = idx >= vanillaBytes.Length;
                        bool modEnd = idx >= modBytes.Length;

                        if (vanillaEnd != modEnd || (!vanillaEnd && vanillaBytes[idx] != modBytes[idx]))
                        {
                            inSync = false;
                            break;
                        }
                    }
                    if (inSync) break;
                    i++;
                }

                // Slice out vanilla and mod regions for this change
                int vanillaLen = Math.Min(i, vanillaBytes.Length) - changeStart;
                int modLen = Math.Min(i, modBytes.Length) - changeStart;

                byte[] newContent = new byte[modLen];
                if (modLen > 0)
                    Array.Copy(modBytes, changeStart, newContent, 0, modLen);

                changes.Add(new FileChange(changeStart, vanillaLen, newContent));
            }

            return changes.ToArray();
        }

        public struct FileChange
        {
            public int changeStartIndex;
            public int originalContentLength;
            public byte[] newContent;

            public FileChange(int changeStartInd, int ogContentLength, byte[] newContent)
            {
                this.changeStartIndex = changeStartInd;
                this.originalContentLength = ogContentLength;
                this.newContent = newContent;
            }

            public readonly byte[] ToBytes()
            {
                List<byte> bytes = new();

                bytes.AddInt(changeStartIndex);
                bytes.AddInt(originalContentLength);
                bytes.AddInt(newContent.Length);
                bytes.AddRange(newContent);

                return bytes.ToArray();
            }
        }
    }

    [Serializable]
    public struct ModMeta
    {
        public string name, author, version, description;
        public TTGame game;
        public DateTime releaseDate;
        public ModIcon icon;
        public TextAsset readme;

        public ModMeta(string name, string author, string version, string description, TTGame game, DateTime releaseDate, ModIcon icon, TextAsset readme)
        {
            this.name = name;
            this.author = author;
            this.version = version;
            this.description = description;
            this.releaseDate = releaseDate;
            this.icon = icon;
            this.readme = readme;
            this.game = game;
        }

        public readonly byte[] ToBytes()
        {
            List<byte> bytes = new();
            bytes.AddString8(name);
            bytes.AddString8(author);
            bytes.AddString8(version);
            bytes.Add((byte)game);
            bytes.AddLong(DateTime.Now.ToBinary());
            bytes.AddString8(description);
            bytes.AddRange(icon.ToBytes());
            bytes.AddInt((int)readme.dataSize);
            bytes.AddRange(readme.bytes);
            return bytes.ToArray();
        }

        public static ModMeta FromBytes(BinaryReader reader)
        {
            string name = reader.ReadString8();
            string author = reader.ReadString8();
            string version = reader.ReadString8();
            TTGame game = (TTGame)reader.ReadByte();
            DateTime release = DateTime.FromBinary(reader.ReadInt64());
            string desc = reader.ReadString8();
            ModIcon icon = ModIcon.FromBytes(reader);
            int textAssetSize = reader.ReadInt32();
            TextAsset readme = new(reader.ReadBytes(textAssetSize));

            return new ModMeta(name,author,version,desc,game,release,icon,readme);
        }
    }

    [Serializable]
    public struct ModIcon
    {
        public enum ImageType { PNG=0, JPG=1, DDS=2, ICO=3 }

        public Texture2D texture;
        public ImageType type;

        public ModIcon(Texture2D texture, ImageType type)
        {
            this.texture = texture;
            this.type = type;
        }

        public readonly byte[] ToBytes()
        {
            if (texture == null) return new byte[] { 0xff, 0,0,0,0 };

            List<byte> bytes = new() { (byte)type };

            byte[] txtrBytes = (type) switch
            {
                //ImageType.PNG => texture.EncodeToPNG(),
                //ImageType.JPG => texture.EncodeToJPG(),
                //ImageType.DDS => texture.TextureToDDSBytes(),
                //ImageType.ICO => texture.TextureToICOBytes(),
                _ => texture.EncodeToPNG()
            };

            bytes.AddInt(txtrBytes.Length);
            bytes.AddRange(txtrBytes);

            return bytes.ToArray();
        }

        public static ModIcon FromBytes(BinaryReader reader)
        {
            byte imgType = reader.ReadByte();
            int txtrLen = reader.ReadInt32();
            if (imgType == 0xff) return new ModIcon(null,0);

            ImageType type = (ImageType)imgType;
            byte[] txtrBytes = reader.ReadBytes(txtrLen);
            Texture2D txtr = new(1,1);
            switch(type)
            {
                //case ImageType.PNG: txtr.LoadImage(txtrBytes); break;
                //case ImageType.JPG: txtr.LoadImage(txtrBytes); break;
                //case ImageType.DDS: DDSConvert.DDSBytesToTexture(txtrBytes); break;
                //case ImageType.ICO: ICOConvert.ICOBytesToLargestTexture(txtrBytes); break;
                default: txtr.LoadImage(txtrBytes); break;
            }
            return new ModIcon(txtr, type);
        }
    }
}
#endif