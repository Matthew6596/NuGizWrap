#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace TTModdingKit.GizFlow
{
    public class GitStringProperty : GitBoxProperty
    {
        private readonly TextField textField;
        private string value;

        public GitStringProperty(string name, string initialValue = "") : base(name)
        {
            value = initialValue;
            textField = new TextField(name);
            textField.SetValueWithoutNotify(value);
            textField.RegisterValueChangedCallback((e) => { value = e.newValue; OnValueChanged.Invoke(value); });
            RootVisualElement.Add(textField);
        }

        public override T GetValue<T>() => (T)Convert.ChangeType(textField.value,typeof(T));

        public override void SetValue(object value)
        {
            this.value = value.ToString();
            textField.value = this.value;
        }

        public override object Clone() => new GitStringProperty(name);
    }
}
#endif