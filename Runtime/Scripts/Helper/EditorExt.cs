#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using UnityEngine.SceneManagement;
using TTModdingKit.Gizmos;
using System.Linq;

namespace TTModdingKit.Helper
{
    public static class EditorExt
    {
        private static readonly GUIStyle headerStyle = new() { fontStyle = FontStyle.Bold, normal=new() {textColor=Color.white } };

        public static void Header(string txt)
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField(txt, headerStyle);
        }

        public static void Prop(this SerializedObject obj, string propName) => EditorGUILayout.PropertyField(obj.FindProperty(propName));
        public static void Props(this SerializedObject obj, params string[] propNames)
        {
            foreach (var propName in propNames) obj.Prop(propName);
        }

        public static void Prop(this SerializedProperty prop, string propName, Rect rect) => EditorGUI.PropertyField(rect, prop.FindPropertyRelative(propName));

        /// <summary>
        /// Creates EditorGUI for the GizmoSection's version as a readonly label, but with a link to edit at the GizmoSecion GameObject.
        /// </summary>
        /// <typeparam name="T">The GizmoSection's specific type</typeparam>
        /// <param name="section">The GizmoSection</param>
        /// <param name="getVersion">A function to get the section's version</param>
        /// <param name="name">The name of the type of gizmo in the section (or the GizmoSection class name w/out "Section")</param>
        /// <param name="version">The version of the GizmoSection</param>
        /// <returns>Whether an instance of the GizmoSection exists</returns>
        public static bool CreateVersionEditorGUI<T>(this T section, Func<T, int> getVersion, string name, out int version) where T : GizmoSection
        {
            if (section == null)
            {
                EditorGUILayout.HelpBox($"There must be an instance of {name}Section in the scene.", MessageType.Error);
                version = 0;
                return false;
            }
            else CheckSectionCompatibility(section);
            version = getVersion(section);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{name} Version: {version}");
            if (EditorGUILayout.LinkButton("Edit Version")) Selection.activeGameObject = section.gameObject;
            EditorGUILayout.EndHorizontal();
            return true;
        }

        public static bool CheckSectionCompatibility<T>(this T section) where T : GizmoSection
        {
            //Add label showing all compatible games for this gizmo
            StringBuilder sb = new($"{section.ID} is compatible with:");
            foreach (TTGame val in Enum.GetValues(typeof(TTGame)))
            {
                if (section.IsGameCompatible(val)) sb.Append($" {val},");
            }
            GUIContent lbl = new(sb.ToString()[..^1]);
            EditorGUILayout.LabelField(lbl, EditorStyles.miniLabel);

            //Check if compatible, if not show warning message
            bool compatible = section.IsGameCompatible(TTUnityProject.Game);
            if (!compatible)
            {
                EditorGUILayout.HelpBox($"{section.ID} is not compatible with the project's current target game ({TTUnityProject.Game}) and will not be exported.", MessageType.Warning);
                if (EditorGUILayout.LinkButton("Change Project Settings")) Selection.activeObject = TTUnityProject.Instance;
            }

            return compatible;
        }

        public static bool CheckSectionCompatibilityAndVersion<T>(this T section, SerializedObject serializedObject, bool editable=true) where T : GizmoSection
        {
            if (!CheckSectionCompatibility(section)) return false;
            var versionProp = serializedObject.FindProperty("version");
            int maxVers = section.MaxVersion();
            if (versionProp.intValue < 1) versionProp.intValue = 1;
            if (versionProp.intValue > maxVers) versionProp.intValue = maxVers;
            if (editable) EditorGUILayout.IntSlider(versionProp, 1, maxVers);
            else EditorGUILayout.LabelField($"Version: {versionProp.intValue} (not editable)");
            return true;
        }

        public static void SetIcon<T>(this T obj, ref Texture2D icon, string path, string ext=".png") where T : Object
        {
            if (icon == null)
            {
                icon = TTResourceManager.LoadEditorAsset<Texture2D>(path, ext);
                EditorGUIUtility.SetIconForObject(obj, icon);
            }
        }

        public static void DelayDestroy(this Object obj)
        {
            EditorApplication.delayCall += () => { if (obj != null) Object.DestroyImmediate(obj); };
        }

        public static string FixLength(this string str, int len) => str.Length > len ? str[..len] : str.PadRight(len, '\0');

        public static long ReadLong(this byte[] bytes, ref int index)
        {
            long v = BitConverter.ToInt64(bytes, index);
            index += 8;
            return v;
        }

        public static int ReadInt(this byte[] bytes, ref int index)
        {
            int v = BitConverter.ToInt32(bytes, index);
            index += 4;
            return v;
        }

        public static short ReadShort(this byte[] bytes, ref int index)
        {
            short v = BitConverter.ToInt16(bytes, index);
            index += 2;
            return v;
        }

        public static byte ReadByte(this byte[] bytes, ref int index)
        {
            byte v = bytes[index];
            index++;
            return v;
        }

        public static float ReadFloat(this byte[] bytes, ref int index)
        {
            float v = BitConverter.ToSingle(bytes, index);
            index += 4;
            return v;
        }

        public static Vector3 ReadVector3(this byte[] bytes, ref int index) => 
            new()
            {
                x = ReadFloat(bytes, ref index),
                y = ReadFloat(bytes, ref index),
                z = ReadFloat(bytes, ref index)
            };

        public static string ReadString(this byte[] bytes, ref int index, int len, bool trim=false)
        {
            //Get bytes and replace '\0' with space
            byte[] strBytes = bytes.Skip(index).Take(len).ToArray();
            for (int i = 0; i < len; i++) if (strBytes[i] == 0) strBytes[i] = (byte)' ';

            //Get string
            string str = Encoding.UTF8.GetString(strBytes, 0, len);
            index += len;

            //Trim and return
            return trim ? str.Trim() : str;
        }

        public static string ReadString8(this byte[] bytes, ref int index)
        {
            byte len = bytes.ReadByte(ref index);
            return bytes.ReadString(ref index, len);
        }

        public static string ReadString16(this byte[] bytes, ref int index)
        {
            short len = bytes.ReadShort(ref index);
            return bytes.ReadString(ref index, len);
        }

        public static string ReadString32(this byte[] bytes, ref int index)
        {
            int len = bytes.ReadInt(ref index);
            return bytes.ReadString(ref index, len);
        }

        public static void AddLong(this List<byte> bytes, long value) => bytes.AddRange(BitConverter.GetBytes(value));
        public static void AddInt(this List<byte> bytes, int value) => bytes.AddRange(BitConverter.GetBytes(value));
        public static void AddShort(this List<byte> bytes, short value) => bytes.AddRange(BitConverter.GetBytes(value));
        public static void AddFloat(this List<byte> bytes, float value) => bytes.AddRange(BitConverter.GetBytes(value));
        public static void AddVector3(this List<byte> bytes, Vector3 value)
        {
            AddFloat(bytes, value.x);
            AddFloat(bytes, value.y);
            AddFloat(bytes, value.z);
        }
        //public static void AddString(this List<byte> bytes, string value) => bytes.AddRange(Encoding.UTF8.GetBytes(value.Replace(' ', '\0')));
        public static void AddString(this List<byte> bytes, string value) => bytes.AddRange(Encoding.UTF8.GetBytes(value));
        //public static void AddFixedString(this List<byte> bytes, string value, int len) => bytes.AddRange(Encoding.UTF8.GetBytes(value.FixLength(len).Replace(' ', '\0')));
        public static void AddFixedString(this List<byte> bytes, string value, int len) => bytes.AddRange(Encoding.UTF8.GetBytes(value.FixLength(len)));
        public static void AddString8(this List<byte> bytes, string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length == 0)
            {
                bytes.Add(0);
                return;
            }

            if (value.Length > 254) value = value[..254] + '\0';
            else if (value[^1] != '\0') value += '\0';

            bytes.Add((byte)value.Length);
            bytes.AddString(value);
        }

        public static void AddString16(this List<byte> bytes, string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length == 0)
            {
                bytes.AddShort(0);
                return;
            }

            if (value.Length > short.MaxValue-1) value = value[..(short.MaxValue-1)] + '\0';
            else if (value[^1] != '\0') value += '\0';

            bytes.AddShort((short)value.Length);
            bytes.AddString(value);
        }

        public static void AddString32(this List<byte> bytes, string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length == 0)
            {
                bytes.AddInt(0);
                return;
            }

            if (value.Length > int.MaxValue-1) value = value[..(int.MaxValue-1)] + '\0';
            else if (value[^1] != '\0') value += '\0';

            bytes.AddInt(value.Length);
            bytes.AddString(value);
        }

        public static string ReadString8(this BinaryReader reader) => ReadString(reader, reader.ReadByte());

        public static string ReadString16(this BinaryReader reader) => ReadString(reader, reader.ReadInt16());

        public static string ReadString32(this BinaryReader reader) => ReadString(reader, reader.ReadInt32());
        public static string ReadString(this BinaryReader reader, int length)
        {
            char[] chars = reader.ReadChars(length);
            for (int i = 0; i < length; i++) if (chars[i] == '\0') chars[i] = ' ';
            return new(chars);
        }

        public static Vector3 ReadVector3(this BinaryReader reader) => new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static float ToFloatAng(this ushort ang) => (ang / 65536f) * 360;
        public static ushort ToShortAng(this float ang) => (ushort)((ang / 360f) * 65536f);

        public static T FindInScene<T>(this Scene scene) where T : Component
        {
            T comp = null;
            foreach(var obj in scene.GetRootGameObjects())
            {
                comp = obj.GetComponentInChildren<T>();
                if (comp != null) return comp;
            }
            return comp;
        }

        public static IEnumerable<T> FindAllInScene<T>(this Scene scene) where T : Component
        {
            List<T> comps = new();
            foreach (var obj in scene.GetRootGameObjects()) comps.AddRange(obj.GetComponentsInChildren<T>());
            return comps;
        }
    }
}
#endif