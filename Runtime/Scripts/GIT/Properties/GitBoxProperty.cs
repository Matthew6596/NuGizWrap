#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace NuGizWrap.GizFlow
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
        public virtual string ToLine() => name;

        public abstract object Clone();
    }
}
#endif