#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace TTModdingKit.GizFlow
{
    public abstract class GitBoxProperty : ICloneable
    {
        public string name;

        public VisualElement RootVisualElement { get; private set; }
        public UnityEvent<object> OnValueChanged = new();

        public GitBoxProperty(string name)
        {
            this.name = name;
            RootVisualElement = new();
        }

        public abstract T GetValue<T>();
        public abstract void SetValue(object value);
        public virtual void LoadValue(string valueStr) { }

        public abstract object Clone();
    }
}
#endif