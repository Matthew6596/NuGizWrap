#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace NuGizWrap.GizFlow
{
    using Codice.CM.Common.Tree;
    using Helper;
    using System;
    using System.Linq;

    [CustomEditor(typeof(CustomGitNodeAsset))]
    public class CustomGitNodeAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if(GUILayout.Button("Edit in Custom GitNode Window"))
            {

            }

            //TEMP
            serializedObject.ApplyModifiedProperties();
            return;

            var nodeProp = serializedObject.FindProperty("node");
            var nodesProp = nodeProp.FindPropertyRelative("nodes");
            var inpsProp = nodeProp.FindPropertyRelative("inputs");
            var outsProp = nodeProp.FindPropertyRelative("outputs");

            string[] boxNames = new string[nodesProp.arraySize];
            for(int i = 0; i < nodesProp.arraySize; i++)
            {
                var node = nodesProp.GetArrayElementAtIndex(i);
                boxNames[i] = node==null||node.boxedValue == null ? $"null_node_{i}" : (node.boxedValue as GitBox).name;
            }

            EditorExt.Header("Input & Output Ports");

            EditorExt.HorizontalRule();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Input Ports:");
            if(GUILayout.Button("Add Input Port"))
            {
                inpsProp.InsertArrayElementAtIndex(inpsProp.arraySize);
            }
            EditorGUILayout.EndHorizontal();

            void DoPortsList(string portTypeName, Action<SerializedProperty,SerializedProperty> portLogic)
            {
                List<int> inputDeletions = new();
                for (int i = 0; i < inpsProp.arraySize; i++)
                {
                    EditorGUILayout.Space(10);

                    var inp = inpsProp.GetArrayElementAtIndex(i);
                    var inpName = inp.FindPropertyRelative("name");
                    var inpBox = inp.FindPropertyRelative("nodeIndex");
                    var inpPort = inp.FindPropertyRelative($"{portTypeName}PortIndex");

                    string msg = "";
                    MessageType msgType = MessageType.None;


                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("-", GUILayout.Width(20))) inputDeletions.Add(i);
                    EditorGUILayout.PropertyField(inpName, new GUIContent($"Port #{i} | Name:"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    if (boxNames.Length == 0)
                    {
                        msg = $"Add nodes in the Custom GitNode Window to be able to map an {portTypeName} port.";
                        msgType = MessageType.Info;
                    }
                    else
                    {
                        if (inpBox.intValue >= boxNames.Length) inpBox.intValue = 0;
                        inpBox.intValue = EditorGUILayout.Popup(inpBox.intValue, boxNames);
                        var inpNode = nodesProp.GetArrayElementAtIndex(inpBox.intValue);

                        if (inpNode == null || inpNode.boxedValue == null)
                        {
                            msg = $"The {portTypeName} node is null.";
                            msgType = MessageType.Warning;
                        }
                        else
                        {
                            portLogic(inpNode, inpPort);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (msg != "")
                    {
                        EditorGUILayout.HelpBox(msg, msgType);
                    }
                }

                foreach (var inpDel in inputDeletions) inpsProp.DeleteArrayElementAtIndex(inpDel);
            }

            DoPortsList("input", (inpNode, inpPort) =>
            {
                if (inpNode.boxedValue is CustomGitBox customNode)
                {
                    string[] customOpts = customNode.inputs.Select(x => x.name).ToArray();
                    if (inpPort.intValue >= customOpts.Length || inpPort.intValue < 0) inpPort.intValue = 0;

                    inpPort.intValue = EditorGUILayout.Popup(inpPort.intValue, customOpts);
                }
                else inpPort.intValue = 0;
            });

            EditorExt.HorizontalRule();
            EditorGUILayout.LabelField("Output Ports:");
            DoPortsList("output", (outNode, outPort) =>
            {
                if (outNode.boxedValue is CustomGitBox customNode)
                {
                    string[] customOpts = customNode.outputs.Select(x => x.name).ToArray();
                    if (outPort.intValue >= customOpts.Length || outPort.intValue < 0) outPort.intValue = 0;

                    outPort.intValue = EditorGUILayout.Popup(outPort.intValue, customOpts);
                }
                else if (outNode.boxedValue is GitBox boxNode && boxNode != null)
                {
                    //TO-DO
                    //Get box outputs (default/gizmos and such)
                    //Create popup for output selection
                }
                else outPort.intValue = 0;
            });


            EditorExt.HorizontalRule();
            //EditorExt.Header("Nodes:");
            //EditorGUILayout.PropertyField(nodesProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif