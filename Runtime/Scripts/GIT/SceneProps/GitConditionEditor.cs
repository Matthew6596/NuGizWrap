#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TTModdingKit.GizFlow
{
    using Helper;

    [CustomEditor(typeof(GitCondition))]
    public class GitConditionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var typeProp = serializedObject.FindProperty("type");
            EditorGUILayout.PropertyField(typeProp);

            var type = (GitCondition.Type)typeProp.boxedValue;
            string typeNote = (type) switch
            {
                GitCondition.Type.Any => "This condition will output if any of its inputs are true. (OR gate)",
                GitCondition.Type.All => "This condition will only output when all of its inputs are true. (AND gate)",
                GitCondition.Type.None => "This condition will only output when none of its inputs are true. (NOT/NOR gate)",
                GitCondition.Type.Loop => "This condition will loop to its output. If it has no output, it'll loop to the highest parent node.",
                GitCondition.Type.Exactly => "This condition will only output when the exact number of inputs specified are outputting.",
                _ => "unknown gitcondition type"
            };

            if (type == GitCondition.Type.Exactly) serializedObject.Prop("exactlyInputAmount");

            EditorGUILayout.HelpBox(typeNote, MessageType.Info);

            string monitorInputsTip = "When true, the inputs to this node are monitored for changes. (If unsure, I'd recommend checking this to be true.)";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("monitorInputs"), new GUIContent("Monitor Inputs",monitorInputsTip));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif